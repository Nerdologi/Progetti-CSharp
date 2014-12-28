using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Esame
{
    class ElaboraDati
    {
        public static bool ModaccGraphThreadStarted = false;
        public static Thread ModaccGraphThread = null;
        static FormGraph fGAcc;
        public static bool datiAggiornati = false;
        public static bool datiFiniti = false;
        public static bool graphAck = false;
        static List<float> modacc;
        static List<float> modgiro;

        /*
         * modulation accetta come parametri i campioni, il numero del sensore (0..5), e 
         * il tipo di modulazione da eseguire: 
         *  - 0 per modulare l'accelerometro
         *  - 1 per modulare il giroscopio
         * ritorna un array monodimensionale
         * */
        public static List<float> modulation(List<float[,]> samples, int _S, int _type)
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

        public static List<float> smoothing(List<float> module)
        {
            List<float> valori = new List<float>();
            float mean = 0;
            int K = 10;//Fisso a 10 la variabile K che sarà il mio range per costruire la media mobile per ogni i-esimo campione
            float summary=0;
            int j,h,f;
            for (int i = 0; i < module.Count(); i++)
            {
                
                summary = module[i];
                mean = 0;
                if (i < K && (module.Count() - i - 1) >= K)//Ipotizzo che io non abbia dietro all'i-esimo valore K valori ma davanti si
                {
                    for (j = i - 1; j >= 0; j--)
                        summary += module[j];
                    for (h = i + 1; h <= i + K; h++)
                        summary += module[h];
                    mean = summary / ((K + i) + 1);
                }
                else
                if(i >= K && (module.Count() - i - 1) < K)//Ipotizzo che io abbia dietro K valori ma davanti no
                {
                    for (j = i - 1; j >= (i - K); j--)
                        summary += module[j];
                    for (h = i + 1; h < module.Count(); h++)
                        summary += module[h];
                    mean = summary / (K + (module.Count() - i) );
                }
                else
                if (i < K && (module.Count() - i - 1) < K)//Ipotizzo che io non abbia dietro all'i-esimo valore K valori e nemmeno davanti
                {
                    for (j = i - 1; j >= 0; j--)
                        summary += module[j];
                    for (h = i + 1; h < module.Count(); h++)
                        summary += module[h];
                    mean = summary / ((h + i) + 1);
                }
                    ////CASISTICA GENERALE (Ho dieci valori dietro e dieci valori davanti per cui posso effettuare una semplice media mobile)
                else
                {    
                     f=0;
                     j = i - 1;
                     h = i + 1;
                    while (f < K)
                    {
                        summary += module[j]; 
                        summary += module[h];
                        j--;
                        h++;
                        f++;
                    }
                    mean = summary / (2 * K + 1);

                  }
                //FINE CASISTICA GENERALE
                valori.Add(mean);
            }
            return valori;
        }

        public static List<float> deviazioneStandard(List<float> _value)
        {
            List<float> dev_stand=new List<float>();
            List<float> value_mean;
            int K= 10;//Range di elementi per calcolare la deviazione standard mobile
            //ricavo i valori mediati tramite una media mobile(smmothing)
            value_mean = smoothing(_value);
            //Effettuo una semplice media
            float mean=0,summary=0,sd=0;
            int j, h, f;

            for (int i = 0; i < value_mean.Count(); i++)
                summary += value_mean[i];
            mean = summary / value_mean.Count();
            /////////////////////////////////////////////

            for (int i = 0; i < _value.Count(); i++)
            {

                summary = (float)Math.Pow( (_value[i] - mean) , 2);
               
                if (i < K && (_value.Count() - i - 1) >= K)//Ipotizzo che io non abbia dietro all'i-esimo valore K valori ma davanti si
                {
                    for (j = i - 1; j >= 0; j--)
                        summary += (float)Math.Pow( (_value[j] - mean) , 2);
                    for (h = i + 1; h <= i + K; h++)
                        summary += (float)Math.Pow( (_value[h] - mean), 2);
                    sd =(float) Math.Sqrt( summary / ((K + i) + 1) );
                }
                else
                    if (i >= K && (_value.Count() - i - 1) < K)//Ipotizzo che io abbia dietro K valori ma davanti no
                    {
                        for (j = i - 1; j >= (i - K); j--)
                            summary += (float)Math.Pow( (_value[j] - mean) , 2);
                        for (h = i + 1; h < _value.Count(); h++)
                            summary += (float)Math.Pow((_value[h] - mean), 2);
                        sd =(float)Math.Sqrt( summary / (K + (_value.Count() - i)));
                    }
                    else
                        if (i < K && (_value.Count() - i - 1) < K)//Ipotizzo che io non abbia dietro all'i-esimo valore K valori e nemmeno davanti
                        {
                            for (j = i - 1; j >= 0; j--)
                                summary += (float)Math.Pow((_value[j] - mean), 2);
                            for (h = i + 1; h < _value.Count(); h++)
                                summary += (float)Math.Pow((_value[h] - mean), 2);
                            sd =(float)Math.Sqrt( summary / ((h + i) + 1) );
                        }
                        ////CASISTICA GENERALE (Ho dieci valori dietro e dieci valori davanti per cui posso effettuare una semplice media mobile)
                        else
                        {
                            f = 0;
                            j = i - 1;
                            h = i + 1;
                            while (f < K)
                            {
                                summary += (float)Math.Pow((_value[j] - mean), 2);
                                summary += (float)Math.Pow((_value[h] - mean), 2);
                                j--;
                                h++;
                                f++;
                            }
                            sd =(float)Math.Sqrt( summary / (2 * K + 1));

                        }
                //FINE CASISTICA GENERALE
                dev_stand.Add(sd);
            }



            ////////////////////////////////////////////
            

            return dev_stand;
            }

        /*
         *La funzione prende un vettore di valori in ingresso e 
         *restituisce il vettore monodimensionale RI[]. 
         *L' elemento RI[i] rappresenta il Rapporto Incrementale
         *della funzione nel punto i.
         */ 
        public static float[] RIfun(float[] input) 
        {
            float[] RI = new float [input.Length -1];
            //l' elemento i+1 non esiste se scorro l' array fino all' elemento length
            //dunque mi fermo a length -1
            for (int i = 1; i < (input.Length - 1); i++) {
                float deltaF = input[i + 1] - input[i];
                float deltaX = (i + 1) - i;
                RI[i] = deltaF / deltaX;
            }
            return RI;  
        }
        public static List<AngoloEulero[]> angoliEulero(List<float[,]> campioni)
        {

            List<AngoloEulero[]> angoliEulero = new List<AngoloEulero[]>();
            for (int i = 0; i < campioni.Count; i++)
            {
                //un angolo di eulero per ogni sensore (nel nostro caso sono 5)
                //nel caso di un numero di sensori variabile bisogna pescare il numero di
                //sensori dalla classe Server
                angoliEulero.Add(new AngoloEulero[5]);
                //itero i sensori del campione che sto considerando
                for (int numSensore = 0; numSensore < 5; numSensore++)
                {
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

        public static void FunzioneCheElaboraIDati(Object obj)
        {
            ElaboraDati.graphAck = false;
            List<float[,]> window = (List<float[,]>)obj;
            if (Form1.info.InvokeRequired)
            {
                Form1.info.Invoke(new MethodInvoker(delegate { Form1.info.AppendText("\r\n\r\nTHREAD CHE LAVORA SU UNA FINSTRA DI " + window.Count + " CAMPIONI\r\n\r\n"); }));
            }

            modacc = modulation(window, 0, 0);
            modgiro = modulation(window, 0, 1);

            ElaboraDati.datiAggiornati = true;
            
            /* Se non è mai stato fatto partire il thread che gestisce lo faccio partire
             * Viene chiamato il metodo DisegnaSulGrafico() che automaticamente
             * inizializza la form dove verrà visualizzato il grafico se è il "primo avvio"
             */
            
            if (!ModaccGraphThreadStarted)
                ModaccGraphThread.Start();
        }

        /* Viene chiamata solo al primo avvio del thread che gestisce il grafico
         * e inizializza gli assi e le etichette del g. stesso
         */
        public static void InizializzaGrafico()
        {
            fGAcc = new FormGraph();
            fGAcc.InitGraph("Segmentazione", "tempo", "MODACC");
            fGAcc.Show();
        }

        // Questa fz. viene chiamata quando si lancia il thread che gestisce il grafico
        public static void DisegnaSulGrafico()
        {
            if (!ModaccGraphThreadStarted)
            {
                InizializzaGrafico();
                ModaccGraphThreadStarted = true;
            }
            fGAcc.DrawGraph(modacc);

            // Aspetto che mi vengano inviati nuovi dati
            ElaboraDati.datiAggiornati = false;
            while (!datiAggiornati && !datiFiniti)
            { }

            if (!datiFiniti)
                DisegnaSulGrafico();
            else
            {
                // Inizializzo per eventuale invio di nuovi pacchetti
                datiAggiornati = false;
                datiFiniti = false;

                Application.Run();
            }
        }

        public static void CalcoloGirata() 
        {
        }

        //in linea teorica per lo studio della girata ci basiamo sul sensore del bacino
        //nell' algoritmo fisso dunque come numero dal sensore qeullo 
        //qeullo che si trova sul bacino
        public static List<float>  FunzioneOrientamento(List<float[,]> samples)
        {
            int sensoreBacino = 2;
            List <float >angoliTheta = new List<float>();
            for (int i = 0; i < samples.Count; i++) { 
                float x = samples[i][sensoreBacino, 6];
                float y = samples[i][sensoreBacino, 7];
                float z = samples[i][sensoreBacino, 8];
                angoliTheta.Add((float)Math.Atan(x / z));
            }

            return angoliTheta;
        }
    
    }
}
