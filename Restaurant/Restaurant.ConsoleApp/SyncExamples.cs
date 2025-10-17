using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public static class SyncExamples
{
    // Демонстрації примітивів синхронізації. Кожна демонстрація запускається коротко та виводить результат.
    public static void RunAll()
    {
        Console.WriteLine("\n--- Sync primitives приклади ---");
        LockExample();
        SemaphoreSlimExample().Wait();
        AutoResetEventExample();
        ManualResetEventSlimExample();
        ReaderWriterLockSlimExample();
        MonitorWaitPulseExample();
        MutexExample();
        BarrierExample();
        CountdownEventExample();
        Console.WriteLine("--- Кінець sync examples ---\n");
    }

    private static void LockExample()
    {
        var locker = new object();
        int counter = 0;
        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < 1000; j++)
                {
                    lock (locker)
                    {
                        counter++;
                    }
                }
            }));
        }
        Task.WaitAll(tasks.ToArray());
        Console.WriteLine($"Підрахунок LockExample = {counter} (очікується 10000)");
    }

    private static async Task SemaphoreSlimExample()
    {
        var semaphore = new SemaphoreSlim(3); // одночасно 3
        int running = 0;
        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                await semaphore.WaitAsync();
                try
                {
                    Interlocked.Increment(ref running);
                    Console.WriteLine($"Запуск SemaphoreSlimExample={running}");
                    await Task.Delay(50);
                }
                finally
                {
                    Interlocked.Decrement(ref running);
                    semaphore.Release();
                }
            }));
        }
        await Task.WhenAll(tasks);
        semaphore.Dispose();
        Console.WriteLine("Виконано SemaphoreSlimExample");
    }

    private static void AutoResetEventExample()
    {
        using var are = new AutoResetEvent(false);
        int data = 0;
        Task.Run(() =>
        {
            Thread.Sleep(100);
            data = 42;
            are.Set(); // сигналізує одному чекателю
        });

        are.WaitOne();
        Console.WriteLine($"AutoResetEventExample data={data}");
    }

    private static void ManualResetEventSlimExample()
    {
        using var mre = new ManualResetEventSlim(false);
        int sum = 0;
        var t = Task.Run(() =>
        {
            for (int i = 1; i <= 5; i++) sum += i;
            mre.Set();
        });

        mre.Wait();
        Console.WriteLine($"ManualResetEventSlimExample сума={sum}");
    }

    private static void ReaderWriterLockSlimExample()
    {
        var rw = new ReaderWriterLockSlim();
        int value = 0;
        var readers = new List<Task>();
        for (int i = 0; i < 5; i++)
        {
            readers.Add(Task.Run(() =>
            {
                rw.EnterReadLock();
                try { var v = value; Console.WriteLine($"Reader read {v}"); }
                finally { rw.ExitReadLock(); }
            }));
        }

        var writer = Task.Run(() =>
        {
            rw.EnterWriteLock();
            try { value = 100; Console.WriteLine("Writer set value=100"); }
            finally { rw.ExitWriteLock(); }
        });

        Task.WaitAll(readers.ToArray());
        writer.Wait();
        rw.Dispose();
    }

    private static void MonitorWaitPulseExample()
    {
        object lockObj = new object();
        bool ready = false;
        var t = Task.Run(() =>
        {
            lock (lockObj)
            {
                while (!ready)
                    Monitor.Wait(lockObj);
                Console.WriteLine("MonitorWaitPulseExample повернув сигнал");
            }
        });

        Task.Delay(100).Wait();
        lock (lockObj)
        {
            ready = true;
            Monitor.Pulse(lockObj);
        }
        t.Wait();
    }

    private static void MutexExample()
    {
        var name = "Global_" + Guid.NewGuid().ToString();
        using var m = new Mutex(false, name);
        // локальний приклад: захоплюємо і звільняємо
        m.WaitOne();
        Console.WriteLine("MutexExample звільнено локально");
        m.ReleaseMutex();
    }

    private static void BarrierExample()
    {
        int participants = 3;
        using var barrier = new Barrier(participants, b => Console.WriteLine($"Barrier post-phase: {b.CurrentPhaseNumber}"));
        var tasks = new List<Task>();
        for (int i = 0; i < participants; i++)
        {
            int idx = i;
            tasks.Add(Task.Run(() =>
            {
                Console.WriteLine($"Учасник {idx} досяг бар'єра");
                barrier.SignalAndWait();
                Console.WriteLine($"Учасник {idx} після бар'єра");
            }));
        }
        Task.WaitAll(tasks.ToArray());
    }

    private static void CountdownEventExample()
    {
        using var cde = new CountdownEvent(3);
        for (int i = 0; i < 3; i++)
        {
            Task.Run(() =>
            {
                Task.Delay(50).Wait();
                Console.WriteLine("CountdownEventExample робота завершена");
                cde.Signal();
            });
        }
        cde.Wait();
        Console.WriteLine("CountdownEventExample всі сигналізували");
    }
}
