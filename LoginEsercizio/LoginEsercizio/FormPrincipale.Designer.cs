namespace LoginEsercizio
{
    partial class FormPrincipale
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Liberare le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Barca6");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Barca7");
            this.button1 = new System.Windows.Forms.Button();
            this.buttonCreaBarca = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.labelLoggatoUser = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelNumeroBarche = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelNumeroUtenti = new System.Windows.Forms.Label();
            this.listViewRiepilogoInserimento = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Logon";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonCreaBarca
            // 
            this.buttonCreaBarca.Location = new System.Drawing.Point(13, 42);
            this.buttonCreaBarca.Name = "buttonCreaBarca";
            this.buttonCreaBarca.Size = new System.Drawing.Size(75, 23);
            this.buttonCreaBarca.TabIndex = 1;
            this.buttonCreaBarca.Text = "Crea barca";
            this.buttonCreaBarca.UseVisualStyleBackColor = true;
            this.buttonCreaBarca.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(170, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Utente loggato:";
            // 
            // labelLoggatoUser
            // 
            this.labelLoggatoUser.AutoSize = true;
            this.labelLoggatoUser.Location = new System.Drawing.Point(170, 42);
            this.labelLoggatoUser.Name = "labelLoggatoUser";
            this.labelLoggatoUser.Size = new System.Drawing.Size(35, 13);
            this.labelLoggatoUser.TabIndex = 3;
            this.labelLoggatoUser.Text = "label2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(170, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Numero barche:";
            // 
            // labelNumeroBarche
            // 
            this.labelNumeroBarche.AutoSize = true;
            this.labelNumeroBarche.Location = new System.Drawing.Point(259, 75);
            this.labelNumeroBarche.Name = "labelNumeroBarche";
            this.labelNumeroBarche.Size = new System.Drawing.Size(13, 13);
            this.labelNumeroBarche.TabIndex = 5;
            this.labelNumeroBarche.Text = "1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(170, 102);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Numero utenti:";
            // 
            // labelNumeroUtenti
            // 
            this.labelNumeroUtenti.AutoSize = true;
            this.labelNumeroUtenti.Location = new System.Drawing.Point(259, 102);
            this.labelNumeroUtenti.Name = "labelNumeroUtenti";
            this.labelNumeroUtenti.Size = new System.Drawing.Size(13, 13);
            this.labelNumeroUtenti.TabIndex = 7;
            this.labelNumeroUtenti.Text = "1";
            // 
            // listViewRiepilogoInserimento
            // 
            this.listViewRiepilogoInserimento.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewRiepilogoInserimento.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem3,
            listViewItem4});
            this.listViewRiepilogoInserimento.Location = new System.Drawing.Point(13, 85);
            this.listViewRiepilogoInserimento.Name = "listViewRiepilogoInserimento";
            this.listViewRiepilogoInserimento.Size = new System.Drawing.Size(151, 164);
            this.listViewRiepilogoInserimento.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewRiepilogoInserimento.TabIndex = 8;
            this.listViewRiepilogoInserimento.UseCompatibleStateImageBehavior = false;
            // 
            // FormPrincipale
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.listViewRiepilogoInserimento);
            this.Controls.Add(this.labelNumeroUtenti);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelNumeroBarche);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelLoggatoUser);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCreaBarca);
            this.Controls.Add(this.button1);
            this.Name = "FormPrincipale";
            this.Text = "Menù di inserimento";
            this.Activated += new System.EventHandler(this.FormPrincipale_Activated);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonCreaBarca;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelLoggatoUser;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelNumeroBarche;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelNumeroUtenti;
        private System.Windows.Forms.ListView listViewRiepilogoInserimento;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}

