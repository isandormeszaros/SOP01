//using System.Linq;

//namespace Soap05ThreadPool
//{
//    internal class Program
//    {
//        static List<int> RandomList(int cont, int min, int max)
//        {

//            Random rnd = new Random();
//            List<int> list = new List<int>();

//            for (int i = min; i <= max; i++) 
//            { 
//                list.Add(rnd.Next(minValue, maxValue +1));
//            }
//            return list;
//        }

//        static (int, int) FindMaxAndCount(List<int> List)
//        {
//            int max = list.Max();
//            int maxCount = List.Count(int x => x == max);
//            return (max, maxCount);
//        }

//        static (int, int) FindMinAndCount(List<int> List)
//        {
//            int min = List.Min();
//            int minCount = List.Count(int x => x == min);
//            return (min, minCount)
//        }

//        static void Main(string[] args)
//        {
//            List<int> randomNumbers = RandomList(1000, 0, 1000);
//            (int max, int maxCount) maxResult = (int.MinValue, -1);
//            (int min, int momCount) minResult = (int.MaxValue, -1);

//            CountdownEvent countdownEvent = new CountdownEvent(2);

//            Thread.QueueUserWorkItem(object ? =>
//            {
//                try
//                {
//                    maxResult = FindMaxAndCount(randomNumbers);
//                }
//                catch (Exception)
//                {

//                    throw;
//                }

//                finally 
//                { 
//                    countdownEvent.Signal(); //initialCount--;
//                }
//            });
//        }
//    }
//}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Soap05ThreadPool
{
    internal class Program
    {
        static List<int> RandomList(int count, int min, int max)
        {
            Random rnd = new Random();
            List<int> list = new List<int>();

            // fixed loop and variable names
            for (int i = 0; i < count; i++)
            {
                list.Add(rnd.Next(min, max + 1));
            }
            return list;
        }

        static (int, int) FindMaxAndCount(List<int> list)
        {
            int max = list.Max();
            int maxCount = list.Count(x => x == max);
            return (max, maxCount);
        }

        static (int, int) FindMinAndCount(List<int> list)
        {
            int min = list.Min();
            int minCount = list.Count(x => x == min);
            return (min, minCount);
        }

        static void Main(string[] args)
        {
            List<int> randomNumbers = RandomList(1000, 0, 1000);

            (int max, int maxCount) maxResult = (int.MinValue, -1);
            (int min, int minCount) minResult = (int.MaxValue, -1);

            CountdownEvent countdownEvent = new CountdownEvent(2);

            // fixed parameter syntax and variable name
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    maxResult = FindMaxAndCount(randomNumbers);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    countdownEvent.Signal(); // signals completion
                }
            });

            // added missing thread for min calculation
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    minResult = FindMinAndCount(randomNumbers);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    countdownEvent.Signal();
                }
            });

            // wait for both threads
            countdownEvent.Wait();

            Console.WriteLine("Feladat befejezve");
            Console.WriteLine($"Max value: {maxResult.max}, appears {maxResult.maxCount} times");
            Console.WriteLine($"Min value: {minResult.min}, appears {minResult.minCount} times");
        }
    }
}
