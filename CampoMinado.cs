using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class CampoMinado
{
    enum Celula
    {
        VAZIO,
        BOMBA
    }

    static void Main(string[] args)
    {
        Campo campo = new Campo(Dificuldade.NORMAL);

        campo.Desenhar();

        Console.Read();
    }

    
}