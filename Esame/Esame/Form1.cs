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
        private List<float[,]> samples;
        private TextBox infoSample;
        private Button showSample;
        private NumericUpDown numSample;
        private NumericUpDown numSensor;
        private CheckBox eulerAngles;
        private CheckBox modulation;
        private CheckBox smoothing;
        private Server server;
        List<AngoloEulero[]> angoliEulero;
        List<float> modacc;
        List<float> modgiro;
        List<float> smoothacc;
        List<float> smoothgir;
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
            infoSample.Enabled = false;
            showSample.Enabled = false;
            numSample.Enabled = false;
            numSensor.Enabled = false;
            eulerAngles.Enabled = false;
            modulation.Enabled = false;
            smoothing.Enabled = false;

            server = new Server();
        }

        private void startServer_Click(object sender, EventArgs e)
        {
            samples = server.StartListening();
            if (samples != null)
            {
                infoSample.Enabled = true;

                showSample.Enabled = true;
                numSample.Enabled = true;
                numSample.Maximum = (samples.Count - 1);
                numSensor.Enabled = true;
                numSensor.Maximum = 4;
                eulerAngles.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            infoSample.Clear();
            infoSample.AppendText("acc1: ");
            infoSample.AppendText(samples[(int)numSample.Value][(int)numSensor.Value, 0] + "\r\n");
            infoSample.AppendText("acc2: ");
            infoSample.AppendText(samples[(int)numSample.Value][(int)numSensor.Value, 1] + "\r\n");
            infoSample.AppendText("acc3: ");
            infoSample.AppendText(samples[(int)numSample.Value][(int)numSensor.Value, 2] + "\r\n");
            infoSample.AppendText("gyr1: ");
            infoSample.AppendText(samples[(int)numSample.Value][(int)numSensor.Value, 3] + "\r\n");
            infoSample.AppendText("gyr2: ");
            infoSample.AppendText(samples[(int)numSample.Value][(int)numSensor.Value, 4] + "\r\n");
            infoSample.AppendText("gyr3: ");
            infoSample.AppendText(samples[(int)numSample.Value][(int)numSensor.Value, 5] + "\r\n");
            infoSample.AppendText("mag1: ");
            infoSample.AppendText(samples[(int)numSample.Value][(int)numSensor.Value, 6] + "\r\n");
            infoSample.AppendText("mag2: ");
            infoSample.AppendText(samples[(int)numSample.Value][(int)numSensor.Value, 7] + "\r\n");
            infoSample.AppendText("mag3: ");
            infoSample.AppendText(samples[(int)numSample.Value][(int)numSensor.Value, 8] + "\r\n");
            infoSample.AppendText("q0: ");
            infoSample.AppendText(samples[(int)numSample.Value][(int)numSensor.Value, 9] + "\r\n");
            infoSample.AppendText("q1: ");
            infoSample.AppendText(samples[(int)numSample.Value][(int)numSensor.Value, 10] + "\r\n");
            infoSample.AppendText("q2: ");
            infoSample.AppendText(samples[(int)numSample.Value][(int)numSensor.Value, 11] + "\r\n");
            infoSample.AppendText("q3: ");
            infoSample.AppendText(samples[(int)numSample.Value][(int)numSensor.Value, 12] + "\r\n");
            if (eulerAngles.Checked == true)
            {
                AngoliEulero = ElaboraDati.angoliEulero(samples);
                infoSample.AppendText("yaw: ");
                infoSample.AppendText(angoliEulero[(int)numSample.Value][(int)numSensor.Value].getYaw() + "\r\n");
                infoSample.AppendText("pitch: ");
                infoSample.AppendText(angoliEulero[(int)numSample.Value][(int)numSensor.Value].getPitch() + "\r\n");
                infoSample.AppendText("roll: ");
                infoSample.AppendText(angoliEulero[(int)numSample.Value][(int)numSensor.Value].getRoll() + "\r\n");

            }
            if (modulation.Checked == true)
            {
                modacc = ElaboraDati.modulation(samples, 0);
                modgiro = ElaboraDati.modulation(samples, 1);

            }
            if (smoothing.Checked == true)
            {
                smoothacc = ElaboraDati.smoothing(modacc, 0);
                smoothgir = ElaboraDati.smoothing(modgiro, 1);

            }
            
      }

        public List<AngoloEulero[]> AngoliEulero { get; set; }
    }
   
}

    

