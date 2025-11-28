namespace _01_EveryThingPhone
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Supervisor.CreateProducers(4, 40, Brand.X1, Spec.PRO);
            Supervisor.CreateProducers(4, 40, Brand.X1, Spec.PRO);
            Supervisor.CreateConsumers(3, Brand.X1, Spec.PRO);
            Supervisor.CreateConsumers(3, Brand.X1, Spec.PRO);
            Supervisor.CreateConsumers(3, Brand.X1, Spec.PRO);

            Supervisor.StartProcess();
            Console.WriteLine("*** ALL THREADS STARTED ***");
            foreach (Thread t in Supervisor.producerConsumerThreads)
            {
                t.Join();
            }

            Supervisor.WriteOutOneStack();

            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();
        }
    }
}
