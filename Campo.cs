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

struct Celula
{
    public bool escondido, bomba;
    public Celula(bool escondido, bool bomba)
    {
        this.escondido = escondido;
        this.bomba = bomba;
    }
}

public class Campo
{
    Celula[][] celulas;

    public Campo(Dificuldade dificuldade = Dificuldade.NORMAL)
    {
        celulas = new Celula[26][];
        for (uint i = 0; i < 26; i++)
            celulas[i] = new Celula[26];

        Random r = new Random(DateTime.Now.Second);
        for (uint i = 0; i < 26; i++)
        {
            for (uint j = 0; j < 26; j++)
            {
                celulas[i][j] = new Celula(true, r.Next() % 10 == 0);
            }
        }   

        switch (dificuldade)
        {
            default:
                return;
        }
    }

    static readonly Dictionary<string, uint> mapaLetraIndex = new Dictionary<string, uint>
    {
        { "A", 0 },
        { "B", 1 },
        { "C", 2 },
        { "D", 3 },
        { "E", 4 },
        { "F", 5 },
        { "G", 6 },
        { "H", 7 },
        { "I", 8 },
        { "J", 9 },
        { "K", 10 },
        { "L", 11 },
        { "M", 12 },
        { "N", 13 },
        { "O", 14 },
        { "P", 15 },
        { "Q", 16 },
        { "R", 17 },
        { "S", 18 },
        { "T", 19 },
        { "U", 20 },
        { "V", 21 },
        { "W", 22 },
        { "X", 23 },
        { "Y", 24 },
        { "Z", 25 }
    };

    static readonly ConsoleColor[] cores =
    {
        ConsoleColor.DarkGray,
        ConsoleColor.Green,
        ConsoleColor.DarkGreen,
        ConsoleColor.DarkYellow,
        ConsoleColor.Blue,
        ConsoleColor.DarkBlue,
        ConsoleColor.Magenta,
        ConsoleColor.DarkMagenta,
        ConsoleColor.DarkRed
    };

    public void Desenhar()
    {
        Console.Clear();
        Console.ResetColor();

        Console.Write("   A B C D E F G H I J K L M N O P Q R S T U V W X Y Z\n");

        for(uint i = 0; i < 26; i++)
        {
            Console.ResetColor();
            Console.Write($"{((i<9)?" "+(i+1).ToString():(i+1).ToString())} ");
            for(uint j=0; j < 26; j++)
            {
                Console.ForegroundColor = cores[0];
                if (celulas[i][j].escondido)
                    Console.Write("  ");
                else if (celulas[i][j].bomba)
                    Console.Write("@ ");
                else
                {
                    uint vizinhos = CalcularVizinhos(i, j);
                    Console.ForegroundColor = cores[vizinhos];

                    if (vizinhos > 0)
                        Console.Write($"{vizinhos} ");
                    else Console.Write("# ");
                }
                // Console.Write(celulas[i][j]);
            }
            Console.Write("\n");
        }
    }

    void Jogar(string coluna, uint linha)
    {

    }

    uint CalcularVizinhos(uint celulaX, uint celulaY)
    {
        uint vizinhos = 0;

        for (int x = -1; x <= 1; x++)
        {
            if (celulaX + x < 0 || celulaX + x > 25)
                continue;
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                if (celulaY + y < 0 || celulaY + y > 25)
                    continue;

                if (celulas[celulaX + x][celulaY + y].bomba)
                    vizinhos++;
            }
        }

        return vizinhos;
    }
}