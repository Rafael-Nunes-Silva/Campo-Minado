using System;
using System.Text.RegularExpressions;

class tableMinado
{
    static void Main(string[] args)
    {
        /*
        Console.WriteLine(new Regex("^[0-9]?[0-9]").Match("15 -h").ToString());
        Console.Read();
        return;
        */

        Console.Write("Em qual dificuldade deseja jogar?\n0 - Fácil\n1 - Normal\n2 - Difícil\n: ");
        Table.Difficulty difficulty = Table.Difficulty.NORMAL;
        try
        {
            difficulty = (Table.Difficulty)Convert.ToInt32(Console.ReadLine());
        }
        catch
        {
            Main(args);
            return;
        }
        if ((int)difficulty < 0 || (int)difficulty > 2)
        {
            Main(args);
            return;
        }

        Table table = new Table(difficulty);
        table.Draw();

        Table.GameStatus gameStatus = GameLoop(table);

        if (gameStatus == Table.GameStatus.WON)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Parabens! Você venceu!");
        }
        else if (gameStatus == Table.GameStatus.LOST)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Você perdeu :(");
        }

        Console.ResetColor();
        Console.Write("Quer tentar de novo?(Y/n): ");
        if (Console.ReadLine().ToUpper() == "Y")
            Main(args);
    }

    static Table.GameStatus GameLoop(Table table)
    {
        Table.GameStatus gameStatus = Table.GameStatus.PLAYING;
        do
        {
            switch(InputHandler.GetGameInput(out string input))
            {
                case InputHandler.InputType.PLAY:
                    gameStatus = table.Play(InputHandler.GetInputCells(table, input));
                    break;
                case InputHandler.InputType.FLAG:
                    table.PutFlags(InputHandler.GetInputCells(table, input));
                    break;
                case InputHandler.InputType.HILIGHT:
                    table.HighlightLine(InputHandler.GetHighlightInputLine(table, input));
                    break;
            }

            table.Draw();
        } while (gameStatus == Table.GameStatus.PLAYING);
        return gameStatus;
    }
}