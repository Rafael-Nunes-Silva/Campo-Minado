using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class CampoMinado
{
    static KeyValuePair<string, uint> PegarInputs()
    {
        Console.ResetColor();

        Console.Write("Insira a coluna: ");
        string coluna = Console.ReadLine().ToUpper();
        
        if (int.TryParse(coluna, out int i) || coluna.Length > 1 || !Regex.IsMatch(coluna, @"^[A-Z]+$")){
            Console.WriteLine("****************************\nCOLUNA PRECISA SER UMA LETRA\n****************************");
            return PegarInputs();
        }
        
        Console.Write("Insira a linha: ");
        uint linha = 0;
        try
        {
            linha = Convert.ToUInt32(Console.ReadLine());
            if (linha < 1 || linha > 26)
            {
                Console.WriteLine("****************************************\nLINHA PRECISA SER UM NÚMERO ENTRE 1 E 26\n****************************************");
                return PegarInputs();
            }
        }
        catch
        {
            Console.WriteLine("****************************************\nLINHA PRECISA SER UM NÚMERO ENTRE 1 E 26\n****************************************");
            return PegarInputs();
        }

        return new KeyValuePair<string, uint>(coluna, linha - 1);
    }

    static void Main(string[] args)
    {
        Console.Write("Em qual dificuldade deseja jogar?\n0 - Fácil\n1 - Normal\n2 - Difícil\n: ");
        uint dificuldade = 1;
        try
        {
            dificuldade = Convert.ToUInt32(Console.ReadLine());
        }
        catch
        {
            Main(args);
            return;
        }
        if (dificuldade < 0 || dificuldade > 2)
        {
            Main(args);
            return;
        }

        Campo campo = new Campo((Dificuldade)dificuldade);
        campo.Desenhar();

        campo.Preencher(PegarInputs());

        Status statusDoJogo = Status.JOGANDO;
        do
        {
            statusDoJogo = campo.Jogar(PegarInputs());
            
            campo.Desenhar();
        } while (statusDoJogo == Status.JOGANDO);

        if (statusDoJogo == Status.VENCEU)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Parabens! Você venceu!");
        }
        else if (statusDoJogo == Status.PERDEU)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Você perdeu :(");
        }

        Console.ResetColor();
        Console.Write("Quer tentar de novo?(Y/n): ");
        if (Console.ReadLine().ToUpper() == "Y")
            Main(args);
    }
}