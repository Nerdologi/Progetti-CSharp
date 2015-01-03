using System;
using System.Collections.Generic;

public class Buffer
{
    private List<float[,]> buffer;
    private int head;
    private int capacity;

    public Buffer(int size) 
    {
        buffer = new List<float[,]>(size);
        head = 0;
        capacity = size;
        float[,] vuoto = new float[5, 13];
        for (int i = 0; i < size; i++)
            buffer.Insert(head, vuoto);
    }

    public float[,] GetElementAtIndex(int index)
    {
        return buffer[index];  
    }

    public void InsertElement(float[,] value)
    {
        if (head >= this.capacity)
        {
            head = 0;
        }
        buffer[head] = value;
        head++;
    }

    public int Count() 
    {
        return head;
    }

    public int Capacity()
    {
        return this.capacity;
    }
    // CONTROLLARE ASSOLUTAMENTE TUTTI GLI INDICI
    /*la funzione permette di estrarre dal Buffer la finestra di campioni
     * che devono essere analizzati 
     */
    public List<float[,]> GetWindow(int index, int windowSize)
    {
        List<float[,]> window = new List<float[,]>();
        if (index - windowSize < 0)
        {
            int missing = this.capacity - (windowSize - index);
            for (int i = missing; i < this.capacity; i++)
            {
                window.Add(buffer[i]);
            }
           
            for (int i = 0; i < index; i++)
            {
                window.Add(buffer[i]);
            }
        }
        else
        {
            for (int i = index - windowSize; i <index ; i++)
            {
                window.Add(buffer[i]);
            }
        }
        return window;
    }

    public void Clear()
    {
        buffer.Clear();
        head = 0;
    }
}
