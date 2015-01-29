using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using ZedGraph;

namespace Esame
{
    class ElaboraDati
    {
        public static bool GraphThreadStarted = false;
        public static Thread GraphThread = null;
        static FormGraph fg;
        public static bool datiAggiornati = false;
        public static bool datiFiniti = false;
        public static bool graphAck = false;
        public static List<float> modacc;
        public static List<float> modgiro;
        public static List<float> theta;
        public static List<float> thetaNoDiscontinuita;
        public static List<float> SD;
        public static List<float> yaw;
        public static List<float> yawNoDiscontinuita;
        public static List<float> pitch;
        public static List<float> pitchNoDiscontinuita;
        public static List<float> roll;
        public static List<float> rollNoDiscontinuita;
        public static List<float> samplesBacinoY;
        public static PointPairList drPoints;
        public static DateTime timeZero;
        public static int windowNumber = 0;
        public static DateTime timeStartLastEventMoto;
        public static DateTime timeEndLastEventMoto;
        public static string stateLastEventMoto;
        //parametri per Lay Sit Stand
        public static float soglia_sdraiato = (float)6.0;
        public static float soglia_seduto = (float)1.9;
        public static DateTime timeStartLastEventInclinazione;
        public static DateTime timeEndLastEventInclinazione;
        public static string stateLastEventInclinazione;
        public static bool eventAllWindow = false;
        public static double x_prec = 0;
        public static double y_prec = 0;
        
        //potrebbero essere variablili condivise da più thread?
        public static float segnoAngoloTheta = 0;
        public static float segnoAngoloYaw = 0;
        public static float segnoAngoloPitch = 0;
        public static float segnoAngoloRoll = 0;

        /*
         * modulation accetta come parametri i campioni, il numero del sensore (0..5), e 
         * il tipo di modulazione da eseguire: 
         *  - 0 per modulare l'accelerometro
         *  - 1 per modulare il giroscopio
         * ritorna un array monodimensionale
         * */
        public static List<float> Modulation(List<float[,]> samples, int _S, int _type)
        {
            float a0, a1, a2;
            float g0, g1, g2;
            List<float> valori = new List<float>();
            for (int i = 0; i < samples.Count; i++)
            {
                if (_type == 0)
                {
                    a0 = samples[i][_S, 0];
                    a1 = samples[i][_S, 1];
                    a2 = samples[i][_S, 2];
                    a0 = (float)Math.Pow(a0, 2);
                    a1 = (float)Math.Pow(a1, 2);
                    a2 = (float)Math.Pow(a2, 2);
                    valori.Add((float)Math.Sqrt(a0 + a1 + a2));
                }
                else
                {
                    g0 = samples[i][_S, 3];
                    g1 = samples[i][_S, 4];
                    g2 = samples[i][_S, 5];
                    g0 = (float)Math.Pow(g0, 2);
                    g1 = (float)Math.Pow(g1, 2);
                    g2 = (float)Math.Pow(g2, 2);
                    valori.Add((float)Math.Sqrt(g0 + g1 + g2));
                }
            }
            return valori;         
        }

        public static List<float> Smoothing(List<float> module)
        {
            List<float> valori = new List<float>();
            float mean = 0;
            for (int i = 0; i < module.Count(); i++)
            {
                mean = MobileMean(i, module);            
                valori.Add(mean);
            }
            return valori;
        }

        public static float MobileMean(int i,List<float> value_mean)
        {
            float mean=0, summary=0;
            int K = 10;//Fisso a 10 la variabile K che sarà il mio range per costruire la media mobile per ogni i-esimo campione
            int j, h, f;
            ///////////////
            summary = value_mean[i];
            mean = 0;
            if (i < K && (value_mean.Count() - i - 1) >= K)//Ipotizzo che io non abbia dietro all'i-esimo valore K valori ma davanti si
            {
                for (j = i - 1; j >= 0; j--)
                    summary += value_mean[j];
                for (h = i + 1; h <= i + K; h++)
                    summary += value_mean[h];
                mean = summary / ((K + i) + 1);
            }
            else
                if (i >= K && (value_mean.Count() - i - 1) < K)//Ipotizzo che io abbia dietro K valori ma davanti no
                {
                    for (j = i - 1; j >= (i - K); j--)
                        summary += value_mean[j];
                    for (h = i + 1; h < value_mean.Count(); h++)
                        summary += value_mean[h];
                    mean = summary / (K + (value_mean.Count() - i));
                }
                else
                    if (i < K && (value_mean.Count() - i - 1) < K)//Ipotizzo che io non abbia dietro all'i-esimo valore K valori e nemmeno davanti
                    {
                        for (j = i - 1; j >= 0; j--)
                            summary += value_mean[j];
                        for (h = i + 1; h < value_mean.Count(); h++)
                            summary += value_mean[h];
                        mean = summary / ((h + i) + 1);
                    }
                    ////CASISTICA GENERALE (Ho dieci valori dietro e dieci valori davanti per cui posso effettuare una semplice media mobile)
                    else
                    {
                        f = 0;
                        j = i - 1;
                        h = i + 1;
                        while (f < K)
                        {
                            summary += value_mean[j];
                            summary += value_mean[h];
                            j--;
                            h++;
                            f++;
                        }
                        mean = summary / (2 * K + 1);

                    }
            ////////////
            return mean; 
        }

