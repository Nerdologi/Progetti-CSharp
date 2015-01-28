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
    public partial class FormGraphDR : Form
    {
        public FormGraphDR()
        {
            InitializeComponent();
        }

        public void InitGraph()
        {
            zedGraphControl1.GraphPane.CurveList.Clear();
        }

        public void DrawGraph(List<float[]> coords)
        {
            this.Text = "Grafico Dead Reckoning";

            // Creo la lista di punti
            PointPairList list1 = new PointPairList();
            for (int i = 0; i < coords.Count; ++i)
            {
                list1.Add((double)coords[i][0], (double)coords[i][1]);
            }

            //myPane1.XAxis.Scale.Max = (inizio + 2) * 20;

            // Creo la curva da visualizzare
            LineItem myCurve = zedGraphControl1.GraphPane.AddCurve("",
                    list1, Color.Red, SymbolType.Circle);

            // Risetto gli assi
            zedGraphControl1.AxisChange();
            // Forza la ri-scrittura dei dati sul grafico
            zedGraphControl1.Invalidate();
            // Ricarico grafico
            zedGraphControl1.Refresh();
        }
    }
}
