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
        private Server server;
        public Form1()
        {
            InitializeComponent();
            info = this.textBoxInfo;
            infoSample = this.textBox1;
            showSample = this.button1;
            numSample = this.numericUpDown1;
            numSensor = this.numericUpDown2;
            eulerAngles = this.checkBox1;
            infoSample.Enabled = false;
            showSample.Enabled = false;
            numSample.Enabled = false;
            numSensor.Enabled = false;
            eulerAngles.Enabled = false;

            server = new Server();
        }

        private void startServer_Click(object sender, EventArgs e)
        {
            samples = server.StartListening();
            if (samples != null) {
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
            if (eulerAngles.Checked == true) {
                List<AngoloEulero[]> angoliEulero = ElabroazioneDati.angoliEulero(samples);
                infoSample.AppendText("yaw: ");
                infoSample.AppendText(angoliEulero[(int)numSample.Value][(int)numSensor.Value].getYaw() + "\r\n");
                infoSample.AppendText("pitch: ");
                infoSample.AppendText(angoliEulero[(int)numSample.Value][(int)numSensor.Value].getPitch() + "\r\n");
                infoSample.AppendText("roll: ");
                infoSample.AppendText(angoliEulero[(int)numSample.Value][(int)numSensor.Value].getRoll() + "\r\n");
            
            }
        }
    }
    public class Server
    {
        private IPAddress ipAddress;
        private Socket listener;
        public Server() 
        { 
            // Dns.GetHostName returns the name of the 
            // host running the application.
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 45555);

            // Create a TCP/IP socket.
            listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and 
            // listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);
            }
            catch (Exception e)
            {
                Form1.info.AppendText(e.ToString());
            }
        }
        public List<float[,]> StartListening()
        {
            try
            {
                // Start listening for connections.
                //while (true)
                {
                    Form1.info.AppendText("\r\nHOST IP: " + this.ipAddress.ToString()  + "\r\nWaiting for a connection....\r\n");
                    // Program is suspended while waiting for an incoming connection.
                    Socket handler = this.listener.Accept();
                    Form1.info.AppendText("\r\nClient connect :)\r\nReading samples....\r\n");
                    NetworkStream myNetworkStream =  new NetworkStream(handler, true);
                    BinaryReader bin = new BinaryReader(myNetworkStream);
                    int byteToRead;
                    byte[] pacchetto;
                    int numSensori;
                    int maxSensori = 10;
                    byte[] len = new byte[2];
                    byte[] tem = new byte[3];
                    while (!(tem[0] == 0xFF && tem[1] == 0x32)) // cerca la sequenza FF-32
                    {
                        tem[0] = tem[1];
                        tem[1] = tem[2];
                        byte[] read = bin.ReadBytes(1);
                        tem[2] = read[0];
                    }
                    if (tem[2] != 0xFF) // modalità normale
                    {
                        byteToRead = tem[2]; // byte da leggere
                    }
                    else  // modalità extended-length
                    {
                        len = new byte[2];
                        len = bin.ReadBytes(2);
                        byteToRead = (len[0] * 256) + len[1]; // byte da leggere
                    }

                    byte[] data = new byte[byteToRead + 1]; //il +1 è dovuto al checksum?
                    data = bin.ReadBytes(byteToRead + 1); // lettura dei dati

                    if (tem[2] != 0xFF)
                    {
                        pacchetto = new byte[byteToRead + 4]; // creazione pacchetto
                    }
                    else
                    {
                        pacchetto = new byte[byteToRead + 6];
                    }

                    numSensori = (byteToRead - 2) / 52; // calcolo del numero di sensori
                    //il -2 è dovuto ai due byte che fungono da contatore 
                   
                    pacchetto[0] = 0xFF; // copia dei primi elementi
                    pacchetto[1] = 0x32;
                    pacchetto[2] = tem[2];

                    if (tem[2] != 0xFF)
                    {
                        data.CopyTo(pacchetto, 3); // copia dei dati
                    }
                    else
                    {
                        pacchetto[3] = len[0];
                        pacchetto[4] = len[1];
                        data.CopyTo(pacchetto, 5); // copia dei dati
                    }


                    //List<List<double>> array = new List<List<double>>(); // salvataggio dati
                    List<float[,]> campioni = new List<float[,]>();
                    int[] t = new int[maxSensori];
                    for (int x = 0; x < numSensori; x++)
                    {
                        //array.Add(new List<double>()); // una lista per ogni sensore
                        t[x] = 5 + (52 * x);
                    }
                    bool part1 = handler.Poll(1000, SelectMode.SelectRead);
                    bool part2 = (handler.Available == 0);
                    while (!part1 || !part2)
                    //while(true) //crash
                    {
                        float [,] campione = new float[numSensori, 13];
                        for (int i = 0; i < numSensori; i++)
                        {
                            byte[] temp = new byte[4];
                            for (int tr = 0; tr < 13; tr++)// 13 campi, 3 * 3 + 4
                            {
                                if (numSensori < 5)
                                {
                                    temp[0] = pacchetto[t[i] + 3]; // lettura inversa
                                    temp[1] = pacchetto[t[i] + 2];
                                    temp[2] = pacchetto[t[i] + 1];
                                    temp[3] = pacchetto[t[i]];
                                }
                                else
                                {
                                    temp[0] = pacchetto[t[i] + 5];
                                    temp[1] = pacchetto[t[i] + 4];
                                    temp[2] = pacchetto[t[i] + 3];
                                    temp[3] = pacchetto[t[i] + 2];
                                }
                                float valore = BitConverter.ToSingle(temp, 0); // conversione
                                //array[i].Add(valore); // memorizzazione
                                campione[i, tr] = valore;
                                t[i] += 4; //passa al byte successivo
                            }
                        }
                        campioni.Add(campione);
                        Form1.info.AppendText(campioni.Count+ ", ");
                        for (int x = 0; x < numSensori; x++)
                        {
                            t[x] = 5 + (52 * x);
                        }

                        /*for (int j = 0; j < numSensori; j++)
                        {
                            for (int tr = 0; tr < 13; tr++)
                            {
                                Form1.info.AppendText(array[j][tr] + "; ");
                            }
                            array[j].RemoveRange(0, 13); // cancellazione dati
                        }*/
                        if (numSensori < 5) // lettura pacchetto seguente
                        {
                            pacchetto = bin.ReadBytes(byteToRead + 4);
                        }
                        else
                        {
                            pacchetto = bin.ReadBytes(byteToRead + 6);
                        }
                        part1 = handler.Poll(1000, SelectMode.SelectRead);
                        part2 = (handler.Available == 0);
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    return campioni;
                    
                }

            }
            catch (Exception e)
            {
                Form1.info.AppendText(e.ToString());
                return null;
            }
        }
    }

    public class ElabroazioneDati
    { 
        public static List<AngoloEulero[]> angoliEulero (List<float[,]> campioni)
        {
            
            List<AngoloEulero[]> angoliEulero = new List<AngoloEulero[]>();
            for (int i = 0; i < campioni.Count; i++){
                //un angolo di eulero per ogni sensore (nel nostro caso sono 5)
                //nel caso di un numero di sensori variabile bisogna pescare il numero di
                //sensori dalla classe Server
                angoliEulero.Add(new AngoloEulero[5]);
                //itero i sensori del campione che sto considerando
                for (int numSensore = 0; numSensore < 5; numSensore++ ){
                    float q0 = campioni[i][numSensore, 9];
                    float q1 = campioni[i][numSensore, 10];
                    float q2 = campioni[i][numSensore, 11];
                    float q3 = campioni[i][numSensore, 12];
                    float yaw = (float)Math.Atan(((2 * q2 * q3) + (2 * q0 * q1)) / ((2 * q0 * q0) + (2 * q3 * q3) - 1));
                    float pitch = (float)Math.Asin(((2 * q1 * q3) - (2 * q0 * q2)));
                    float roll = (float)Math.Atan(((2 * q1 * q2) + (2 * q0 * q3)) / ((2 * q0 * q0) + (2 * q1 * q1) - 1));
                    angoliEulero[i][numSensore] = new AngoloEulero(yaw, pitch, roll);
                }
            }

            return angoliEulero;
        
        }
    
    
    
    }

    public class AngoloEulero
    {
        float yaw;
        float pitch;
        float roll;
  
        public AngoloEulero(float yaw, float pitch, float roll)
        {
            this.yaw = yaw;
            this.pitch = pitch;
            this.roll = roll;
        }

        public void setYaw (float value)
        {
            this.yaw = value;
        }

        public void setPitch (float value)
        {
            this.pitch = value;
        }

        public void setRoll (float value)
        {
            this.roll = value;
        }

        public float getYaw ()
        {
            return this.yaw;
        }

        public float getPitch ()
        {
            return this.pitch;
        }

        public float getRoll ()
        {
            return this.roll;
        }
    }


}
