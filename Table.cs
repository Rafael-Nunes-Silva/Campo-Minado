using System;

public class Table
{
    public enum GameStatus
    {
        PLAYING,
        WON,
        LOST
    }

    public enum Difficulty
    {
        EASY = 0,
        NORMAL = 1,
        HARD = 2
    }

    struct Cell
    {
        public bool hidden, bomb, flag;
        public Cell(bool hidden, bool bomb)
        {
            this.hidden = hidden;
            this.bomb = bomb;
            this.flag = false;
        }
    }

    Cell[][] cells;
    public int numOfColumns = 26, numOfLines = 26;

    bool firstPlay = true;

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
        switch (difficulty)
        {
            case Difficulty.EASY:
                numOfColumns = 5;
                numOfLines = 5;
                break;
            case Difficulty.NORMAL:
                numOfColumns = 14;
                numOfLines = 14;
                break;
            case Difficulty.HARD:
                numOfColumns = 26;
                numOfLines = 26;
                break;
        }

        cells = new Cell[numOfColumns][];
        for (int i = 0; i < numOfColumns; i++)
            cells[i] = new Cell[numOfLines];
    }

    public void Draw()
    {
        Console.Clear();
        Console.ResetColor();

        Console.Write($"  |{"A|B|C|D|E|F|G|H|I|J|K|L|M|N|O|P|Q|R|S|T|U|V|W|X|Y|Z|".Substring(0, numOfColumns * 2)}\n");

        for (int i = 0; i < numOfLines; i++)
        {
            Console.ResetColor();
            Console.Write($"{((i < 9) ? " " + (i + 1).ToString() : (i + 1).ToString())}|");
            for (int j = 0; j < numOfColumns; j++)
            {
                Console.ForegroundColor = colors[0];
                if (cells[i][j].flag)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("&");
                }
                else if (cells[i][j].hidden)
                {
                    Console.Write(" ");
                }
                else if (cells[i][j].bomb)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("@");
                }
                else
                {
                    int vizinhos = CalcNeighbours(i, j);
                    Console.ForegroundColor = colors[vizinhos];

                    if (vizinhos > 0)
                        Console.Write($"{vizinhos}");
                    else Console.Write("#");
                }
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("|");
            }
            Console.Write("\n");
        }
    }

    public GameStatus Play(InputHandler.GameInput[] pos)
    {
        if (pos.Length == 0)
            return GameStatus.PLAYING;

        if (firstPlay)
        {
            PlaceBombs(pos[0]);
            firstPlay = false;
        }

        for (int i = 0; i < pos.Length; i++)
        {
            if (pos[i].flag)
            {
                cells[pos[i].line][pos[i].column].flag = !cells[pos[i].line][pos[i].column].flag;
                continue;
            }

            cells[pos[i].line][pos[i].column].hidden = false;
            if (cells[pos[i].line][pos[i].column].bomb)
            {
                ShowBombs();
                return GameStatus.LOST;
            }
            else if (CalcNeighbours(pos[i].line, pos[i].column) == 0)
            {
                OpenArea(pos[i].line, pos[i].column);
            }
        }

        if (CheckVictory())
        {
            ShowBombs();
            return GameStatus.WON;
        }
        return GameStatus.PLAYING;
    }

    void PlaceBombs(InputHandler.GameInput startPos)
    {
        Random r = new Random(DateTime.Now.Second);
        for (int i = 0; i < numOfLines; i++)
        {
            for (int j = 0; j < numOfColumns; j++)
            {
                cells[i][j] = new Cell(true, r.Next() % 4 == 0);
            }
        }

        cells[startPos.column][startPos.line].bomb = false;

        OpenArea(startPos.column, startPos.line, false);

        Draw();
    }

    bool CheckVictory()
    {
        for (int i = 0; i < numOfLines; i++)
        {
            for (int j = 0; j < numOfColumns; j++)
            {
                if (!cells[i][j].bomb && cells[i][j].hidden)
                    return false;
            }
        }
        return true;
    }

    void OpenArea(int posX, int posY, bool stopOnNum = true)
    {
        for (int x = -1; x <= 1; x++)
        {
            if (posX + x < 0 || posX + x >= numOfColumns)
                continue;
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                if (posY + y < 0 || posY + y >= numOfLines)
                    continue;

                if (!cells[posX + x][posY + y].hidden || cells[posX + x][posY + y].bomb)
                    continue;

                cells[posX + x][posY + y].hidden = false;
                if (!stopOnNum || CalcNeighbours(posX + x, posY + y) == 0)
                    OpenArea(posX + x, posY + y);
            }
        }
    }

    int CalcNeighbours(int cellX, int cellY)
    {
        int vizinhos = 0;

        for (int x = -1; x <= 1; x++)
        {
            if (cellX + x < 0 || cellX + x >= numOfColumns)
                continue;
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                if (cellY + y < 0 || cellY + y >= numOfLines)
                    continue;

                if (cells[cellX + x][cellY + y].bomb)
                    vizinhos++;
            }
        }

        return vizinhos;
    }

    public void ShowBombs()
    {
        for (int i = 0; i < numOfColumns; i++)
        {
            for (int j = 0; j < numOfLines; j++)
            {
                if (cells[i][j].bomb)
                    cells[i][j].hidden = false;
            }
        }
        Draw();
    }
}