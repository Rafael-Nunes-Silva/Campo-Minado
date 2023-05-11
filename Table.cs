using System;

public class Table
{
    public enum GameStatus
    {
        NOT_PLAYING,
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
    public int maxFlags = 0, flags = 0;
    int highlitedLine = -1;
    DateTime startTime;
    public int elapsedTime = 0;

    bool firstPlay = true;
    int tableSeed = -1;

    Difficulty difficulty;

    public static readonly ConsoleColor[] colors =
    {
        ConsoleColor.DarkGray,
        ConsoleColor.Green,
        ConsoleColor.DarkGreen,
        ConsoleColor.DarkYellow,
        ConsoleColor.Cyan,
        ConsoleColor.Blue,
        ConsoleColor.Magenta,
        ConsoleColor.DarkMagenta,
        ConsoleColor.DarkRed
    };

    public Table(Difficulty difficulty = Difficulty.NORMAL, int tableSeed = -1)
    {
        this.tableSeed = tableSeed;
        this.difficulty = difficulty;
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
        Console.CursorVisible = false;

        elapsedTime = (int)(DateTime.Now - startTime).TotalSeconds;
        Console.WriteLine($"Bandeiras: {maxFlags - flags} | Tempo: {(elapsedTime < 0 ? 0 : elapsedTime)}s");
        Console.WriteLine($"  |{"A|B|C|D|E|F|G|H|I|J|K|L|M|N|O|P|Q|R|S|T|U|V|W|X|Y|Z|".Substring(0, numOfColumns * 2)}");

        for (int i = 0; i < numOfLines; i++)
        {
            Console.ResetColor();
            Console.Write($"{((i < 9) ? " " + (i + 1).ToString() : (i + 1).ToString())}|");

            if (i == highlitedLine)
                Console.BackgroundColor = ConsoleColor.White;
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

        Console.CursorVisible = true;
    }

    public GameStatus Play(InputHandler.InputCell[] pos)
    {
        if (pos.Length == 0)
            return GameStatus.PLAYING;

        if (firstPlay)
        {
            startTime = DateTime.Now;
            PlaceBombs(pos[0]);
            firstPlay = false;
        }

        for (int i = 0; i < pos.Length; i++)
        {
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

    public void PutFlags(InputHandler.InputCell[] pos)
    {
        for (int i = 0; i < pos.Length && flags < maxFlags; i++)
        {
            if (!cells[pos[i].line][pos[i].column].flag)
            {
                cells[pos[i].line][pos[i].column].flag = true;
                flags++;
            }
            else
            {
                cells[pos[i].line][pos[i].column].flag = false;
                flags--;
            }
        }
    }

    public void HighlightLine(int line)
    {
        if (highlitedLine == line)
            highlitedLine = -1;
        else highlitedLine = line;
    }

    void PlaceBombs(InputHandler.InputCell startPos)
    {
        Random rand = new Random(tableSeed < 0 ? DateTime.Now.Second : tableSeed);
        for (int i = 0; i < numOfLines; i++)
        {
            for (int j = 0; j < numOfColumns; j++)
            {
                cells[i][j] = new Cell(true, rand.Next() % 4 == 0);
                if (cells[i][j].bomb)
                    maxFlags++;
            }
        }
        maxFlags--;

        cells[startPos.line][startPos.column].bomb = false;

        OpenArea(startPos.line, startPos.column, (difficulty == Difficulty.EASY ? true : false));

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