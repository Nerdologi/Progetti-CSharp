using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;

namespace Esame
{
    public partial class FormGraph : Form
    {
        private ZedGraphControl zgc;
        private GraphPane myPane;
        private int numFinestra;
        private int numColore;
        private LineItem myCurve;
        private List<Color> lc = new List<Color>(5);

        public FormGraph()
        {
            InitializeComponent();
        }

        // Costruisce grafico
        public void InitGraph(string _title, string _XAxisTitle, string _YAxisTitle)
        {
            lc.Add(Color.Red);
            lc.Add(Color.Blue);
            lc.Add(Color.GreenYellow);
            lc.Add(Color.HotPink);
            lc.Add(Color.Black);
            // Ottengo riferimento al pannello
            zgc = zedGraphControl1;
            myPane = zedGraphControl1.GraphPane;
            numFinestra = 0;
            numColore++;

            // Cancello eventuali linee presenti sul grafico
            myPane.CurveList.Clear();

            // Setto il titolo del grafico e le etichette degli assi cartesiani
            myPane.Title.Text = _title;
            myPane.XAxis.Title.Text = _XAxisTitle;
            myPane.YAxis.Title.Text = _YAxisTitle;

            myPane.XAxis.Scale.Min = -40;
            myPane.XAxis.Scale.MinorStep = 100;
            myPane.XAxis.Scale.MajorStep = 500;
            myPane.XAxis.Scale.MinorUnit = DateUnit.Millisecond;
            myPane.XAxis.Scale.MajorUnit = DateUnit.Millisecond;
        }

        /* Accetta in ingresso due parametri:
         * - data, sono i punti da rappresentare sul grafico
         * - category, che può assumere i valori "modacc" , "modgiro", "theta", "thetaNoDiscontinuita", "yaw", "yawNoDiscontinuita",
         * "pitch", "pitchNoDiscontinuita", "roll", "rollNoDiscontinuita", "deviazioneStandard"
         * Il secondo parametro serve per sapere quale dei thread sta elaborando
         * i dati, per informare i server di avvenuta ricezione dei dati
         * aggiornati.
         */
        public void DrawGraph(List<float> data, string category)
        {
            if (category == "modacc")
                this.Text = "Grafico Modulo Accelerometro";
            else if (category == "modgiro")
                this.Text = "Grafico Modulo Giroscopio";
            else if (category == "theta")
                this.Text = "Grafico Angolo Theta";
            else if (category == "thetaNoDiscontinuita")
                this.Text = "Grafico Angolo Theta Senza Discontinuità";
            else if (category == "yaw")
                this.Text = "Grafico Angolo Eulero Yaw";
            else if (category == "yawNoDiscontinuita")
                this.Text = "Grafico Angolo Eulero Yaw Senza Discontinuità";
            else if (category == "pitch")
                this.Text = "Grafico Angolo Eulero Pitch";
            else if (category == "pitchNoDiscontinuita")
                this.Text = "Grafico Angolo Eulero Pitch Senza Discontinuità";
            else if (category == "roll")
                this.Text = "Grafico Angolo Eulero Roll";
            else if (category == "rollNoDiscontinuita")
                this.Text = "Grafico Angolo Eulero Roll Senza Discontinuità";
            else if (category == "deviazioneStandard")
                this.Text = "Grafico Deviazione Standard";

            // Creo la lista di punti
            PointPairList list1 = new PointPairList();
            int inizio = numFinestra * 250;
            numFinestra++;
            numColore++;
            for (int i = 0; i < data.Count; ++i)
            {
                list1.Add((float)inizio*20, data[i]);
                inizio++;
            }

            myPane.XAxis.Scale.Max = (inizio + 2)*20;
            myPane.YAxis.Scale.Max = (data.Max() + 1);
            myPane.YAxis.Scale.Min = (data.Min() - 1);

            if (numColore >= lc.Count)
                numColore = 0;
            // Creo la curva da visualizzare
            myCurve = myPane.AddCurve("",
                  list1, lc[numColore], SymbolType.None);

            // Risetto gli assi
            zgc.AxisChange();
            // Forza la ri-scrittura dei dati sul grafico
            zgc.Invalidate();
            // Ricarico grafico
            zedGraphControl1.Refresh();
        }

        public void DrawGraphDR(List<float[]> data, string category)
        {
            this.Text = "Grafico Dead Reckoning";

            // Creo la lista di punti
            PointPairList list1 = new PointPairList();
            int inizio = numFinestra * 250;
            numFinestra++;
            numColore++;
            for (int i = 0; i < data.Count; ++i)
            {
                list1.Add(data[i][0], data[i][1]);
                inizio++;
            }

            myPane.XAxis.Scale.Max = (inizio + 2) * 20;

            if (numColore >= lc.Count)
                numColore = 0;
            // Creo la curva da visualizzare
            myCurve = myPane.AddCurve("",
                  list1, lc[numColore], SymbolType.None);

            // Risetto gli assi
            zgc.AxisChange();
            // Forza la ri-scrittura dei dati sul grafico
            zgc.Invalidate();
            // Ricarico grafico
            zedGraphControl1.Refresh();
        }

        private void zedGraphControl1_Load(object sender, EventArgs e)
        {

        }
    }
}
