namespace _01_SomeThingLaptop
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Supervisor.CreateProducers(1, 60, Brand.A, 10);
            Supervisor.CreateProducers(1, 60, Brand.A, 20);
            Supervisor.CreateProducers(1, 60, Brand.F, 1);
            Supervisor.CreateConsumers(2, Brand.A, 10);
            Supervisor.CreateConsumers(2, Brand.A, 20);
            Supervisor.CreateConsumers(2, Brand.F, 1);

            Supervisor.StartProcess();

            foreach (var t in Supervisor.producerConsumerThreads)
            {
                t.Join();
            }
            Console.WriteLine("\nPress Enter to write out one stack of items!");
            Console.ReadLine();
            Console.Clear();
            Console.WriteLine("One stack of items: ");
            Supervisor.WriteOutOneStack();

            Console.ReadLine();
        }
    }
}
