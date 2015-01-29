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
        private ZedGraphControl[] zgcs = new ZedGraphControl[4];
        private GraphPane[] myPanes = new GraphPane[4];
        private int numFinestra;
        private int numColore;
        private LineItem myCurve;
        private List<Color> lc = new List<Color>(5);

        public FormGraph()
        {
            InitializeComponent();
        }

        // Costruisce grafico
        public void InitGraph()
        {
            lc.Add(Color.Red);
            lc.Add(Color.Blue);
            lc.Add(Color.GreenYellow);
            lc.Add(Color.HotPink);
            lc.Add(Color.Black);
            // Ottengo riferimento al pannello
            zgcs[0] = zedGraphControl1;
            zgcs[1] = zedGraphControl2;
            zgcs[2] = zedGraphControl3;
            zgcs[3] = zedGraphControl4;
            for (int i=0; i<4; i++)
                myPanes[i] = zgcs[i].GraphPane;

            numFinestra = 0;
            numColore++;

            // Cancello eventuali linee presenti sul grafico
            for (int i = 0; i < 4; i++)
                myPanes[i].CurveList.Clear();
        }

        /* Accetta in ingresso due parametri:
         * - data, sono i punti da rappresentare sul grafico
         * - category, che può assumere i valori "modacc" , "modgiro", "theta", "thetaNoDiscontinuita", "yaw", "yawNoDiscontinuita",
         * "pitch", "pitchNoDiscontinuita", "roll", "rollNoDiscontinuita", "deviazioneStandard"
         * Il secondo parametro serve per sapere quale dei thread sta elaborando
         * i dati, per informare i server di avvenuta ricezione dei dati
         * aggiornati.
         */
        public void DrawGraph(string[] categories)
        {
            List<float>[] data = GetDataPerCategories(categories);

            if (numFinestra == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    string[] titles = GetAxisTitlesPerCategories(categories[i]);
                    myPanes[i].Title.IsVisible = false;
                    myPanes[i].XAxis.Title.Text = titles[0];
                    myPanes[i].YAxis.Title.Text = titles[1];
                }
            }

            for (int k = 0; k < 4; k++)
            {
                // Creo la lista di punti
                PointPairList list1 = new PointPairList();
                int inizio = numFinestra * 250;
                if (categories[k] != "DR")
                    for (int i = 0; i < data[k].Count; ++i)
                    {
                        list1.Add((float)inizio * 20, data[k][i]);
                        inizio++;
                    }
                else
                {
                    list1 = ElaboraDati.drPoints;
                }

                if (numColore >= lc.Count)
                    numColore = 0;
                // Creo la curva da visualizzare
                if (categories[k] != "DR")
                    myCurve = myPanes[k].AddCurve("",
                      list1, lc[numColore], SymbolType.None);
                else
                    myCurve = myPanes[k].AddCurve("",
                      list1, lc[numColore], SymbolType.Circle);

                // Risetto gli assi
                zgcs[k].AxisChange();
                // Forza la ri-scrittura dei dati sul grafico
                zgcs[k].Invalidate();
                // Ricarico grafico
                zgcs[k].Refresh();
            }

            numFinestra++;
            numColore++;
        }

        public List<float>[] GetDataPerCategories(string[] categorie)
        {
            List<float>[] data = new List<float>[4];

            for (int i=0; i<categorie.Length; i++)
            {
                switch(categorie[i])
                {
                    case "modacc":
                        data[i] = ElaboraDati.modacc;
                        break;
                    case "modgiro":
                        data[i] = ElaboraDati.modgiro;
                        break;
                    case "theta":
                        data[i] = ElaboraDati.theta;
                        break;
                    case "thetaNoDiscontinuita":
                        data[i] = ElaboraDati.thetaNoDiscontinuita;
                        break;
                    case "deviazioneStandard":
                        data[i] = ElaboraDati.SD;
                        break;
                    case "yaw":
                        data[i] = ElaboraDati.yaw;
                        break;
                    case "yawNoDiscontinuita":
                        data[i] = ElaboraDati.yawNoDiscontinuita;
                        break;
                    case "pitch":
                        data[i] = ElaboraDati.pitch;
                        break;
                    case "pitchNoDiscontinuita":
                        data[i] = ElaboraDati.pitchNoDiscontinuita;
                        break;
                    case "roll":
                        data[i] = ElaboraDati.roll;
                        break;
                    case "rollNoDiscontinuita":
                        data[i] = ElaboraDati.rollNoDiscontinuita;
                        break;
                    case "DR":
                        data[i] = null;
                        break;
                }
            }
            return data;
        }

        public string[] GetAxisTitlesPerCategories(string categoria)
        {
            string[] titles = new string[2];

            for (int i = 0; i < categoria.Length; i++)
            {
                switch (categoria)
                {
                    case "modacc":
                        titles[0] = "tempo";
                        titles[1] = "MODACC";
                        break;
                    case "modgiro":
                        titles[0] = "tempo";
                        titles[1] = "MODGIRO";
                        break;
                    case "theta":
                        titles[0] = "tempo";
                        titles[1] = "ArcTan(magnx/magnz)";
                        break;
                    case "thetaNoDiscontinuita":
                        titles[0] = "tempo";
                        titles[1] = "ArcTan(magnx/magnz)";
                        break;
                    case "deviazioneStandard":
                        titles[0] = "tempo";
                        titles[1] = "Deviazione Standard";
                        break;
                    case "yaw":
                        titles[0] = "tempo";
                        titles[1] = "Angolo Eulero Yaw sensore bacino";
                        break;
                    case "yawNoDiscontinuita":
                        titles[0] = "tempo";
                        titles[1] = "Angolo Eulero Yaw sensore bacino";
                        break;
                    case "pitch":
                        titles[0] = "tempo";
                        titles[1] = "Angolo Eulero Pitch sensore bacino";
                        break;
                    case "pitchNoDiscontinuita":
                        titles[0] = "tempo";
                        titles[1] = "Angolo Eulero Pitch sensore bacino";
                        break;
                    case "roll":
                        titles[0] = "tempo";
                        titles[1] = "Angolo Eulero Roll sensore bacino";
                        break;
                    case "rollNoDiscontinuita":
                        titles[0] = "tempo";
                        titles[1] = "Angolo Eulero Roll sensore bacino";
                        break;
                    case "DR":
                        titles[0] = "metri";
                        titles[1] = "metri";
                        break;
                }
            }
            return titles;
        }
    }
}
