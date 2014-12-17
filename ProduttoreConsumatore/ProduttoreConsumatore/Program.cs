using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ProduttoreConsumatore
{
    class Program
    {
        public static int MAX_LENGTH = 100;
        static void Main(string[] args)
        {
            Store s = new Store();
            /*Thread M = new Thread(new Produttore(s).Scrive);
            Thread N = new Thread(new Consumatore(s).Legge);
            N.Start();
            M.Start();*/

            List<Thread> Ms = new List<Thread>(10);
            List<Thread> Ns = new List<Thread>(10);

            for (int i = 0; i < 10; ++i)
                Ms.Add(new Thread(new Produttore(s).Scrive));

            for (int i = 0; i < 10; ++i)
                Ns.Add(new Thread(new Consumatore(s).Legge));

            for (int i = 0; i < Ns.Count; ++i)
            {
                Ms[i].Start();
                Ns[i].Start();
            }
        }
    }

    public class Produttore
    {
        private Store _s;

        public Produttore(Store s) 
        {
            _s = s;
        }
        
        public void Scrive()
        {
            Random random = new Random();
            int rand;
            while (true)
            {
                lock (_s.lista)
                {
                    if (_s.lista.Count > Program.MAX_LENGTH)
                    {
                        Console.WriteLine("Produttore in attesa...");
                        Monitor.Wait(_s.lista);
                    }

                    _s.WriteTo();
                    Monitor.Pulse(_s.lista);
                }

                rand = random.Next(500, 1000);
                Thread.Sleep(rand);
            }
        }
    }

    public class Consumatore
    {
        private Store _s;

        public Consumatore(Store s)
        {
            _s = s;
        }

        public void Legge()
        {
            Random random = new Random();
            int rand;
            while (true)
            {
                lock (_s.lista)
                {
                    if (_s.lista.Count < 1)
                    {
                        Console.WriteLine("Consumatore in attesa...");
                        Monitor.Wait(_s.lista);
                    }

                    _s.ReadFrom();
                    Monitor.Pulse(_s.lista);
                }
                rand = random.Next(500, 1000);
                Thread.Sleep(rand);
            }
        }
    }

    public class Store
    {
        public List<string> lista = new List<string>(Program.MAX_LENGTH);

        public void WriteTo()
        {
            lista.Add("Elemento n° " + (lista.Count+1));
            Console.WriteLine("Aggiunto un elemento in posizione: " + lista.Count);
        }

        public void ReadFrom()
        {
            Console.WriteLine("Tolto elemento '"+lista[lista.Count-1]+"'");
            lista.RemoveAt(lista.Count-1);
        }

    }
}
