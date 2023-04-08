using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class CampoMinado
{
    static KeyValuePair<string, uint> PegarInputs()
    {
        Console.Write("Insira a coluna: ");
        string coluna = Console.ReadLine().ToUpper();

        Console.Write("Insira a linha: ");
        uint linha = Convert.ToUInt32(Console.ReadLine());

        return new KeyValuePair<string, uint>(coluna, linha);
    }

    static void Main(string[] args)
    {
        Campo campo = new Campo(Dificuldade.NORMAL);

        campo.Desenhar();
        do
        {
            Console.ResetColor();
            try
            {
                campo.Jogar(PegarInputs());
            }
            catch
            {
                campo.Desenhar();
                continue;
            }

            campo.Desenhar();
        } while (true);

        Console.Read();
    }
}