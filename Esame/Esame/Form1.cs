using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.IO;

namespace Esame
{
    public partial class Form1 : Form
    {
        public static TextBox info;
        private TextBox infoSample;
        private Button showSample;
        private NumericUpDown numSample;
        private NumericUpDown numSensor;
        private CheckBox eulerAngles;
        private CheckBox modulation;
        private CheckBox smoothing;
        private CheckBox modaccRI;
        private CheckBox sd;
        private CheckBox graficoAngoloTheta;
        private CheckBox Yvalue;
        private Server server;
        List<float> modacc;
        List<float> modgiro;
        List<float> smoothacc;
        List<float> smoothgir;
        List<float> dev;
        private List<AngoloEulero[]> AngoliEulero;
        public static string[] graphs = new string[4];

        public Form1()
        {
            InitializeComponent();
            info = this.textBoxInfo;
            infoSample = this.textBox1;
            showSample = this.button1;
            numSample = this.numericUpDown1;
            numSensor = this.numericUpDown2;
            eulerAngles = this.checkBox1;
            modulation = this.checkBox2;
            smoothing = this.checkBox3;
            modaccRI = this.checkBox4;
            sd = this.checkBox5;
            Yvalue = this.checkBox7;
            graficoAngoloTheta = this.checkBox6;
            infoSample.Enabled = false;
            showSample.Enabled = false;
            numSample.Enabled = false;
            numSensor.Enabled = false;
            eulerAngles.Enabled = false;
            modulation.Enabled = false;
            smoothing.Enabled = false;
            modaccRI.Enabled = false;
            sd.Enabled = false;
            graficoAngoloTheta.Enabled = false;
            Yvalue.Enabled = false;

            server = new Server();
        }

