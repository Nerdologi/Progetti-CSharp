using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace ReaderFile1
{
    public partial class Form1 : Form
    {
        Person p = null;
        List<Person> persone=null;
        public Form1()
        {
            InitializeComponent();
            persone=new List<Person>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (StreamReader streamreader = new StreamReader("file.txt"))
            { 
                string line;
                
                line=streamreader.ReadLine();
                
                while (line != null )
                {
                    listView1.Items.Add(line);
                    string[] componenti=new string[4];
                    
                    char spazio=' ';
                    componenti=line.Split(spazio);
                    p = new Person();
                    p.nome = componenti[0];
                    p.cognome = componenti[1];
                    p.matricola = Convert.ToInt32(componenti[2]);
                    persone.Add(p);
                    line = streamreader.ReadLine();
                    
                }         
            }
            using (StreamWriter streamWriter = new StreamWriter("utenti.txt"))
            { 
                
                int i=1;
                string separatore = ",";
               
                foreach(Person p in persone){
                    streamWriter.Write(i);
                    streamWriter.Write(separatore);
                    streamWriter.Write(p.nome);
                    streamWriter.Write(separatore);
                    streamWriter.Write(p.cognome);
                    streamWriter.Write(separatore);
                    streamWriter.Write(p.matricola);
                    streamWriter.WriteLine();
                    i++;
                }
            
            }
        }
    
    }
    public class Person {

        string _nome, _cognome;
        int _matricola;

        public string nome
        {
            get
            {     return _nome; }
            set
            {     _nome = value; }
        }

        public string cognome
        {
            get
            { return _cognome; }
            set
            { _cognome = value; }
        }

        public int matricola
        {
            get
            { return _matricola; }
            set
            { _matricola = value; }
        }
    }
}
