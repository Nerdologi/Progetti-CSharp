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
        /* la grandezza della finestra e la capacità del buffer andranno settati
         * in base alla velocità di campionamento
         * se campiono a 50 Hz in 10s avrò 500 campioni = > buffer size > 500
         */
        public static Buffer samples;
        public static List<float[,]> samplesList;
        private static int windowSize = 500;
        private static int samplesSize = 0;
        //indica se siamo alla prima finestra di dati
        private static bool flag = true;
        //serve per salvare il tempo zero quando acquisisco il primo campione
        private static bool start = true;
        public static string path = @".\Eventi.txt";
        public static string path1 = @".\Samples.txt";
        
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
                    /* Inizializzo Thread grafici nel caso in cui fosse
                     * già stato elaborato qualche dato in una sessione 
                     * precedente, senza chiudere però il programma
                     */
                    if (ElaboraDati.GraphThread != null)
                    {
                        ElaboraDati.GraphThreadStarted = false;
                        ElaboraDati.GraphThread.Abort();
                    }
                    ElaboraDati.GraphThread = new Thread(ElaboraDati.DisegnaSulGrafico);
                    start = true;
                    ElaboraDati.windowNumber = 0;
                    if (!File.Exists(path))
                        File.Create(path);
                    //cancellazione contenuto file Eventi.txt
                    File.WriteAllText(path, string.Empty);
                    Form1.info.AppendText("Waiting for a connection at LOCALHOST... \r\n");
                    samples = new Buffer(750);
                    Socket socket = this.listener.AcceptSocket();
                    Thread t1 = new Thread(new ParameterizedThreadStart(Server.ReadFromSocket));
                    t1.Start(socket);
                }
            }
            catch (Exception e)
            {
                    Form1.info.AppendText(e.ToString());
            }
        }

        public static void  ReadFromSocket (Object obj)
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
                    samplesList = new List<float[,]>();
                    int[] t = new int[maxSensori];
                    for (int x = 0; x < numSensori; x++)
                    {
                        //array.Add(new List<double>()); // Una lista per ogni sensore
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
                                    // Lettura inversa
                                    temp[0] = pacchetto[t[i] + 3];
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
                                // Conversione
                                float valore = BitConverter.ToSingle(temp, 0);
                                //array[i].Add(valore); // memorizzazione
                                sample[i, tr] = valore;
                                // Passa al byte successivo
                                t[i] += 4;
                            }
                        }
                        if (start)
                        {
                            ElaboraDati.timeZero = DateTime.Now;
                            start = false;
                            using (StreamWriter sw = File.AppendText(Server.path))
                            {
                                sw.WriteLine("\r\n\"" + "TEMPO ZERO: " + ElaboraDati.timeZero + " " + "\"");
                            }
                        }
                        
                        samples.InsertElement(sample);
                        samplesList.Add(sample);
                        if (flag)
                        {
                            samplesSize++;
                             if (samplesSize == windowSize) { 
                                 samplesSize = 0;
                                 flag = false;
                                 ElaboraDati.graphAck = false;
                                 Thread thread = new Thread(new ParameterizedThreadStart(ElaboraDati.FunzioneCheElaboraIDati));
                                 thread.Start(samples.GetWindow(samples.Count(), windowSize));

                                 // Aspetto che i threads che gestiscono i grafici, abbiano iniziato a elaborare i dati
                                 while (!ElaboraDati.graphAck)
                                 { }
                                 // ... ora posso proseguire ...
                            }
                        }
                        else
                        {
                            samplesSize++;
                            if (samplesSize >= windowSize/2 )
                            {
                                samplesSize = 0;
                                ElaboraDati.graphAck = false;
                                Thread thread = new Thread(new ParameterizedThreadStart(ElaboraDati.FunzioneCheElaboraIDati));
                                thread.Start(samples.GetWindow(samples.Count(), windowSize));

                                // Aspetto che i threads che gestiscono i grafici, abbiano iniziato a elaborare i dati
                                while (!ElaboraDati.graphAck)
                                { }
                                // ... ora posso proseguire ...
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

                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();

                    if (flag)
                    {
                        ElaboraDati.graphAck = false;
                        Thread t2 = new Thread(new ParameterizedThreadStart(ElaboraDati.FunzioneCheElaboraIDati));
                        t2.Start(samples.GetWindow(samples.Count(), samplesSize));
                    }
                    else
                    {
                        ElaboraDati.graphAck = false;
                        Thread t2 = new Thread(new ParameterizedThreadStart(ElaboraDati.FunzioneCheElaboraIDati));
                        t2.Start(samples.GetWindow(samples.Count(), windowSize / 2 + samplesSize));
                    }

                    // Aspetto che i threads che gestiscono i grafici, abbiano iniziato a elaborare i dati
                    while (!ElaboraDati.graphAck)
                    { }
                    // ... ora posso proseguire ...

                    // Informo i threads che gestiscono i grafici che non avranno più nessun dato da elaborare
                    ElaboraDati.datiFiniti = true;

                    // Attendo avvenuta ricezione di "dati finiti" da entrambi i Threads
                    ElaboraDati.graphAck = false;
                    while (!ElaboraDati.graphAck)
                    { }
                    // ... ora posso proseguire ...
                    //Scrivo l'evento dell'ultima finestra del moto
                   using (StreamWriter sw = File.AppendText(Server.path))
                    {
                        sw.WriteLine("\r\n\"" + ElaboraDati.timeStartLastEventMoto.ToString("T") + " " + ElaboraDati.timeEndLastEventMoto.ToString("T") + " " + ElaboraDati.stateLastEventMoto + "\"");
                    }
                   //Scrivo l'evento dell'ultima finestra dell'inclinazione
                   using (StreamWriter sw = File.AppendText(Server.path))
                   {
                       sw.WriteLine("\r\n\"" + ElaboraDati.timeStartLastEventInclinazione.ToString("T") + " " + ElaboraDati.timeEndLastEventInclinazione.ToString("T") + " " + ElaboraDati.stateLastEventInclinazione + "\"");
                   }
                    //Crea un file .csv con tutti i campioni
                   string filePath = @".\Samples.csv";  
	               string delimiter = ";";  
	               int numSample=samplesList.Count();
                   int position=14*5,f=0;
	               string[][] output = new string[numSample][];
                     for (int i = 0; i < samplesList.Count(); i++) {
                         output[i] = new String[position];
                         f = 0;
                         for (int j = 0; j < 5; j++) {

                             for (int h = 0; h < 13; h++) {
                                 output[i][f] = Convert.ToString(samplesList[i][j, h]);
                                 f++;
                             }
                             output[i][f] = ";;";
                             f++;
                         }

                     }
                     
	            int length = output.GetLength(0);  
	            StringBuilder sb = new StringBuilder();  
	            for (int index = 0; index < length; index++)  
	                sb.AppendLine(string.Join(delimiter, output[index]));  
	 
	            File.WriteAllText(filePath, sb.ToString());
                    // Pulisco buffer
                    samples.Clear();
                    flag = true;
                    samplesSize = 0;

                    // Inizializzo per eventuale invio di nuovi pacchetti
                    ElaboraDati.datiAggiornati = false;
                    ElaboraDati.datiFiniti = false;
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

    }
}