        private void startServer_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.CheckedIndices.Count == 4)
            {
                for (int i = 0; i < 4; i++)
                    graphs[i] = GetGraphKey(checkedListBox1.CheckedIndices[i]);
                server.StartListening();

                infoSample.Enabled = true;
                showSample.Enabled = true;
                numSample.Enabled = true;

                numSensor.Enabled = true;
                numSensor.Maximum = 4;
                eulerAngles.Enabled = true;
                modulation.Enabled = true;
                smoothing.Enabled = true;
                modaccRI.Enabled = true;
                sd.Enabled = true;
                graficoAngoloTheta.Enabled = true;
                Yvalue.Enabled = true;
            }
            else
                MessageBox.Show("Seleziona 4 grafici che vuoi visualizzare!");
        }

        public string GetGraphKey(int index)
        {
            string key = "";

            switch (index)
            {
                case 0:
                    key = "modacc";
                    break;
                case 1:
                    key = "modgiro";
                    break;
                case 2:
                    key = "theta";
                    break;
                case 3:
                    key = "thetaNoDiscontinuita";
                    break;
                case 4:
                    key = "deviazioneStandard";
                    break;
                case 5:
                    key = "yaw";
                    break;
                case 6:
                    key = "yawNoDiscontinuita";
                    break;
                case 7:
                    key = "pitch";
                    break;
                case 8:
                    key = "pitchNoDiscontinuita";
                    break;
                case 9:
                    key = "roll";
                    break;
                case 10:
                    key = "rollNoDiscontinuita";
                    break;
            }
            return key;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            numSample.Maximum = (Server.samplesList.Count - 1);
            infoSample.Clear();
            infoSample.AppendText("acc1: ");
            infoSample.AppendText(Server.samplesList[(int)numSample.Value][(int)numSensor.Value, 0] + "\r\n");
            infoSample.AppendText("acc2: ");
            infoSample.AppendText(Server.samplesList[(int)numSample.Value][(int)numSensor.Value, 1] + "\r\n");
            infoSample.AppendText("acc3: ");
            infoSample.AppendText(Server.samplesList[(int)numSample.Value][(int)numSensor.Value, 2] + "\r\n");
            infoSample.AppendText("gyr1: ");
            infoSample.AppendText(Server.samplesList[(int)numSample.Value][(int)numSensor.Value, 3] + "\r\n");
            infoSample.AppendText("gyr2: ");
            infoSample.AppendText(Server.samplesList[(int)numSample.Value][(int)numSensor.Value, 4] + "\r\n");
            infoSample.AppendText("gyr3: ");
            infoSample.AppendText(Server.samplesList[(int)numSample.Value][(int)numSensor.Value, 5] + "\r\n");
            infoSample.AppendText("mag1: ");
            infoSample.AppendText(Server.samplesList[(int)numSample.Value][(int)numSensor.Value, 6] + "\r\n");
            infoSample.AppendText("mag2: ");
            infoSample.AppendText(Server.samplesList[(int)numSample.Value][(int)numSensor.Value, 7] + "\r\n");
            infoSample.AppendText("mag3: ");
            infoSample.AppendText(Server.samplesList[(int)numSample.Value][(int)numSensor.Value, 8] + "\r\n");
            infoSample.AppendText("q0: ");
            infoSample.AppendText(Server.samplesList[(int)numSample.Value][(int)numSensor.Value, 9] + "\r\n");
            infoSample.AppendText("q1: ");
            infoSample.AppendText(Server.samplesList[(int)numSample.Value][(int)numSensor.Value, 10] + "\r\n");
            infoSample.AppendText("q2: ");
            infoSample.AppendText(Server.samplesList[(int)numSample.Value][(int)numSensor.Value, 11] + "\r\n");
            infoSample.AppendText("q3: ");
            infoSample.AppendText(Server.samplesList[(int)numSample.Value][(int)numSensor.Value, 12] + "\r\n");
            if (eulerAngles.Checked == true)
            {
                List<AngoloEulero[]> 
                AngoliEulero = ElaboraDati.AngoliEulero(Server.samplesList);
                infoSample.AppendText("yaw: ");
                infoSample.AppendText(AngoliEulero[(int)numSample.Value][(int)numSensor.Value].getYaw() + "\r\n");
                infoSample.AppendText("pitch: ");
                infoSample.AppendText(AngoliEulero[(int)numSample.Value][(int)numSensor.Value].getPitch() + "\r\n");
                infoSample.AppendText("roll: ");
                infoSample.AppendText(AngoliEulero[(int)numSample.Value][(int)numSensor.Value].getRoll() + "\r\n");

            }
            if (modulation.Checked == true)
            {
                modacc = ElaboraDati.Modulation(Server.samplesList, (int)numSensor.Value, 0);
                modgiro = ElaboraDati.Modulation(Server.samplesList, (int)numSensor.Value, 1);
                infoSample.AppendText("\r\nmodacc: ");
                foreach (float elem in modacc)
                {
                    infoSample.AppendText(elem + " - ");
                }
                infoSample.AppendText("\r\nmodgiro: ");
                foreach (float elem in modgiro)
                {
                    infoSample.AppendText(elem + " - ");
                }
            }
            if (smoothing.Checked == true)
            {
                smoothacc = ElaboraDati.Smoothing(modacc);
                smoothgir = ElaboraDati.Smoothing(modgiro);
                infoSample.AppendText("\r\nsmoothacc: ");
                foreach (float elem in smoothacc)
                {
                    infoSample.AppendText(elem + " - ");
                }
                infoSample.AppendText("\r\nsmoothgir: ");
                foreach (float elem in smoothgir)
                {
                    infoSample.AppendText(elem + " - ");
                }
            }
            if (modaccRI.Checked == true)
            {
                modacc = ElaboraDati.Modulation(Server.samplesList, (int)numSensor.Value, 0);
                /* Fino ad ora le varie informazioni sono state salvate in List dato che
                 * non lavoriamo su finestre di dati, ma sull' insieme di campioni
                 * e non  possiamo sapere a priori quanti sono e dunque abbaimo bisogno di 
                 * strutture dati dinamiche. Dalla documentazione la funzione RIfun accetta in input
                 * un array e dunque ho dovuto trasfomare la List in array in modo che la signature della 
                 * funzione sia esattamente uguale a quella della documentazione
                 */
                float[] modaccArray = modacc.ToArray();
                float[] modaccRIArray = ElaboraDati.RIfun(modaccArray);
                infoSample.AppendText("\r\nFI of modacc: ");
                foreach (float elem in modaccRIArray)
                {
                    infoSample.AppendText(elem + " - ");
                }
              
            }
            if (sd.Checked == true)
            {
                dev = ElaboraDati.DeviazioneStandard(modacc);
                infoSample.AppendText("\r\nDeviazioni standard: \r\n");
                foreach (float elem in dev)
                {
                    infoSample.AppendText(elem + " - ");
                }
            
            }
            if (Yvalue.Checked == true)
            {
                infoSample.AppendText("\r\n" + " Valori Y del smussati: " +"\r\n");
                List<float> smussati = new List<float>();
                for (int i = 0; i < Server.samplesList.Count(); i++)
                    smussati.Add(Server.samplesList[i][0, 1]);
                smussati=ElaboraDati.Smoothing(smussati);
                for(int i=0;i < Server.samplesList.Count();i++)
                infoSample.AppendText(smussati[i] + "\r\n");
                
            }
        
        }
        
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox3.Checked == true)
                checkBox2.Checked = true;
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox5.Checked == true)
                checkBox2.Checked = true;
        }
    }
}

    