        public static List<float> DeviazioneStandard(List<float> _value)
        {
            List<float> dev_stand=new List<float>();
            List<float> value_mean;
            int K= 10;//Range di elementi per calcolare la deviazione standard mobile
            //ricavo i valori mediati tramite una media mobile(smmothing)
            value_mean = _value;// smoothing(_value);
            //Effettuo una semplice media
            float mean=0,summary=0,sd=0;
            int j, h, f;

            for (int i = 0; i < value_mean.Count(); i++)
            {
                mean = MobileMean(i, value_mean);
                summary = (float)Math.Pow( (value_mean[i] - mean) , 2);
               
                if (i < K && (value_mean.Count() - i - 1) >= K)//Ipotizzo che io non abbia dietro all'i-esimo valore K valori ma davanti si
                {
                    for (j = i - 1; j >= 0; j--)
                        summary += (float)Math.Pow( (value_mean[j] - mean) , 2);
                    for (h = i + 1; h <= i + K; h++)
                        summary += (float)Math.Pow( (value_mean[h] - mean), 2);
                    sd =(float) Math.Sqrt( summary / ((K + i) + 1) );
                }
                else
                    if (i >= K && (value_mean.Count() - i - 1) < K)//Ipotizzo che io abbia dietro K valori ma davanti no
                    {
                        for (j = i - 1; j >= (i - K); j--)
                            summary += (float)Math.Pow( (value_mean[j] - mean) , 2);
                        for (h = i + 1; h < value_mean.Count(); h++)
                            summary += (float)Math.Pow((value_mean[h] - mean), 2);
                        sd =(float)Math.Sqrt( summary / (K + (value_mean.Count() - i)));
                    }
                    else
                        if (i < K && (value_mean.Count() - i - 1) < K)//Ipotizzo che io non abbia dietro all'i-esimo valore K valori e nemmeno davanti
                        {
                            for (j = i - 1; j >= 0; j--)
                                summary += (float)Math.Pow((value_mean[j] - mean), 2);
                            for (h = i + 1; h < value_mean.Count(); h++)
                                summary += (float)Math.Pow((value_mean[h] - mean), 2);
                            sd =(float)Math.Sqrt( summary / ((h + i) + 1) );
                        }
                        // CASISTICA GENERALE (Ho dieci valori dietro e dieci valori davanti per cui posso effettuare una semplice media mobile)
                        else
                        {
                            f = 0;
                            j = i - 1;
                            h = i + 1;
                            while (f < K)
                            {
                                summary += (float)Math.Pow((value_mean[j] - mean), 2);
                                summary += (float)Math.Pow((value_mean[h] - mean), 2);
                                j--;
                                h++;
                                f++;
                            }
                            sd =(float)Math.Sqrt( summary / (2 * K + 1));

                        }
                //FINE CASISTICA GENERALE
                dev_stand.Add(sd);
            }
            return Smoothing(dev_stand);
        }

        /*
         * La 
         * prende un vettore di valori in ingresso e 
         * restituisce il vettore monodimensionale RI[]. 
         * L' elemento RI[i] rappresenta il Rapporto Incrementale
         * della funzione nel punto i.
         */ 
        public static float[] RIfun(float[] input) 
        {
            float[] RI = new float [input.Length -1];
            //l' elemento i+1 non esiste se scorro l' array fino all' elemento length
            //dunque mi fermo a length -1
            for (int i = 1; i < (input.Length - 1); i++) 
            {
                float deltaF = input[i + 1] - input[i];
                float deltaX = (i + 1) - i;
                RI[i] = deltaF / deltaX;
            }
            return RI;  
        }
        
        public static List<AngoloEulero[]> AngoliEulero(List<float[,]> campioni)
        {
            List<AngoloEulero[]> angoliEulero = new List<AngoloEulero[]>();
            for (int i = 0; i < campioni.Count; i++)
            {
                /* Un angolo di eulero per ogni sensore (nel nostro caso sono 5)
                 * nel caso di un numero di sensori variabile bisogna pescare il numero di
                 * sensori dalla classe Server
                 */
                angoliEulero.Add(new AngoloEulero[5]);
                // Itero i sensori del campione che sto considerando
                for (int numSensore = 0; numSensore < 5; numSensore++)
                {
                   float q0 = campioni[i][numSensore, 9];
                    float q1 = campioni[i][numSensore, 10];
                    float q2 = campioni[i][numSensore, 11];
                    float q3 = campioni[i][numSensore, 12];
                    //atan R -> (-pigreco/2, +pigreco/2)
                    float roll = (float)Math.Atan(((2 * q2 * q3) + (2 * q0 * q1)) / ((2 * q0 * q0) + (2 * q3 * q3) - 1));
                    //arcsin [-1, 1] -> [-pigreco/2, +pigreco/2]
                    float pitch = (float)Math.Asin(((2 * q1 * q3) - (2 * q0 * q2)));
                    //atan R -> (-pigreco/2, +pigreco/2)
                    float yaw = (float)Math.Atan(((2 * q1 * q2) + (2 * q0 * q3)) / ((2 * q0 * q0) + (2 * q1 * q1) - 1));
                    angoliEulero[i][numSensore] = new AngoloEulero(yaw, pitch, roll);
                }
            }
            return angoliEulero;
        }

