class MainClass
{
    static double timer = 0;
    static List<UInt64> Output = new List<UInt64>();
    static UInt64 ThreadsMax = 0;
    static UInt64 Min = 1, Max = 1;
    static List<Threader> th = new List<Threader>();
    static bool End;

    static void Main()
    {
        Console.WriteLine("Введите максимальное и минимальные значения и количество потоков:");
        ReadConsole();
        th.Add(new Threader(Min, Max / ThreadsMax));

        for (UInt64 i = 1; i < ThreadsMax; i++)
            th.Add(new Threader(Max / ThreadsMax * i + 1, Max / ThreadsMax * (i + 1)));
        Time();
        ConsoleUpdate().GetAwaiter().GetResult(); // Ждем завершения обновления консоли
    }

    static void ReadConsole()
    {
        try
        {
            Min = Convert.ToUInt64(Console.ReadLine());
            Max = Convert.ToUInt64(Console.ReadLine());
            ThreadsMax = Convert.ToUInt64(Console.ReadLine());

            if (ThreadsMax == 0)
                throw new Exception("Количество потоков должно быть больше нуля.");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ForegroundColor = ConsoleColor.White;
            Thread.Sleep(2000);
            Main();
        }

        if (Min == UInt64.MaxValue || Max == UInt64.MinValue || ThreadsMax > 9)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Невозможно! Слишком маленькие(большие) значения!");
            Console.ForegroundColor = ConsoleColor.White;
            Main();
        }
    }

    static async Task ConsoleUpdate(bool last = false)
    {
        Console.Clear();
        Console.WriteLine(Min + "  " + Max);
        End = false;
        Output.Clear();

        foreach (var x in th)
        {
            End = true;
            Console.WriteLine("Поток вычисляет: " + x.State);
            Output.AddRange(x.OutputTH);
            if (x.State)
                End = false;
        }

        Output.Sort();
        Output.ForEach(x => Console.Write(x + "\t"));

        if (End)
        {
            Console.WriteLine("\nВычисления завершены!\nВремя: " + timer);
            return;
        }

        await Task.Delay(TimeSpan.FromSeconds(10));
        await ConsoleUpdate(); // Обновляем консоль
    }
    static async Task Time()
    {
        foreach (var x in th)
        {
            End = true;
            if (x.State)
                End = false;
        }
        await Task.Delay(10);
        timer += 0.01;
        if(!End)
        {
            Time();
        }
    }
}

class Threader
{
    Thread thread;
    UInt64 startDot = 0, endDot = 0;
    public bool State = true;
    public List<ulong> OutputTH = new List<ulong>();

    public Threader(UInt64 _startDot, UInt64 _endDot)
    {
        startDot = _startDot;
        endDot = _endDot;
        thread = new Thread(Threads);
        thread.Start();
    }

    public void Threads()
    {
        for (UInt64 i = startDot; i <= endDot; i++)
        {
            HaveFiveDivisors(i);
        }
        State = false;
        Console.WriteLine("end");
    }

    void HaveFiveDivisors(UInt64 number)
    {
        int DivisorCount = 0;

        // Проверяем делители от 1 до sqrt(number)
        for (UInt64 i = 1; i * i <= number; i++)
        {
            if (number % i == 0)
            {
                DivisorCount++;
                if (i != number / i)
                    DivisorCount++;
            }

            if (DivisorCount > 5) return; // Если больше 5 делителей, выходим
        }

        if (DivisorCount == 5)
            OutputTH.Add(number);
    }
}
