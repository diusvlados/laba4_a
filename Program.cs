using System;

using MyTime = (int hour, int min, int sec);

class Program
{
    // Нормалізація: приводимо до реального часу доби
    static MyTime Normalize(MyTime t)
    {
        return FromSecSinceMidnight(ToSecSinceMidnight(t));
    }

    // Рядкове подання: H:mm:ss
    static string MyTimeToString(MyTime t)
    {
        return $"{t.hour}:{t.min:D2}:{t.sec:D2}";
    }

    // Кількість секунд від початку доби (00:00:00)
    static int ToSecSinceMidnight(MyTime t)
    {
        return t.hour * 3600 + t.min * 60 + t.sec;
    }

    // З секунд від початку доби — назад у кортеж.(метод що уже був даний)
    static MyTime FromSecSinceMidnight(int t)
    {
        int secPerDay = 60 * 60 * 24;
        t %= secPerDay;
        if (t < 0) t += secPerDay; // від'ємні секунди → кінець доби
        int h = t / 3600;
        int m = (t / 60) % 60;
        int s = t % 60;
        return (h, m, s);
    }

    // Додавання 1 секунди / 1 хвилини / 1 години
    static MyTime AddOneSecond(MyTime t)
    {
        return AddSeconds(t, 1);
    }
    static MyTime AddOneMinute(MyTime t) => AddSeconds(t, 60);
    static MyTime AddOneHour(MyTime t) => AddSeconds(t, 3600);


    // Додавання довільної кількості секунд -->  s може бути від'ємним або > 86400 — Normalize впорається
    static MyTime AddSeconds(MyTime t, int s)
    {
        return FromSecSinceMidnight(ToSecSinceMidnight(t) + s);
    }

    // Різниця між двома моментами в секундах
    static int Difference(MyTime t1, MyTime t2)
    {
        return ToSecSinceMidnight(t1) - ToSecSinceMidnight(t2);
    }

    // Розклад дзвінків університету (6 пар)
    static string WhatLesson(MyTime t)
    {
        int s = ToSecSinceMidnight(Normalize(t));

        // Допоміжна функція: час у секундах з рядка "H:MM"
        int T(int h, int m) => h * 3600 + m * 60;

        int[] start = { T(8, 00), T(9, 40), T(11, 20), T(13, 00), T(14, 40), T(16, 20) };
        int[] end = { T(9, 20), T(11, 00), T(12, 40), T(14, 20), T(16, 00), T(17, 40) };

        string[] names = { "1-а", "2-а", "3-я", "4-а", "5-а", "6-а" };
        string[] between = {
            "перерва між 1-ю та 2-ю парами",
            "перерва між 2-ю та 3-ю парами",
            "перерва між 3-ю та 4-ю парами",
            "перерва між 4-ю та 5-ю парами",
            "перерва між 5-ю та 6-ю парами"
        };

        if (s < start[0]) return "пари ще не почалися";
        if (s > end[5]) return "пари вже скінчилися";

        for (int i = 0; i < 6; i++)
        {
            if (s >= start[i] && s <= end[i])
                return $"{names[i]} пара";

            if (i < 5 && s > end[i] && s < start[i + 1])
                return between[i];
        }

        return "пари вже скінчилися";
    }

    // Зчитуємо одне ціле число з перевіркою вводу
    static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value))
                return value;
            Console.WriteLine("  Помилка: введіть ціле число.");
        }
    }

    // Ручне введення кортежу MyTime --> будь які знчаення
    // — Normalize потім виправить
    static MyTime ReadManual(string label)
    {
        Console.WriteLine($"\n  Введіть {label}:");
        int h = ReadInt("    години  : ");
        int m = ReadInt("    хвилини : ");
        int s = ReadInt("    секунди : ");
        return (h, m, s);
    }

    // Випадкове заповнення 
    static MyTime ReadRandom()
    {
        Random rnd = new Random();
        int h = rnd.Next(-5, 30);   // > меж 0–23
        int m = rnd.Next(-10, 80);  // > меж 0–59
        int s = rnd.Next(-10, 150); // > меж 0–59
        Console.WriteLine($"  Згенеровано (до нормалізації): ({h}, {m}, {s})");
        return (h, m, s);
    }

    //показ всіх моїх операцій  ++ вибір скільки секунд додати
    static void ShowAll(MyTime t)
    {
        MyTime norm = Normalize(t);

        Console.WriteLine($"\n  Введено (сирі дані)  : ({t.hour}, {t.min}, {t.sec})");
        Console.WriteLine($"  Після нормалізації   : {MyTimeToString(norm)}");
        Console.WriteLine($"  Секунд від опівночі  : {ToSecSinceMidnight(norm)}");
        Console.WriteLine($"  + 1 секунда          : {MyTimeToString(AddOneSecond(norm))}");
        Console.WriteLine($"  + 1 хвилина          : {MyTimeToString(AddOneMinute(norm))}");
        Console.WriteLine($"  + 1 година           : {MyTimeToString(AddOneHour(norm))}");
        int sec = ReadInt("\n  Введіть кількість секунд для додавання (може бути від'ємною): ");
        Console.WriteLine($"  Час + ({sec}) сек : {MyTimeToString(AddSeconds(norm, sec))}");
        Console.WriteLine($"  Що зараз за розкладом: {WhatLesson(norm)}");
    }

    // MAIN
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("------------------------------------");
        Console.WriteLine("      Робота з часом (MyTime)       ");
        Console.WriteLine("------------------------------------");
        Console.WriteLine("   1 — ввести вручну                ");
        Console.WriteLine("   2 — згенерувати випадково        ");
        Console.WriteLine("------------------------------------");
        Console.Write("Ваш вибір: ");

        MyTime t1, t2;

        switch (Console.ReadLine())
        {
            case "1":
                t1 = ReadManual("перший час");
                t2 = ReadManual("другий час (для різниці)");
                break;
            case "2":
                Console.WriteLine("\n  Перший час:");
                t1 = ReadRandom();
                Console.WriteLine("  Другий час:");
                t2 = ReadRandom();
                break;
            default:
                Console.WriteLine("Невірний вибір. Використовується випадкове заповнення.");
                Console.WriteLine("\n  Перший час:");
                t1 = ReadRandom();
                Console.WriteLine("  Другий час:");
                t2 = ReadRandom();
                break;
        }

        //  Результати для двох часів
        Console.WriteLine("  ПЕРШИЙ ЧАС");
        ShowAll(t1);
        Console.WriteLine("  ДРУГИЙ ЧАС");
        ShowAll(t2);

        //Різниця між двома часами 
        Console.WriteLine("  РІЗНИЦЯ МІЖ ЧАСАМИ");
        int diff = Difference(Normalize(t1), Normalize(t2));
        Console.WriteLine($"  t1 - t2 = {diff} сек");
        if (diff > 0) Console.WriteLine("  -> перший час ПІЗНІШЕ за другий");
        else if (diff < 0) Console.WriteLine("  -> перший час РАНІШЕ за другий");
        else Console.WriteLine("  -> часи ОДНАКОВІ");
    }
}