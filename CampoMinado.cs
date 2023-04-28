using System;

class CampoMinado
{
    static bool exit = false;
    const string title =    "   _____                              __  __ _                 _       \n" +
                            "  / ____|                            |  \\/  (_)               | |      \n" +
                            " | |     __ _ _ __ ___  _ __   ___   | \\  / |_ _ __   __ _  __| | ___  \n" +
                            " | |    / _` | '_ ` _ \\| '_ \\ / _ \\  | |\\/| | | '_ \\ / _` |/ _` |/ _ \\ \n" +
                            " | |___| (_| | | | | | | |_) | (_) | | |  | | | | | | (_| | (_| | (_) |\n" +
                            "  \\_____\\__,_|_| |_| |_| .__/ \\___/  |_|  |_|_|_| |_|\\__,_|\\__,_|\\___/ \n" +
                            "                       | |                                             \n" +
                            "                       |_|                                             \n\n";

    static void PrintTitle()
    {
        for (int i = 0, j = 0; i < title.Length; i++)
        {
            Console.ForegroundColor = Table.colors[j % Table.colors.Length];
            Console.Write(title[i]);

            if (title[i] != ' ')
                j++;
        }
        Console.ResetColor();
    }

    static void MainMenu()
    {
        int mode = 0;
        // Console.Write("Qual modo deseja jogar?\n0 - Um jogador\n1 - Multiplayer\n: ");
        Console.Write("Menu Principal\n0 - Sair\n1 - Um Jogador\n2 - Multiplayer\n: ");
        try { mode = int.Parse(Console.ReadLine()); }
        catch (Exception e)
        {
            MainMenu();
            return;
        }

        switch (mode)
        {
            case 0:
                exit = true;
                return;
            case 1:
                Singleplayer();
                break;
            case 2:
                Multiplayer();
                break;
        }
    }

    static void Singleplayer()
    {
        Table table = new Table(InputHandler.GetDifficulty());
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
        Console.Write("Enter para continuar");
        Console.ReadLine();
    }

    static void Multiplayer()
    {
        Console.Write("Insira o IP do servidor: ");
        string ip = Console.ReadLine();

        int port = 6778;
        Console.Write("Insira a porta do servidor: ");
        try { port = int.Parse(Console.ReadLine()); }
        catch (Exception e) {
            Console.WriteLine("Valor invalido");
            Multiplayer();
        }

        Console.Write("Insira como quer ser chamado: ");
        string name = Console.ReadLine();

        while (!Connector.Connect(ip, port, name))
        {
            Console.Write("Deseja tentar conectar novamente?(Y/n): ");
            if (Console.ReadLine().ToUpper() != "Y")
                return;
        }

        Console.WriteLine("Conexão bem sucedida");

        while (Connector.IsConnected())
        {
            Console.Clear();
            Console.WriteLine(Connector.GetRooms());

            Console.WriteLine("0 - Desconectar");
            Console.WriteLine("1 - Mostrar salas");
            Console.WriteLine("2 - Criar sala");
            Console.WriteLine("3 - Conectar a sala");
            Console.Write(": ");

            try
            {
                int input = int.Parse(Console.ReadLine());
                string roomName = "";

                switch (input)
                {
                    case 0:
                        Connector.Disconnect();
                        break;
                    case 1:
                        break;
                    case 2:
                        Console.Write("Insira o nome da sala: ");
                        roomName = Console.ReadLine();

                        int maxPlayers = 2;
                        Console.Write("Insira o número máximo de jogadores: ");
                        try { maxPlayers = int.Parse(Console.ReadLine()); }
                        catch (Exception e)
                        {
                            Console.WriteLine("Valor invalido");
                            break;
                        }

                        Connector.CreateRoom(roomName, maxPlayers);
                        Connector.EnterRoom(roomName);
                        break;
                    case 3:
                        Console.Write("Insira o nome da sala: ");
                        Connector.EnterRoom(Console.ReadLine());
                        break;
                }
            }
            catch (Exception e) { continue; }
        }

        Console.WriteLine("Conexão terminada");

        Console.Write("Enter para continuar");
        Console.ReadLine();
    }

    static void Main(string[] args)
    {
        Console.Clear();
        PrintTitle();
        MainMenu();

        /*
        Table table = new Table(InputHandler.GetDifficulty());
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
        */

        if (!exit)
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