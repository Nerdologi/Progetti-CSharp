using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication2
{
    class Persona
    {
        String nome;
        String cognome;

        public override string ToString()
        {
            return nome;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<String> gente = new List<String>();
            gente.Add("Luca");
            gente.Add("Silvio");
            gente.Add("Lorenzo");
            gente.Add("Hamza");

            gente.Sort();

            int index = gente.BinarySearch("Luca");
            Console.Clear();
            Console.WriteLine(index);

            String[] persone = gente.ToArray();

            foreach (String p in persone)
                Console.WriteLine(p.ToString());

        }
    }
}
