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
            Sailboat s = new Sailboat();
            s.IDUnivoco = FormPrincipale.barcheInserite.Count();
            s.marca = this.textBoxMarca.Text;
            s.nome = this.textBoxNome.Text;
            s.lunghezza = float.Parse(this.textBoxLunghezza.Text);
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
