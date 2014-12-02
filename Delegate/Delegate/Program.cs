<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

delegate void Op(double n1, double n2);

namespace Delegate
{
    class Program
    {
        static void Main(string[] args)
        {
            MathOpClass m = new MathOpClass();
            m.Operazione += new Op(MathOpClass.Add);
            m.Operazione += new Op(MathOpClass.Multiply);
            m.Operazione += new Op(MathOpClass.Sub);
            m.CallOp(2, 3);
        }
    }

    class MathOpClass
    {
        public Op Operazione;
  
        public static void Add(double n1, double n2)
        {
            Console.WriteLine((n1+n2) + " = " + n1 + " + " + n2);
        }

        public static void Multiply(double n1, double n2)
        {
            Console.WriteLine((n1 * n2) + " = " + n1 + " * " + n2);
        }

        public static void Sub(double n1, double n2)
        {
            Console.WriteLine((n1 - n2) + " = " + n1 + " - " + n2);
        }

        public  void CallOp(double n1, double n2)
        {
            Operazione(n1, n2);   
        }
    }
}
=======
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

delegate void Op(double n1, double n2);

namespace Delegate
{
    class Program
    {
        static void Main(string[] args)
        {
            MathOpClass m = new MathOpClass();
            m.Operazione += new Op(MathOpClass.Add);
            m.Operazione += new Op(MathOpClass.Multiply);
            m.Operazione += new Op(MathOpClass.Sub);
            m.CallOp(2, 3);
        }
    }

    class MathOpClass
    {
        public Op Operazione;
  
        public static void Add(double n1, double n2)
        {
            Console.WriteLine((n1+n2) + " = " + n1 + " + " + n2);
        }

        public static void Multiply(double n1, double n2)
        {
            Console.WriteLine((n1 * n2) + " = " + n1 + " * " + n2);
        }

        public static void Sub(double n1, double n2)
        {
            Console.WriteLine((n1 - n2) + " = " + n1 + " - " + n2);
        }

        public  void CallOp(double n1, double n2)
        {
            Operazione(n1, n2);   
        }
    }
}
>>>>>>> origin/master
