namespace QualifyingSimulation
{
    internal class Program
    {
        static async Task Main()
        {
            Random rnd = new Random();
            HashSet<int> driverNumbers = new HashSet<int>();

            while (driverNumbers.Count < 10)
            {
                driverNumbers.Add(rnd.Next(1, 100));
            }

            List<Driver> drivers = driverNumbers.Select(n => new Driver(n)).ToList();

            QualifyingController controller = new QualifyingController(drivers);

            await controller.StartAsync();
            controller.PrintResults();
        }
    }
}
