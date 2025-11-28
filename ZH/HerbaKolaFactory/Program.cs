namespace ColaFactory
{
    internal class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Controller.CreateProducers(1, 500, (ColaType)i, (BottleSize)j);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Controller.CreateConsumers(1, (ColaType)i, (BottleSize)j);
                }
            }

            Controller.StartProcess();

            Console.ReadLine();
        }
    }
}
