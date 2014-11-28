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
    public partial class FormPrincipale : Form
    {
        public static User userAttuale = null;
        public static List<User> utentiRegistrati = null;
        public static List<Sailboat> barcheInserite = null;

        public FormPrincipale()
        {
            InitializeComponent();
            utentiRegistrati = new List<User>();
            barcheInserite = new List<Sailboat>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form f2 = new InsertUser();
            f2.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            InsertBoat b1 = new InsertBoat();
            b1.ShowDialog();
        }

        private void FormPrincipale_Activated(object sender, EventArgs e)
        {
            if (userAttuale != null)
            {
                this.labelLoggatoUser.Text = FormPrincipale.userAttuale.nome;
                this.buttonCreaBarca.Enabled = true;
            }
            else
            {
                this.labelLoggatoUser.Text = "NESSUNO";
                this.buttonCreaBarca.Enabled = false;
            }
            this.labelNumeroUtenti.Text = utentiRegistrati.Count().ToString();
            this.labelNumeroBarche.Text = barcheInserite.Count().ToString();

            //aggiorno listview barca
            if (barcheInserite.Count() != 0)
                this.listViewRiepilogoInserimento.Items.Add(barcheInserite.Last().ToString());
        }
    }

    public class Sailboat
    {
        string _marca;
        string _nome;
        float _lunghezza;
        float _prezzo;
        private int _IDUnivoco;

        public string marca
        {
            get { return _marca; }
            set { _marca = value; }
        }

        public string nome
        {
            get { return _nome; }
            set { _nome = value; }
        }

        public float lunghezza
        {
            get { return _lunghezza; }
            set { _lunghezza = value; }
        }

        public float prezzo
        {
            get { return _prezzo; }
            set { _prezzo = value; }
        }

        public int IDUnivoco
        {
            get { return _IDUnivoco; }
            set { _IDUnivoco = value; }
        }

        public override string ToString()
        {
            /*string ret = "Nome: " + this.nome + "\n";
            ret += "Marca: " + this.marca + "\n";
            ret += "Lunghezza: " + this.lunghezza + "m\n";
            ret += "Prezzo: " + this.prezzo.ToString() + "€";*/

            return this.nome;
        }

    }

    public class User
    {
        string _nome;
        string _password;
        int _IDUnivoco;

        public string nome
        {
            get { return _nome; }
            set { _nome = value; }
        }

        public string password
        {
            get { return _password; }
            set { _password = value; }
        }

        public int IDUnivoco
        {
            get { return _IDUnivoco; }
            set { _IDUnivoco = value; }
        }
    }

    public class SessionLog
    {
        private static SessionLog istanza;
        List<UserBarca> logs = new List<UserBarca>();

        private SessionLog()
        { }

        public static SessionLog getInstance()
        {
            if (istanza == null)
                istanza = new SessionLog();
            return istanza;
        }

        public void scrivi_su_log(UserBarca ub)
        {
            logs.Add(ub);
        }

        public UserBarca leggi_da_log(int i)
        {
            return logs[i];
        }
    }

    public struct UserBarca
    {
        public int IDUnivocoUser;
        public int IDUnivocoBarca;
    }
}
