using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LoginEsercizio
{
    public partial class InsertUser : Form
    {
        public InsertUser()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = this.textBoxUsername.Text;
            string password = this.textBoxPassword.Text;

            User u = new User();
            u.nome = username;
            u.password = password;
            u.IDUnivoco = FormPrincipale.utentiRegistrati.Count();
            FormPrincipale.userAttuale = u;
            FormPrincipale.utentiRegistrati.Add(u);

            this.Close();
        }
    }
}
