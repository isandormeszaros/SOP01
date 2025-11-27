namespace Soap04TermeloFogyaszto
{
    internal class Product
    {
        private static int NextId = 0;
        private readonly int id;

        public Product()
        {
            // Szálbiztos módon növeli az ID-t, ha több szál dolgozik
            id = Interlocked.Increment(ref NextId);
        }

        public override string ToString()
        {
            return $"Termék ID: {id}";
        }
    }
}