        public static void FunzioneCheElaboraIDati(Object obj)
        {
            List<float[,]> window = (List<float[,]>)obj;
            if (Form1.info.InvokeRequired)
            {
                Form1.info.Invoke(new MethodInvoker(delegate { Form1.info.AppendText("\r\n\r\nTHREAD CHE LAVORA SU UNA FINSTRA DI " + window.Count + " CAMPIONI\r\n\r\n"); }));
            }

            modacc = Modulation(window, 0, 0);
            modgiro = Modulation(window, 0, 1);
            theta = new List<float>();
            thetaNoDiscontinuita = new List<float>();
            List<float[]> temp = FunzioneOrientamento(window);
            for (int i = 0; i < temp.Count; i++)
            {
                theta.Add(temp[i][0]);
                thetaNoDiscontinuita.Add(temp[i][1]);
            }
            CalcoloGirata(thetaNoDiscontinuita);
            SD = DeviazioneStandard(modacc);
            CalcolaMoto(SD);
            samplesBacinoY = ExtractionSamplesBacinoY(window);
            samplesBacinoY = Smoothing(samplesBacinoY);
            CalcoloInclinazione(samplesBacinoY);
            // Serve solo per scopi di debug in modo da vedere sul grafico l' angolo theta smussato e verficare ad occhio se le girate sono rilevate correttamente
            thetaNoDiscontinuita = Smoothing(thetaNoDiscontinuita);

            // Dead reckoning debug 
            List<AngoloEulero[]> tempAngoliEulero = AngoliEulero(window);
            yaw = new List<float>();
            pitch = new List<float>();
            roll = new List<float>();
            // Prendo tutti gli angoli di eulero per il sensore sul bacino
            foreach (AngoloEulero[] angolo in tempAngoliEulero)
            {
                yaw.Add(angolo[0].getYaw());
                pitch.Add(angolo[0].getPitch());
                roll.Add(angolo[0].getRoll());
            }

            yawNoDiscontinuita = EliminaDiscontinuitaYaw(yaw);
            pitchNoDiscontinuita = EliminaDiscontinuitaPitch(pitch);
            rollNoDiscontinuita = EliminaDiscontinuitaRoll(roll);

            drPoints = DR(SD, yawNoDiscontinuita);

            /*for (int i = 0; i < yawNoDiscontinuita.Count; i++)
            {
                yawNoDiscontinuita[i] = (float)(yawNoDiscontinuita[i] * 180 / Math.PI);
            }*/
            ElaboraDati.datiAggiornati = true;

            
            /* Se non è mai stato fatto partire il thread che gestisce lo faccio partire
             * Viene chiamato il metodo DisegnaSulGrafico() che automaticamente
             * inizializza la form dove verrà visualizzato il grafico se è il "primo avvio"
             */
            if (!GraphThreadStarted)
                GraphThread.Start();
            windowNumber++;
        }

        /* Viene chiamata solo al primo avvio del thread che gestisce il grafico
         * e inizializza gli assi e le etichette del g. stesso
         */
        public static void InizializzaGrafico()
        {
            fg = new FormGraph();
            fg.InitGraph();
            fg.Show();
        }

        // Questa fz. viene chiamata quando si lancia il thread che gestisce il grafico
        public static void DisegnaSulGrafico()
        {
            if (!GraphThreadStarted)
            {
                InizializzaGrafico();
                GraphThreadStarted = true;
            }

            fg.DrawGraph(Form1.graphs);
            
            // Informo il server che ho elaborato i dati aggiornati
            ElaboraDati.graphAck = true;

            // Aspetto che mi vengano inviati nuovi dati
            ElaboraDati.datiAggiornati = false;
            while (!datiAggiornati && !datiFiniti)
            { }

            if (!datiFiniti)
                DisegnaSulGrafico();
            else
            {
                // Mi assicuro che il server stia aspettando la mia risposta
                Thread.Sleep(1000);
                ElaboraDati.graphAck = true;
                
                // Avvio un ciclo infinito sul form del grafico per tenerlo aperto
                Application.Run();
            }
        }

