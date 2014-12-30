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
        private LineItem myCurve;

        public FormGraph()
        {
            InitializeComponent();
        }

        // Costruisce grafico
        public void InitGraph(string _title, string _XAxisTitle, string _YAxisTitle)
        {
            // Ottengo riferimento al pannello
            zgc = zedGraphControl1;
            myPane = zedGraphControl1.GraphPane;
            numFinestra = 0;

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
         * - category, che può assumere i valori "modacc" , "modgiro" o "theta"
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

            // Creo la lista di punti
            PointPairList list1 = new PointPairList();
            int inizio = numFinestra * 250;
            numFinestra++;
            for (int i = 0; i < data.Count; ++i)
            {
                list1.Add((float)inizio*20, data[i]);
                inizio++;
            }

            myPane.XAxis.Scale.Max = (inizio + 2)*20;

            // Creo la curva da visualizzare, di colore rosso
            myCurve = myPane.AddCurve("",
                  list1, Color.Red, SymbolType.None);

            // Risetto gli assi
            zgc.AxisChange();
            zgc.Invalidate();
            // Ricarico grafico
            zedGraphControl1.Refresh();
        }
    }
}
