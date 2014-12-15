using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ThreadExercise
{
    class Program
    {
        
        static void Main(string[] args)
        {
            CT ct = new CT();

            ParameterizedThreadStart ts1 = new ParameterizedThreadStart(ct.ThreadCode);
            Thread t1 = new Thread(ts1);
            t1.Start(t1.ManagedThreadId);

            ParameterizedThreadStart ts2 = new ParameterizedThreadStart(ct.ThreadCode);
            Thread t2 = new Thread(ts2);
            t2.Start(t2.ManagedThreadId);
        }
    }

    public class CT
    {
        public void ThreadCode(object pid)
        {
            Random random = new Random();
            int rand;
            while (true)
            {
                rand = random.Next(1000, 5000);
                Console.WriteLine("Sono eseguito dal thread " +(int)pid+" e dormirò per " + (rand / 1000).ToString() + " secondi");
                Thread.Sleep(rand);
            }
        }
    }
}