        public static void CalcolaMoto(List<float> SD)
        {
            DateTime timeStart = new DateTime();
            DateTime timeEnd = new DateTime();
            bool start = true;
            string stato;
            float precedente = 0;
            int windowOverlapTime = 5000;
            int sampleTime = 20;
            DateTime precedentWindowTimeEnd = new DateTime(1993, 11, 13);
            for (int i = 0; i < SD.Count(); i++)
            {
                if (i == 0)
                    precedente = SD[i];
                if (SD[i] >= 1 && precedente >= 1)
                {
                    if (start == true)
                    {
                        timeStart = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                        start = false;
                    }
                    if (i == SD.Count() - 1)
                    {
                       timeEndLastEventMoto  = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                       stateLastEventMoto = "In moto";
                       timeStartLastEventMoto = timeStart; 
                    }
                    precedente = SD[i];
                }
                else if (SD[i] < 1 && precedente >= 1)
                {
                    precedente = SD[i];
                    start = true;
                    timeEnd = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                    stato = "In moto";
                    if (windowNumber > 0)
                        precedentWindowTimeEnd = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + windowOverlapTime);
                    // se l' evento rilevato finisce prima della conclusione della finestra precedente, questo evento è già stato letto e non lo considero
                    if (timeEnd > precedentWindowTimeEnd && i != SD.Count() - 1)
                    {
                        using (StreamWriter sw = File.AppendText(Server.path))
                        {
                            sw.WriteLine("\r\n\"" + timeStart.ToString("T") + " " + timeEnd.ToString("T") + " " + stato + "\"");
                        }
                    }
                    else if (i == SD.Count() - 1)
                    {
                        timeEndLastEventMoto = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                        stateLastEventMoto = "In moto";
                        timeStartLastEventMoto = timeStart;
                    }
                }
                else if (SD[i] < 1 && precedente < 1)
                {
                    if (start == true)
                    {
                        timeStart = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                        start = false;
                    }
                    if (i == SD.Count() - 1)
                    {
                        timeEndLastEventMoto = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                        stateLastEventMoto = "Non in moto";
                        timeStartLastEventMoto = timeStart;
                    }
                    precedente = SD[i];

                }
                else if (SD[i] >= 1 && precedente < 1)
                {
                    precedente = SD[i];
                    start = true;
                    timeEnd = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                    stato = "Non in moto";
                    if (windowNumber > 0)
                        precedentWindowTimeEnd = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + windowOverlapTime);
                    // se l' evento rilevato finisce prima della conclusione della finestra precedente, questo evento è già stato letto e non lo considero
                    if (timeEnd > precedentWindowTimeEnd && i != SD.Count() - 1)
                    {
                        using (StreamWriter sw = File.AppendText(Server.path))
                        {
                            sw.WriteLine("\r\n\"" + timeStart.ToString("T") + " " + timeEnd.ToString("T") + " " + stato + "\"");
                        }
                    }
                    else if (i == SD.Count() - 1)
                    {
                        timeEndLastEventMoto = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                        stateLastEventMoto = "Non in moto";
                        timeStartLastEventMoto = timeStart;
                    }
                }
            }
        }

        public static void CalcoloGirata(List<float> angoliTheta) 
        {
            List<float> angoliThetaGradi = new List<float>();
            DateTime timeStart = new DateTime();
            DateTime timeEnd = new DateTime();
            DateTime precedentWindowTimeEnd = new DateTime(1993,11,13);
            DateTime windowTimeEnd = new DateTime();
            //queste variabili cambieranno in base alla frequanza di campionamento
            int windowOverlapTime = 5000;
            int sampleTime = 20;
            bool start = true;
           
            foreach (float angolo in angoliTheta)
            {
                angoliThetaGradi.Add((float)/*(angolo*180/Math.PI)*/angolo);
            }
            angoliThetaGradi = Smoothing(angoliThetaGradi);
            float variazioneGlobale = 0;
            float variazioneLocale = 0;
            for (int i = 1; i < angoliThetaGradi.Count; i++)
            {
                variazioneLocale = variazioneLocale + (angoliThetaGradi[i] - angoliThetaGradi[i-1]);
                if (variazioneLocale > 6)
                {
                    if (start == true)
                    {
                        timeStart = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                        start = false;
                    }
                    if (variazioneLocale >= variazioneGlobale)
                        variazioneGlobale = variazioneLocale;
                    else if (variazioneLocale < variazioneGlobale)
                    {
                        start = true;
                        timeEnd = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                        if (windowNumber > 0)
                            precedentWindowTimeEnd = timeZero.AddMilliseconds(windowNumber* windowOverlapTime + windowOverlapTime);
                        windowTimeEnd = timeZero.AddMilliseconds(windowNumber*windowOverlapTime + windowOverlapTime* 2);
                        /* se l' evento rilevato finisce prima della conclusione della finestra precedente, questo evento è già stato letto e non lo considero
                         * se l' evento finisce con la fine della finestra molto probabilmente lo leggerò completamente con la finestra successiva e dunque
                         * non lo considero. ATTENZIONE questa condizione può creare problemi, valutare se modificarla o eliminarla
                         */
                        if (timeEnd > precedentWindowTimeEnd && timeEnd != windowTimeEnd){
                            string info = "\"" + timeStart.ToString("T") + " " + timeEnd.ToString("T") + " girata verso destra di " + variazioneGlobale + " gradi\"";
                            using (StreamWriter sw = File.AppendText(Server.path))
                            {
                                sw.WriteLine("\r\n" + info );
                            }
                        }
                        variazioneLocale = 0;
                        variazioneGlobale = 0;
                    }
                }
                else if (variazioneLocale < -6)
                {
                    if (start == true)
                    {
                        timeStart = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                        start = false;
                    }
                    if (variazioneLocale <= variazioneGlobale)
                        variazioneGlobale = variazioneLocale;
                    else if (variazioneLocale > variazioneGlobale)
                    {
                        start = true;
                        timeEnd = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                        if (windowNumber > 0)
                            precedentWindowTimeEnd = timeZero.AddMilliseconds(windowNumber* windowOverlapTime + windowOverlapTime);
                        //se l' evento rilevato finisce prima della conclusione della finestra precedente, questo evento è già stato letto e non lo considero
                        if (timeEnd > precedentWindowTimeEnd)
                        {
                            string info = "\"" + timeStart.ToString("T") + " " + timeEnd.ToString("T") + " girata verso sinistra di " + variazioneGlobale + " gradi\"";
                            using (StreamWriter sw = File.AppendText(Server.path))
                            {
                                sw.WriteLine("\r\n" + info);
                            }
                        }
                        variazioneLocale = 0;
                        variazioneGlobale = 0;
                    }
                }
            }
        }

        /* In linea teorica per lo studio della girata ci basiamo sul sensore del bacino
         * nell' algoritmo fisso dunque come numero del sensore quello 
         * che si trova sul bacino
         */
        public static List<float[]>  FunzioneOrientamento(List<float[,]> samples)
        {
            float segnoCritico = 0;
            int sensoreBacino = 0;
            List <float[]> angoliTheta = new List<float[]>();
            for (int i = 0; i < samples.Count; i++) { 
                float x = samples[i][sensoreBacino, 6];
                float y = samples[i][sensoreBacino, 7];
                float z = samples[i][sensoreBacino, 8];
                float angoloTheta = (float)Math.Atan(x / z);
                //ATTENZIONE CHE CON HZ DIVERSI DA 50 IL VALORE PER IL CONTROLLO NON è PIU 250
                /* In sostanza memorizzo il segno (variabile che mi permette di memorizzare quando raggiungo 
                 * i +pigreco/2 e i - pigreco/2 in modo da "reagire" di conseguenza)
                 * da cui dovrà partire la finestra successiva
                 */
                if (i == 250)
                    segnoCritico = segnoAngoloTheta;
                if (angoliTheta.Count > 0)
                {
                    if ((angoloTheta > 1.4 && angoliTheta[i - 1][0] < -1.4))
                        segnoAngoloTheta = segnoAngoloTheta - (float)Math.PI;
                    else if (angoloTheta < -1.4 && angoliTheta[i - 1][0] > 1.4)
                        segnoAngoloTheta = segnoAngoloTheta + (float)Math.PI;           
                }
                float[] temp = new float[2];
                temp[0] = angoloTheta;
                temp[1] = (angoloTheta + segnoAngoloTheta) * (float)(180 / Math.PI);
                angoliTheta.Add(temp);
            }
            segnoAngoloTheta = segnoCritico;
            return angoliTheta;
        }

        public static List<float> EliminaDiscontinuitaYaw(List<float> angoliYaw)
        {
            List<float[]> angoliYawNoDiscontinuita = new List<float[]>();
            float segnoCritico = 0;
            for (int i = 0; i < angoliYaw.Count; i++)
            {
                //ATTENZIONE CHE CON HZ DIVERSI DA 50 IL VALORE PER IL CONTROLLO NON è PIU 250
                if (i == 250)
                    segnoCritico = segnoAngoloYaw;
                if (angoliYawNoDiscontinuita.Count > 0)
                {
                    if ((angoliYaw[i] > 1.4 && angoliYawNoDiscontinuita[i - 1][0] < -1.4))
                        segnoAngoloYaw = segnoAngoloYaw - (float)Math.PI;
                    else if (angoliYaw[i] < -1.4 && angoliYawNoDiscontinuita[i - 1][0] > 1.4)
                        segnoAngoloYaw = segnoAngoloYaw + (float)Math.PI;
                }
                float[] temp = new float[2];
                temp[0] = angoliYaw[i];
                //temp[1] = (angoliYaw[i] + segnoAngoloYaw) * (float)(180 / Math.PI);
                temp[1] = (angoliYaw[i] + segnoAngoloYaw);
                angoliYawNoDiscontinuita.Add(temp);
            }
            List<float> returnList = new List<float>();
            for (int i = 0; i < angoliYawNoDiscontinuita.Count; i++)
            { 
                returnList.Add(angoliYawNoDiscontinuita[i][1]);
            }
            segnoAngoloYaw = segnoCritico;
            return returnList;
        }

        public static List<float> EliminaDiscontinuitaPitch(List<float> angoliPitch)
        {
            float segnoCritico = 0;
            List<float[]> angoliPitchNoDiscontinuita = new List<float[]>();
            for (int i = 0; i < angoliPitch.Count; i++)
            {
                //ATTENZIONE CHE CON HZ DIVERSI DA 50 IL VALORE PER IL CONTROLLO NON è PIU 250
                if (i == 250)
                    segnoCritico = segnoAngoloPitch;
                if (angoliPitchNoDiscontinuita.Count > 0)
                {
                    if ((angoliPitch[i] > 1.4 && angoliPitchNoDiscontinuita[i - 1][0] < -1.4))
                        segnoAngoloPitch = segnoAngoloPitch - (float)Math.PI;
                    else if (angoliPitch[i] < -1.4 && angoliPitchNoDiscontinuita[i - 1][0] > 1.4)
                        segnoAngoloPitch = segnoAngoloPitch + (float)Math.PI;

                }
                float[] temp = new float[2];
                temp[0] = angoliPitch[i];
                //temp[1] = (angoliPitch[i] + segnoAngoloPitch) * (float)(180 / Math.PI);
                temp[1] = (angoliPitch[i] + segnoAngoloPitch);
                angoliPitchNoDiscontinuita.Add(temp);
            }
            List<float> returnList = new List<float>();
            for (int i = 0; i < angoliPitchNoDiscontinuita.Count; i++)
            { 
                returnList.Add(angoliPitchNoDiscontinuita[i][1]);
            }
            segnoAngoloPitch = segnoCritico;
            return returnList;
        }

        public static List<float> EliminaDiscontinuitaRoll(List<float> angoliRoll)
        {
            float segnoCritico = 0;
            List<float[]> angoliRollNoDiscontinuita = new List<float[]>();
            for (int i = 0; i < angoliRoll.Count; i++)
            {
                //ATTENZIONE CHE CON HZ DIVERSI DA 50 IL VALORE PER IL CONTROLLO NON è PIU 250
                if (i == 250)
                    segnoCritico = segnoAngoloRoll;
                if (angoliRollNoDiscontinuita.Count > 0)
                {
                    if ((angoliRoll[i] > 1.4 && angoliRollNoDiscontinuita[i - 1][0] < -1.4))
                        segnoAngoloRoll = segnoAngoloRoll - (float)Math.PI;
                    else if (angoliRoll[i] < -1.4 && angoliRollNoDiscontinuita[i - 1][0] > 1.4)
                        segnoAngoloRoll = segnoAngoloRoll + (float)Math.PI;

                }
                float[] temp = new float[2];
                temp[0] = angoliRoll[i];
                //temp[1] = (angoliRoll[i] + segnoAngoloRoll) * (float)(180 / Math.PI);
                temp[1] = (angoliRoll[i] + segnoAngoloRoll);
                angoliRollNoDiscontinuita.Add(temp);
            }
            List<float> returnList = new List<float>();
            for (int i = 0; i < angoliRollNoDiscontinuita.Count; i++)
            {
                returnList.Add(angoliRollNoDiscontinuita[i][1]);
            }
            segnoAngoloRoll = segnoCritico;
            return returnList;
        }

        public static List<float> ExtractionSamplesBacinoY(List<float[,]> samples)
        {
            List<float> valuesY=new List<float>();
             for (int i = 0; i < samples.Count; i++)                
                    valuesY.Add(samples[i][0, 1]);

            return valuesY;
        }

        public static bool Lay(float i) 
        {
            //float soglia_sdraiato=6.0;
            if (i > soglia_sdraiato)
                return true;
            else
                return false;
        }

        public static bool Sit(float i) 
        { 
            //soglia_seduto=1.5
            if (i < soglia_sdraiato && i > soglia_seduto)
                return true;
            else
                return false;
        }

        public static bool Stand(float i)
        {
            //soglia_seduto=1.5
            if ( i < soglia_seduto)
                return true;
            else
                return false;
        }

        public static void CalcoloInclinazione(List<float> samples) 
        {
            DateTime timeStart = new DateTime();
            DateTime timeEnd = new DateTime();
            bool start = true;
            string stato;
            float precedente = 0;
            int windowOverlapTime = 5000;
            int sampleTime = 20;
            DateTime precedentWindowTimeEnd = new DateTime(1993, 11, 13);
            int jPiedi = 0;
            for (int i = 0; i < samples.Count(); i++)
            {
                if (i == 0)
                    precedente = samples[i];
                if (Lay(samples[i]) && Lay(precedente))
                {
                    if (start == true)
                    {
                        timeStart = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                        start = false;
                    }
                    if (i == samples.Count() - 1)
                    {
                        timeEndLastEventInclinazione = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                        stateLastEventInclinazione = "Sdraiato";
                        timeStartLastEventInclinazione = timeStart;
                    }
                    precedente = samples[i];
                }
                else if (Sit(samples[i])  && Lay(precedente))
                {
                    precedente = samples[i];
                    start = true;
                    timeEnd = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                    stato = "Sdraiato";
                    if (windowNumber > 0)
                        precedentWindowTimeEnd = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + windowOverlapTime);
                    // se l' evento rilevato finisce prima della conclusione della finestra precedente, questo evento è già stato letto e non lo considero
                    if (timeEnd > precedentWindowTimeEnd && i != samples.Count() - 1)
                    {
                        using (StreamWriter sw = File.AppendText(Server.path))
                        {
                            sw.WriteLine("\r\n\"" + timeStart.ToString("T") + " " + timeEnd.ToString("T") + " " + stato + "\"");
                        }
                    }
                    else if (i == samples.Count() - 1)
                    {
                        timeEndLastEventInclinazione = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                        stateLastEventInclinazione = "Sdraiato";
                        timeStartLastEventInclinazione = timeStart;
                    }
                }
                else if (Sit(samples[i]) && Sit(precedente))
                {
                    if (start == true)
                    {
                        timeStart = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                        start = false;
                    }
                    if (i == samples.Count() - 1)
                    {
                        timeEndLastEventInclinazione = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                        stateLastEventInclinazione = "Seduto";
                        timeStartLastEventInclinazione = timeStart;
                    }
                    precedente = samples[i];

                }
                else if (Lay(samples[i]) && Sit(precedente))
                {
                    precedente = samples[i];
                    start = true;
                    timeEnd = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                    stato = "Seduto";
                    if (windowNumber > 0)
                        precedentWindowTimeEnd = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + windowOverlapTime);
                    // se l' evento rilevato finisce prima della conclusione della finestra precedente, questo evento è già stato letto e non lo considero
                    if (timeEnd > precedentWindowTimeEnd && i != samples.Count() - 1)
                    {
                        using (StreamWriter sw = File.AppendText(Server.path))
                        {
                            sw.WriteLine("\r\n\"" + timeStart.ToString("T") + " " + timeEnd.ToString("T") + " " + stato + "\"");
                        }
                    }
                    else if (i == samples.Count() - 1)
                    {
                        timeEndLastEventInclinazione = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                        stateLastEventInclinazione = "Seduto";
                        timeStartLastEventInclinazione = timeStart;
                    }
                }
                else if (Stand(samples[i]) && Stand(precedente))
                {
                    if (start == true)
                    {
                        timeStart = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                        start = false;
                    }
                    if (i == samples.Count() - 1)
                    {
                        timeEndLastEventInclinazione = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                        stateLastEventInclinazione = " In Piedi";
                        timeStartLastEventInclinazione = timeStart;
                    }
                    else
                        jPiedi++;
                    precedente = samples[i];
                }
                else if (Sit(samples[i]) && Stand(precedente))
                {
                    precedente = samples[i];
                    start = true;
                    timeEnd = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                    stato = "In Piedi";
                    if (windowNumber > 0)
                        precedentWindowTimeEnd = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + windowOverlapTime);
                    // se l' evento rilevato finisce prima della conclusione della finestra precedente, questo evento è già stato letto e non lo considero
                    if ((timeEnd > precedentWindowTimeEnd && i != samples.Count() - 1) || eventAllWindow==true)
                    {
                        eventAllWindow = false;
                        using (StreamWriter sw = File.AppendText(Server.path))
                        {
                            sw.WriteLine("\r\n\"" + timeStart.ToString("T") + " " + timeEnd.ToString("T") + " " + stato + "\"");
                        }
                    }
                    else if (i == samples.Count() - 1)
                    {
                        timeEndLastEventInclinazione = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                        stateLastEventInclinazione = "In Piedi";
                        timeStartLastEventInclinazione = timeStart;
                    }
                }
                else if (Stand(samples[i]) && Sit(precedente))
                {
                    precedente = samples[i];
                    start = true;
                    timeEnd = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                    stato = "Seduto";
                    if (windowNumber > 0)
                        precedentWindowTimeEnd = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + windowOverlapTime);
                    // se l' evento rilevato finisce prima della conclusione della finestra precedente, questo evento è già stato letto e non lo considero
                    if (timeEnd > precedentWindowTimeEnd && i != samples.Count() - 1)
                    {
                        using (StreamWriter sw = File.AppendText(Server.path))
                        {
                            sw.WriteLine("\r\n\"" + timeStart.ToString("T") + " " + timeEnd.ToString("T") + " " + stato + "\"");
                        }
                    }
                    if (i == samples.Count() - 1)
                    {
                        timeEndLastEventInclinazione = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (i * sampleTime));
                        stateLastEventInclinazione = "Seduto";
                        timeStartLastEventInclinazione = timeStart;
                    }
                    precedente = samples[i];

                }
            }
           if (jPiedi == samples.Count() - 1 && windowNumber > 0 && windowNumber < 3) 
           {
                stato = "In Piedi";
                /*timeEnd = timeZero.AddMilliseconds(windowNumber * windowOverlapTime + (samples.Count() * sampleTime));
                    using (StreamWriter sw = File.AppendText(Server.path))
                    {
                        sw.WriteLine("\r\n\"" + timeStart.ToString("T") + " " + timeEnd.ToString("T") + " " + stato + "\"");
                    }*/
                eventAllWindow = true;
            }
        }

        // Angoli di Yaw senza discontinuità
        public static List<float[]> DeadReckoning(List<float> SD, List<float>yaw)
        {
            List<float> yawNoDisconituinitaSmooted = Smoothing(yaw);
            List<float[]> coordinateDR = new List<float[]>();
            // Queste variabili cambieranno in base alla frequanza di campionamento
            float sampleTime = (float)0.020;
            bool start = true;
            float variazioneGlobale = 0;
            float variazioneLocale = 0;
            int xTempGirata = 0;
            int xTempoTotale = 0;
            float yPrec = 0;
            float angoloInizio;
            float maxSD = SD.Max();

            for (int i = 0; i<yawNoDisconituinitaSmooted.Count; i++)
            {
                yawNoDisconituinitaSmooted[i] = yawNoDisconituinitaSmooted[i] * (float)(180 / Math.PI);
            }

            angoloInizio = yawNoDisconituinitaSmooted[0];

            
            coordinateDR.Add(new float[2] { 0, 0 });
            for (int i = 1; i < yawNoDisconituinitaSmooted.Count; i++)
            {
                xTempGirata++;
                xTempoTotale++;
                variazioneLocale = variazioneLocale + (yawNoDisconituinitaSmooted[i] - yawNoDisconituinitaSmooted[i - 1]);
                if (Math.Abs(variazioneLocale) > 6)
                {
                    if (start == true)
                    {
                        yPrec = variazioneLocale;
                        start = false;
                        xTempGirata = 0;
                    }
                    if (variazioneLocale >= variazioneGlobale)
                        variazioneGlobale = variazioneLocale;
                    else if (variazioneLocale < variazioneGlobale)
                    {
                        start = true;
                        if (variazioneGlobale - yPrec > 50)
                        {
                            /*float distanzaGirata = velocita * xTempGirata * sampleTime;
                            // triangolo isoscele dove i lati uguali sono la distanza, e la base è il lato da trovare
                            double angoloBeta = 180 - variazioneGlobale - 90;
                            float altezzaG = (float)(distanzaGirata * Math.Cos(angoloBeta));
                            float ascissaG = (float)(coordinateDR.Last()[0] + altezzaG);
                            float ordinataG = (float)(Math.Sqrt(Math.Pow(distanzaGirata, 2) - Math.Pow(altezzaG, 2)) + coordinateDR.Last()[1]);*/

                            float sum = 0;
                            int count = 0;
                            for (int m = (i-xTempoTotale); m<(i-xTempGirata); m++)
                            {
                                sum += SD[m];
                                count++;
                            }
                            float velocitaD = sum / count / maxSD;

                            sum = 0;
                            count = 0;
                            for (int m = (i - xTempGirata); m < i; m++)
                            {
                                sum += SD[m];
                                count++;
                            }
                            float velocitaG = sum / count / maxSD;

                            float distanzaDritto = velocitaD * (xTempoTotale - xTempGirata) * sampleTime;
                            float ascissaD = (float)(distanzaDritto * Math.Cos(angoloInizio * Math.PI / 180));
                            float ordinataD = (float)(distanzaDritto * Math.Sin(angoloInizio * Math.PI / 180));

                            coordinateDR.Add(new float[2] { coordinateDR.Last()[0] + ascissaD, coordinateDR.Last()[1] + ordinataD });

                            float distanzaGirata = velocitaG * xTempGirata * sampleTime;
                            float ascissaG = (float)(distanzaGirata * Math.Cos((variazioneGlobale + angoloInizio) * Math.PI / 180));
                            float ordinataG = (float)(distanzaGirata * Math.Sin((variazioneGlobale + angoloInizio) * Math.PI / 180));

                            coordinateDR.Add(new float[2] { coordinateDR.Last()[0] + ascissaG, coordinateDR.Last()[1] + ordinataG });

                            xTempGirata = 0;
                            xTempoTotale = 0;
                            angoloInizio += variazioneGlobale;
                        }
                        variazioneLocale = 0;
                        variazioneGlobale = 0;
                    }
                }
            }

            float somma = 0;
            int c = 0;
            for (int m = (SD.Count - xTempoTotale); m < SD.Count; m++)
            {
                somma += SD[m];
                c++;
            }
            float velocita = somma / c / maxSD;

            float distanzaDrittoFinale = velocita * (xTempoTotale) * sampleTime;
            float ascissaDFinale = (float)(distanzaDrittoFinale * Math.Cos(angoloInizio * Math.PI / 180));
            float ordinataDFinale = (float)(distanzaDrittoFinale * Math.Sin(angoloInizio * Math.PI / 180));

            coordinateDR.Add(new float[2] { coordinateDR.Last()[0] + ascissaDFinale, coordinateDR.Last()[1] + ordinataDFinale });
            return coordinateDR;
        }

        public static void DisegnaGraficoDR()
        {
            ElaboraDati.segnoAngoloTheta = 0;
            ElaboraDati.segnoAngoloYaw = 0;
            ElaboraDati.segnoAngoloPitch = 0;
            ElaboraDati.segnoAngoloRoll = 0;
            List<float> yaw = new List<float>();

            // Prendo tutti gli angoli di eulero per il sensore sul bacino
            foreach (AngoloEulero[] angolo in ElaboraDati.AngoliEulero(Server.samplesList))
            {
                yaw.Add(angolo[0].getYaw());
            }
            List<float[]> coordinate = DeadReckoning(DeviazioneStandard(ElaboraDati.Modulation(Server.samplesList, 0, 0)), EliminaDiscontinuitaYaw(yaw));
            FormGraphDR fgdr = new FormGraphDR();
            fgdr.InitGraph();
            fgdr.Show();
            fgdr.DrawGraph(coordinate);
            Application.Run();
        }

        public static PointPairList DR(List<float> SD, List<float> yaw)
        {
            double x = 0, y = 0;
            PointPairList punti = new PointPairList();

            for (int i = 1; i < SD.Count; i++) 
            {
                if (SD[i] > 1.5)
                {
                    if (yaw[i] > Math.PI)
                    { }
                    x = SD[i] * Math.Cos(yaw[i]);
                    y =SD[i] * Math.Sin(yaw[i]);
                    punti.Add(new PointPair(x_prec, y_prec));
                    x_prec += x;
                    y_prec += y;
                }
            }

            return punti;
        }
    }
}
