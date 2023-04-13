using System;

class tableMinado
{
    static void Main(string[] args)
    {
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
            gameStatus = table.Play(InputHandler.GetGameInput(table));

            table.Draw();
        } while (gameStatus == Table.GameStatus.PLAYING);
        return gameStatus;
    }
}