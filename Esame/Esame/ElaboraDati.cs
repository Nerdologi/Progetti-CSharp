using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esame
{
    class ElaboraDati
    {
       
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
        public static List<float> modulation(List<float[,]> samples , int _type)
        {
            float a0, a1, a2;
            float g0, g1, g2;
            List<float> valori = new List<float>();
            for (int i = 0; i < samples.Count; i++)
            {
                for (int numSensore = 0; numSensore < 5; numSensore++)
                {
                    if (_type == 0)
                    {
                        a0 = samples[i][numSensore, 0];
                        a1 = samples[i][numSensore, 1];
                        a2 = samples[i][numSensore, 2];
                        a0 = (float)Math.Pow(a0, 2);
                        a1 = (float)Math.Pow(a1, 2);
                        a2 = (float)Math.Pow(a2, 2);
                        valori[i] = (float)Math.Sqrt((double)a0 + (double)a1 + (double)a2);
                        
                    }
                    else
                    {
                        g0 = samples[i][numSensore, 0];
                        g1 = samples[i][numSensore, 1];
                        g2 = samples[i][numSensore, 2];
                        g0 = (float)Math.Pow(g0, 2);
                        g1 = (float)Math.Pow(g1, 2);
                        g2 = (float)Math.Pow(g2, 2);
                        valori[i] = (float)Math.Sqrt(g0 + g1 + g2);
                    
                    }
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
                summary+=module[i];
                mean=0;
                if (i < K && (module.Count() - i - 1) >= K)//Ipotizzo che io non abbia dietro all'i-esimo valore K valori ma davanti si
                {
                    for (j = i - 1; j >= 0; j--)
                        summary += module[j];
                    for (h = i + 1; h < i + K; h++)
                        summary += module[h];
                    mean = summary / ((K + i) + 1);
                }
                if(i >= K && (module.Count() - i - 1) < K)//Ipotizzo che io abbia dietro K valori ma davanti no
                {
                    for (j = i - 1; j >= (i - 1 - K); j--)
                        summary += module[j];
                    for (h = i + 1; h < module.Count(); h++)
                        summary += module[h];
                    mean = summary / (K + h + 1);
                }

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
                        K++;
                        j--;
                        h++;                
                    }
                    mean = summary / (2 * K + 1);

                  }
                //FINE CASISTICA GENERALE
               /* ////////////
                TODO ALTRE CASISTICHE
                *//////////////
                valori[i] = mean;
            }
            return valori;
        }
    }
}
