using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Esame
{
    class Server
    {
        private TcpListener listener;
        //la grandezza della finestra e la capacità del buffer andranno settati
        //in base alla velocità di campionamento
        //se campiono a 50 Hz in 10s avrò 500 campioni = > buffer size > 500
        public static Buffer samples = new Buffer(750);
        private static int windowSize = 500;
        private static int samplesSize = 0;
        private static bool flag = true;
        
        public Server()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 45555);
            listener = new TcpListener(localEndPoint);
            listener.Start();
        }

        public void StartListening()
        {
            try
            {
                //while (true)
                {
                    Form1.info.AppendText("Waiting for a connection at LOCALHOST... \r\n");
                    Socket socket = this.listener.AcceptSocket();
                    Thread t1 = new Thread(new ParameterizedThreadStart(Server.readFromSocket));
                    t1.Start(socket);
                }
            }
            catch (Exception e)
            {
                    Form1.info.AppendText(e.ToString());
            }
        }

        public static void  readFromSocket (Object obj)
        {
            try
            {
                Socket socket = (Socket)obj;
                //using da tenere o meno??
                using (Stream myNetworkStream = new NetworkStream(socket))
                using (BinaryReader bin = new BinaryReader(myNetworkStream))
                {
                    if (Form1.info.InvokeRequired)
                    {
                        Form1.info.Invoke(new MethodInvoker(delegate { Form1.info.AppendText("\r\nClient connect :)\r\nReading samples....\r\n"); }));
                    }

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
                        byte[] read;
                        read = bin.ReadBytes(1);
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
                    int[] t = new int[maxSensori];
                    for (int x = 0; x < numSensori; x++)
                    {
                        //array.Add(new List<double>()); // una lista per ogni sensore
                        t[x] = 5 + (52 * x);
                    }
                    bool part1 = socket.Poll(1000, SelectMode.SelectRead);
                    bool part2 = (socket.Available == 0);
                    while (!part1 || !part2)
                    //while(true)
                    {
                        float[,] sample = new float[numSensori, 13];
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
                                sample[i, tr] = valore;
                                t[i] += 4; //passa al byte successivo
                            }
                        }
                        samples.insertElement(sample);
                        if (flag)
                        {
                            samplesSize++;
                             if (samplesSize == windowSize) { 
                                 samplesSize = 0;
                                 flag = false;
                                 Thread thread = new Thread(new ParameterizedThreadStart(Server.FunzioneCheElaboraIDati));
                                 thread.Start(samples.getWindow(samples.Count(), windowSize));
                            }
                        }
                        else
                        {
                            samplesSize++;
                            if (samplesSize >= windowSize/2 )
                            {
                                samplesSize = 0;
                                Thread thread = new Thread(new ParameterizedThreadStart(Server.FunzioneCheElaboraIDati));
                                thread.Start(samples.getWindow(samples.Count(), windowSize));
                            }
                        }
                        
                        
                        if (Form1.info.InvokeRequired)
                        {
                            Form1.info.Invoke(new MethodInvoker(delegate { Form1.info.AppendText(samples.Count() + ", "); }));
                        }
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
                        part1 = socket.Poll(1000, SelectMode.SelectRead);
                        part2 = (socket.Available == 0);
                    }

                    /*handler.Shutdown(SocketShutdown.Both);
                    handler.Close();*/

                }
            }
            catch (Exception e)
            {
                if (Form1.info.InvokeRequired)
                {
                    Form1.info.Invoke(new MethodInvoker(delegate { Form1.info.AppendText(e.ToString()); }));
                }
            } 
        }

        public static void FunzioneCheElaboraIDati(Object obj)
        {
            List<float[,]> window = (List<float[,]>) obj; 
            if (Form1.info.InvokeRequired)
            {
                Form1.info.Invoke(new MethodInvoker(delegate { Form1.info.AppendText("\r\nTHREAD CHE LAVORA SU UNA FINSTRA DI " + window.Count + " CAMPIONI\r\n"); }));
            }
        
        
        }
    }
}
