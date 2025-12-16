using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteka.Domain;


// Singleton z zasadami działania biblioteki:
// - godziny otwarcia (Pn–Pt 08:00–18:00)
// - wyliczanie terminu odbioru rezerwacji: do końca drugiego następnego dnia roboczego (18:00)

public sealed class LibraryRules : ILibraryRules
{
    // Singleton – jedna instancja dla całej aplikacji
  
    public TimeOnly OpenFrom { get; init; } = new(8, 0);
    public TimeOnly OpenTo { get; init; } = new(18, 0);

    public bool IsWorkingDay(DateOnly date)
        => date.DayOfWeek is >= DayOfWeek.Monday and <= DayOfWeek.Friday;

   
    // Liczy termin odbioru:
    // - jeśli rezerwacja przed otwarciem: traktujemy jak otwarcie (08:00)
    // - jeśli po zamknięciu lub w weekend: start = najbliższy dzień roboczy 08:00
    // - deadline = koniec drugiego następnego dnia roboczego (18:00)
    // Uwaga: operujemy na czasie lokalnym serwera/aplikacji.
   
    public DateTime CalculatePickupDeadline(DateTime reservationLocalTime)
    {
        // 1) Ustal "efektywny start"
        var startDate = DateOnly.FromDateTime(reservationLocalTime);
        var startTime = TimeOnly.FromDateTime(reservationLocalTime);

        // jeśli weekend -> przeskocz do najbliższego dnia roboczego
        if (!IsWorkingDay(startDate))
        {
            startDate = NextWorkingDay(startDate);
            startTime = OpenFrom;
        }

        // jeśli przed otwarciem -> ustaw na 08:00 tego samego dnia
        if (startTime < OpenFrom)
            startTime = OpenFrom;

        // jeśli po zamknięciu (lub równo 18:00) -> następny dzień roboczy 08:00
        if (startTime >= OpenTo)
        {
            startDate = NextWorkingDay(startDate);
            startTime = OpenFrom;
        }

        // 2) deadline = koniec drugiego następnego dnia roboczego (18:00)
        // liczymy: następny dzień roboczy (1), drugi następny (2)
        var first = NextWorkingDay(startDate);
        var second = NextWorkingDay(first);

        return second.ToDateTime(OpenTo);
    }

    private DateOnly NextWorkingDay(DateOnly date)
    {
        var d = date.AddDays(1);
        while (!IsWorkingDay(d))
            d = d.AddDays(1);

        return d;
    }
}
