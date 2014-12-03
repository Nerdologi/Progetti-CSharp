using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EventCounter;

namespace EventCounter
{
    public partial class Form1 : Form
    {
        int _X;
        public static int reachableNum;
        Counter c = null;
        public Form1()
        {
            InitializeComponent();
            c = new Counter();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _X = int.Parse(this.textBox1.Text);
            if (_X > 100000)
            {
                MessageBox.Show("Il numero deve essere minore di 100.000");
                return;
            }
            Random random = new Random();
            int Y = random.Next(0, 10000);
            reachableNum = _X + Y;
            this.labelValoreCount.Text = "" + (reachableNum / 100) + " secondi!";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            c.CountTo();
        }

        public static void oCounter_NumberReached(object sender, NumberReachedHandlerEventArgs e)
        {
            MessageBox.Show(e.GetMessage + " " + reachableNum);
        }

        public static void oCounter_NumberReached2(object sender, NumberReachedHandlerEventArgs e)
        {
            MessageBox.Show(e.GetMessage + " " + reachableNum);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            c.NumberReached -= new Counter.NumberReachedHandler(Form1.oCounter_NumberReached2);
        }
    }

    public class Counter
    {
        public delegate void NumberReachedHandler(object sender, NumberReachedHandlerEventArgs e);
        public event NumberReachedHandler NumberReached;

        public Counter()
        {
            NumberReached += new NumberReachedHandler(Form1.oCounter_NumberReached2) + new NumberReachedHandler(Form1.oCounter_NumberReached);
        }

        public void CountTo()
        {
            for (int i = 0; i < Form1.reachableNum; ++i)
            {
                System.Threading.Thread.Sleep(10);
            }
            NumberReached(this, new NumberReachedHandlerEventArgs("Numero raggiunto!"));
        }
    }

    public class NumberReachedHandlerEventArgs : EventArgs
    {
        protected string _message;
        public NumberReachedHandlerEventArgs(string message)
        {
            _message = message;
        }

        public string GetMessage
        {
            get { return _message; }
        }
    }
}
