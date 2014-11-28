using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EsercizioForm
{
    public partial class Form1 : Form
    {
        public TextBox t;
        public RichTextBox r;

        public Form1()
        {
            InitializeComponent();
            t = new TextBox();
            t.Multiline = true;
            t.ScrollBars = ScrollBars.Vertical;
            t.Height = 100;
            t.Width = 200;
            t.Location = new Point(10, 10);
            this.Controls.Add(t);

            r = new RichTextBox();
            r.Location = new Point(10, 120);
            r.Height = 100;
            r.Width = 200;
            r.TextChanged += new EventHandler(this.r_TextChange);
            this.Controls.Add(r);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Hai aperto la MB", "", MessageBoxButtons.OKCancel);
            if (res.ToString() == "OK")
                t.Text = "Hai premuto il tasto ok.";
            else
                t.Text = "Hai abortito";
        }

        public void r_TextChange(object sender, EventArgs e)
        {
            t.Text = r.Text;
        }
    }
}
