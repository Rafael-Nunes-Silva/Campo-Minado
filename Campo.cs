using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Dificuldade
{
    FACIL,
    NORMAL,
    DIFICIL
}

enum Celula
{
    VAZIA,
    BOMBA
}

public class Campo
{
    Celula[][] celulas;

    public Campo(Dificuldade dificuldade)
    {
        celulas = new Celula[26][];
        for (uint i = 0; i < 26; i++)
            celulas[i] = new Celula[26];

        Random r = new Random();
        for (uint i = 0; i < 26; i++)
        {
            for (uint j = 0; j < 26; j++)
            {
                if (r.Next() % 10 == 0)
                    celulas[i][j] = Celula.BOMBA;
            }
        }   

        switch (dificuldade)
        {
            default:
                return;
        }
    }

    public void Desenhar()
    {
        Console.Clear();
        Console.ResetColor();

        Console.Write("   A B C D E F G H I J K L M N O P Q R S T U V W X Y Z|\n");

        for(uint i = 0; i < 26; i++)
        {
            Console.Write($"{((i < 10) ? " " + (i+1).ToString() : (i+1).ToString())} ");
            for(uint j=0; j < 26; j++)
            {
                if (celulas[i][j] == Celula.VAZIA)
                    Console.Write(" ");
                else if (celulas[i][j] == Celula.BOMBA)
                    Console.Write("@");
                Console.Write(" ");
                // Console.Write(celulas[i][j]);
            }
            Console.Write("\n");
        }

        Console.Write("\n");
        Console.WriteLine(" 1 @ @ @     @ @");
        Console.WriteLine(" 2   @     @ @ @");
        Console.WriteLine(" 3     @ @   @ @");
    }
}