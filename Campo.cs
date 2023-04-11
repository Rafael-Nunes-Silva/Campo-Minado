using System;
using System.Collections.Generic;

public enum GameStatus
{
    JOGANDO,
    VENCEU,
    PERDEU
}

public enum Difficulty
{
    FACIL = 0,
    NORMAL = 1,
    DIFICIL = 2
}

struct Cell
{
    public bool hidden, bomb;
    public Cell(bool hidden, bool bomb)
    {
        this.hidden = hidden;
        this.bomb = bomb;
    }
}

public class Table
{
    Cell[][] cells;
    Difficulty difficulty;
    
    static readonly Dictionary<string, uint> letterIndexMap = new Dictionary<string, uint>
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
    static readonly ConsoleColor[] colors =
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

    public Table(Difficulty difficulty = Difficulty.NORMAL)
    {
        this.difficulty = difficulty;

        cells = new Cell[26][];
        for (uint i = 0; i < 26; i++)
            cells[i] = new Cell[26];
    }

    public void PlaceBombs(KeyValuePair<string, uint> posicaoInicial)
    {
        uint chance = 1;
        switch (difficulty)
        {
            case Difficulty.FACIL:
                chance = 10;
                break;
            case Difficulty.NORMAL:
                chance = 6;
                break;
            case Difficulty.DIFICIL:
                chance = 4;
                break;
        }

        Random r = new Random(DateTime.Now.Second);
        for (uint i = 0; i < 26; i++)
        {
            for (uint j = 0; j < 26; j++)
            {
                cells[i][j] = new Cell(true, r.Next() % chance == 0);
            }
        }

        cells[posicaoInicial.Value][letterIndexMap[posicaoInicial.Key]].bomb = false;

        Play(posicaoInicial);
        AbrirArea(posicaoInicial.Value, letterIndexMap[posicaoInicial.Key], false);

        Draw();
    }

    public void Draw()
    {
        Console.Clear();
        Console.ResetColor();

        // Console.Write("   A B C D E F G H I J K L M N O P Q R S T U V W X Y Z\n");
        Console.Write("   A|B|C|D|E|F|G|H|I|J|K|L|M|N|O|P|Q|R|S|T|U|V|W|X|Y|Z|\n");

        for(uint i = 0; i < 26; i++)
        {
            Console.ResetColor();
            Console.Write($"{((i<9)?" "+(i+1).ToString():(i+1).ToString())}|");
            for(uint j=0; j < 26; j++)
            {
                Console.ForegroundColor = colors[0];
                if (cells[i][j].hidden)
                    Console.Write(" ");
                else if (cells[i][j].bomb)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("@");
                }
                else
                {
                    uint vizinhos = CalcularVizinhos(i, j);
                    Console.ForegroundColor = colors[vizinhos];

                    if (vizinhos > 0)
                        Console.Write($"{vizinhos}");
                    else Console.Write("#");
                }
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("|");
                // Console.Write(cells[i][j]);
            }
            Console.Write("\n");
        }
    }

    public GameStatus Play(KeyValuePair<string, uint> posicao)
    {
        cells[posicao.Value][letterIndexMap[posicao.Key]].hidden = false;

        if(cells[posicao.Value][letterIndexMap[posicao.Key]].bomb)
        {
            Mostrarbombs();
            return GameStatus.PERDEU;
        }
        else if (CalcularVizinhos(posicao.Value, letterIndexMap[posicao.Key]) == 0)
        {
            AbrirArea(posicao.Value, letterIndexMap[posicao.Key]);
        }

        if (ChecaVitoria())
            return GameStatus.VENCEU;
        return GameStatus.JOGANDO;
    }

    bool ChecaVitoria()
    {
        for(uint i=0; i < 26; i++)
        {
            for(uint j = 0; j < 26; j++)
            {
                if (!cells[i][j].bomb && cells[i][j].hidden)
                    return false;
            }
        }
        return true;
    }

    void AbrirArea(uint posX, uint posY, bool pararNum=true)
    {
        for (int x = -1; x <= 1; x++)
        {
            if (posX + x < 0 || posX + x > 25)
                continue;
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                if (posY + y < 0 || posY + y > 25)
                    continue;

                if (!cells[posX + x][posY + y].hidden || cells[posX + x][posY + y].bomb)
                    continue;

                cells[posX + x][posY + y].hidden = false;
                if (!pararNum || CalcularVizinhos((uint)(posX + x), (uint)(posY + y)) == 0)
                    AbrirArea((uint)(posX + x), (uint)(posY + y));
            }
        }
    }

    uint CalcularVizinhos(uint CellX, uint CellY)
    {
        uint vizinhos = 0;

        for (int x = -1; x <= 1; x++)
        {
            if (CellX + x < 0 || CellX + x > 25)
                continue;
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                if (CellY + y < 0 || CellY + y > 25)
                    continue;

                if (cells[CellX + x][CellY + y].bomb)
                    vizinhos++;
            }
        }

        return vizinhos;
    }

    public void Mostrarbombs()
    {
        for(uint i=0; i < 26; i++)
        {
            for(uint j = 0; j < 26; j++)
            {
                if (cells[i][j].bomb)
                    cells[i][j].hidden = false;
            }
        }
        Draw();
    }
}
