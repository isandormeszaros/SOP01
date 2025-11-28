using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace _02_Checkout
{
    public class Customer
    {
        public int Id { get; }
        public Customer(int id) { Id = id; }
        public override string ToString() => $"Customer ({Id})";
    }

    public class Program
    {
        private static ConcurrentQueue<Customer> customerQueue = new ConcurrentQueue<Customer>();

        private static volatile bool storeIsRunning = true;
        private static bool cashiersOnJob = false;

        private static ManualResetEvent cashiersOpenEvent = new ManualResetEvent(false);
        private static CancellationTokenSource cts = new CancellationTokenSource();

        private static Task[] cashierTasks = new Task[4];
        private static int nextCustomerId = 0;

        public static void Main(string[] args)
        {
            Console.WriteLine("Store simulation started.");
            Console.WriteLine("Commands: [n] = Open Cashiers, [b] = Close Cashiers, [k] = Quit Store");

            for (int i = 0; i < cashierTasks.Length; i++)
            {
                int cashierId = i + 1;
                cashierTasks[i] = Task.Run(() => CashierWork(cashierId, cts.Token));
            }

            Task customerGenerator = Task.Run(() => CustomerGenerator(cts.Token));

            InputLoop();

            Task.WaitAll(cashierTasks);

            Console.WriteLine("--- THE STORE IS CLOSED. All cashiers have finished. ---");
        }

        private static void InputLoop()
        {
            while (storeIsRunning)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.KeyChar)
                {
                    case 'n':
                        OpenCashiers();
                        break;
                    case 'b':
                        CloseCashiers();
                        break;
                    case 'k':
                        CloseStore();
                        break;
                    default:
                        Console.WriteLine("[CONTROL] Unknown command. Use n/b/k.");
                        break;
                }
            }
        }

        private static void OpenCashiers()
        {
            if (!cashiersOnJob)
            {
                cashiersOnJob = true;
                cashiersOpenEvent.Set();
                Console.WriteLine("[CONTROL] Cashiers opened. Waiting for customers...");
            }
            else
            {
                Console.WriteLine("[CONTROL] Cashiers are already open.");
            }
        }

        private static void CloseCashiers()
        {
            if (cashiersOnJob)
            {
                cashiersOnJob = false;
                cashiersOpenEvent.Reset();
                Console.WriteLine("[CONTROL] Cashiers closed. Serving current customers...");
            }
            else
            {
                Console.WriteLine("[CONTROL] Cashiers are already closed.");
            }
        }

        private static void CloseStore()
        {
            Console.WriteLine("[STORE] CLOSING ('k' command)...");
            storeIsRunning = false;
            cts.Cancel();

            cashiersOpenEvent.Set();

            Console.WriteLine("[STORE] Shutdown signal sent. Cashiers are finishing the queue.");
        }

        private static void CashierWork(int cashierId, CancellationToken token)
        {
            Console.WriteLine($"[CASHIER {cashierId}] Started and waiting to open.");

            while (!token.IsCancellationRequested)
            {
                cashiersOpenEvent.WaitOne();

                if (token.IsCancellationRequested) break;

                if (customerQueue.TryDequeue(out Customer? customer))
                {
                    Console.WriteLine($"[CASHIER {cashierId}] Serving {customer}...");
                    Thread.Sleep(new Random().Next(1000, 2500));
                    Console.WriteLine($"[CASHIER {cashierId}] Finished serving {customer}.");
                }
                else
                {
                    Thread.Sleep(200);
                }
            }

            Console.WriteLine($"[CASHIER {cashierId}] CLOSED DOWN.");
        }

        private static void CustomerGenerator(CancellationToken token)
        {
            Random r = new Random();
            try
            {
                while (!token.IsCancellationRequested)
                {
                    Task.Delay(r.Next(500, 2001), token).Wait(token);
                    Customer c = new Customer(Interlocked.Increment(ref nextCustomerId));
                    customerQueue.Enqueue(c);
                    Console.WriteLine($"[STORE] New customer arrived: {c}. (Queue length: {customerQueue.Count})");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("[STORE] Customer generator stopped (store closed).");
            }
        }
    }
}
