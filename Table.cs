using System;
using System.Collections.Generic;



public class Table
{
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

    Cell[][] cells;
    Difficulty difficulty;
    public uint numOfColumns = 26, numofLines = 26;

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
        switch (difficulty)
        {
            case Difficulty.FACIL:
                numOfColumns = 5;
                numofLines = 5;
                break;
            case Difficulty.NORMAL:
                numOfColumns = 14;
                numofLines = 14;
                break;
            case Difficulty.DIFICIL:
                numOfColumns = 26;
                numofLines = 26;
                break;
        }

        cells = new Cell[numOfColumns][];
        for (uint i = 0; i < numOfColumns; i++)
            cells[i] = new Cell[numofLines];
    }

    public void PlaceBombs(InputHandler.GameInput startPos)
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
        for (uint i = 0; i < numOfColumns; i++)
        {
            for (uint j = 0; j < numofLines; j++)
            {
                cells[i][j] = new Cell(true, r.Next() % chance == 0);
            }
        }

        cells[startPos.column][startPos.line].bomb = false;

        Play(startPos);
        OpenArea(startPos.column, startPos.line, false);

        Draw();
    }

    public void Draw()
    {
        Console.Clear();
        Console.ResetColor();

        // Console.Write("   A B C D E F G H I J K L M N O P Q R S T U V W X Y Z\n");
        Console.Write("   A|B|C|D|E|F|G|H|I|J|K|L|M|N|O|P|Q|R|S|T|U|V|W|X|Y|Z|\n");

        for (uint i = 0; i < numOfColumns; i++)
        {
            Console.ResetColor();
            Console.Write($"{((i < 9) ? " " + (i + 1).ToString() : (i + 1).ToString())}|");
            for (uint j = 0; j < numofLines; j++)
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
                    uint vizinhos = CalcNeighbours(i, j);
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

    public GameStatus Play(InputHandler.GameInput pos)
    {
        cells[pos.column][pos.line].hidden = false;

        if (cells[pos.column][pos.line].bomb)
        {
            ShowBombs();
            return GameStatus.PERDEU;
        }
        else if (CalcNeighbours(pos.column, pos.line) == 0)
        {
            OpenArea(pos.column, pos.line);
        }

        if (CheckVictory())
            return GameStatus.VENCEU;
        return GameStatus.JOGANDO;
    }

    bool CheckVictory()
    {
        for (uint i = 0; i < numOfColumns; i++)
        {
            for (uint j = 0; j < numofLines; j++)
            {
                if (!cells[i][j].bomb && cells[i][j].hidden)
                    return false;
            }
        }
        return true;
    }

    void OpenArea(uint posX, uint posY, bool pararNum = true)
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
                if (!pararNum || CalcNeighbours((uint)(posX + x), (uint)(posY + y)) == 0)
                    OpenArea((uint)(posX + x), (uint)(posY + y));
            }
        }
    }

    uint CalcNeighbours(uint CellX, uint CellY)
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

    public void ShowBombs()
    {
        for (uint i = 0; i < numOfColumns; i++)
        {
            for (uint j = 0; j < numofLines; j++)
            {
                if (cells[i][j].bomb)
                    cells[i][j].hidden = false;
            }
        }
        Draw();
    }
}