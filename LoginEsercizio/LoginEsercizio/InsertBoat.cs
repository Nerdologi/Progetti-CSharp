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
    public partial class InsertBoat : Form
    {
        public InsertBoat()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //controllo tipo dati
            float n_lunghezza;
            bool isNumeric = float.TryParse(this.textBoxLunghezza.Text, out n_lunghezza);
            if (!isNumeric)
            {
                MessageBox.Show("La lunghezza deve essere un numero!");
                return;
            }

            float n_prezzo;
            isNumeric = float.TryParse(this.textBoxPrezzo.Text, out n_prezzo);
            if (!isNumeric)
            {
                MessageBox.Show("Il prezzo deve essere un numero!");
                return;
            }

            Sailboat s = new Sailboat();
            s.IDUnivoco = FormPrincipale.barcheInserite.Count();
            s.marca = this.textBoxMarca.Text;
            s.nome = this.textBoxNome.Text;
            s.lunghezza = n_lunghezza;
            s.prezzo = float.Parse(this.textBoxPrezzo.Text);

            FormPrincipale.barcheInserite.Add(s);

            UserBarca ub = new UserBarca();
            ub.IDUnivocoUser = FormPrincipale.userAttuale.IDUnivoco;
            ub.IDUnivocoBarca = s.IDUnivoco;
            SessionLog.getInstance().scrivi_su_log(ub);

            this.Close();
        }
    }
}
