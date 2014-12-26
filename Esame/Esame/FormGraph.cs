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
        public FormGraph()
        {
            InitializeComponent();
        }

        // Costruisce grafico
        public void CreateGraph(List<float> data, string _title, string _XAxisTitle, string _YAxisTitle)
        {
            // Ottengo riferimento al pannello
            ZedGraphControl zgc = zedGraphControl1;
            GraphPane myPane = zgc.GraphPane;

            // Cancello eventuali linee presenti sul grafico
            myPane.CurveList.Clear();

            // Setto il titolo del grafico e le etichette degli assi cartesiani
            myPane.Title.Text = _title;
            myPane.XAxis.Title.Text = _XAxisTitle;
            myPane.YAxis.Title.Text = _YAxisTitle;

            // Creo la lista di punti
            PointPairList list1 = new PointPairList();
            for (int i = 0; i < data.Count; ++i)
            {
                list1.Add((float)i, data[i]);
            }

            

            // Creo la curva da visualizzare, di colore rosso 
            // e i diamanti come punti
            LineItem myCurve = myPane.AddCurve("",
                  list1, Color.Red, SymbolType.Default);

            myPane.XAxis.Type = AxisType.Text;

            // Risetto gli assi
            zgc.AxisChange();

            // Ricarico grafico
            zedGraphControl1.Refresh();
        }
    }
}
