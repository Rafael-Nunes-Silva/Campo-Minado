using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class tableMinado
{
    static KeyValuePair<string, uint> GetInputs()
    {
        Console.ResetColor();

        Console.Write("Insira a coluna: ");
        string coluna = Console.ReadLine().ToUpper();
        
        if (int.TryParse(coluna, out int i) || coluna.Length > 1 || !Regex.IsMatch(coluna, @"^[A-Z]+$")){
            Console.WriteLine("****************************\nCOLUNA PRECISA SER UMA LETRA\n****************************");
            return GetInputs();
        }
        
        Console.Write("Insira a linha: ");
        uint linha = 0;
        try
        {
            linha = Convert.ToUInt32(Console.ReadLine());
            if (linha < 1 || linha > 26)
            {
                Console.WriteLine("****************************************\nLINHA PRECISA SER UM NÚMERO ENTRE 1 E 26\n****************************************");
                return GetInputs();
            }
        }
        catch
        {
            Console.WriteLine("****************************************\nLINHA PRECISA SER UM NÚMERO ENTRE 1 E 26\n****************************************");
            return GetInputs();
        }

        return new KeyValuePair<string, uint>(coluna, linha - 1);
    }

    static void Main(string[] args)
    {
        Console.Write("Em qual difficulty deseja jogar?\n0 - Fácil\n1 - Normal\n2 - Difícil\n: ");
        Difficulty difficulty = Difficulty.NORMAL;
        try
        {
            difficulty = (Difficulty)Convert.ToInt32(Console.ReadLine());
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

        table.PlaceBombs(GetInputs());
        GameStatus gameStatus = GameLoop(table);

        if (gameStatus == GameStatus.VENCEU)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Parabens! Você venceu!");
        }
        else if (gameStatus == GameStatus.PERDEU)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Você perdeu :(");
        }

        Console.ResetColor();
        Console.Write("Quer tentar de novo?(Y/n): ");
        if (Console.ReadLine().ToUpper() == "Y")
            Main(args);
    }

    static GameStatus GameLoop(Table table)
    {
        GameStatus gameStatus = GameStatus.JOGANDO;
        do
        {
            gameStatus = table.Play(GetInputs());

            table.Draw();
        } while (gameStatus == GameStatus.JOGANDO);
        return gameStatus;
    }
}