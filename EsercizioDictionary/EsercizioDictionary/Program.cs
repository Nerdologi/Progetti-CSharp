using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EsercizioDictionary
{
    class Person
    {
        String cacca;
        string nome;
        string cognome;
        private string p;
        private string p_2;

        public Person() { }

        public Person(String n, String c) 
        {
            nome = n;
            cognome = c;
        }

        public override string ToString()
        {
            return nome + " " + cognome;
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<int, Person> d = new Dictionary<int, Person>();
            d.Add(1, new Person("Luca", "Perico"));
            d.Add(2, new Person("Silvio", "Messi"));
            d.Add(3, new Person("Lorenzo", "Fenili"));

            Person p = new Person();

            bool find = d.TryGetValue(2, out p);

            Console.Clear();
            if (find)
               Console.WriteLine(d[2]);
            else
                Console.WriteLine("Non presente!");
        }
    }
}
