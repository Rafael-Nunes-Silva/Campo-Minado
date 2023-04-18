using System;

class tableMinado
{
    const string title =    "   _____                              __  __ _                 _       \n" +
                            "  / ____|                            |  \\/  (_)               | |      \n" +
                            " | |     __ _ _ __ ___  _ __   ___   | \\  / |_ _ __   __ _  __| | ___  \n" +
                            " | |    / _` | '_ ` _ \\| '_ \\ / _ \\  | |\\/| | | '_ \\ / _` |/ _` |/ _ \\ \n" +
                            " | |___| (_| | | | | | | |_) | (_) | | |  | | | | | | (_| | (_| | (_) |\n" +
                            "  \\_____\\__,_|_| |_| |_| .__/ \\___/  |_|  |_|_|_| |_|\\__,_|\\__,_|\\___/ \n" +
                            "                       | |                                             \n" +
                            "                       |_|                                             \n\n";

    static void MainMenu()
    {
        Console.Clear();

        for (int i=0, j=0; i < title.Length; i++)
        {
            Console.ForegroundColor = Table.colors[j % Table.colors.Length];
            Console.Write(title[i]);

            if (title[i] != ' ')
                j++;
        }
        Console.ResetColor();
    }

    static void Main(string[] args)
    {
        MainMenu();

        Console.WriteLine("Em qual dificuldade deseja jogar?");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("0 - Fácil");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("1 - Normal");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("2 - Difícil");
        Console.ResetColor();
        Console.Write(": ");
        
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
        Console.WriteLine($"O jogo durou {table.elapsedTime} segundos e você usou {table.flags} bandeiras de {table.maxFlags}");
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