using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteProjetoGrafos
{
    internal class Truck
    {
        public int Capacidade { get; set; } = 60; // 60m^3
        public int Compactacoes { get; set; } = 0;
        public int Funcionarios { get; set; } = 3;

        public int RecolherLixo(int qtd)
        {
            var diff = Capacidade - qtd;
            if (diff > 0)
            {
                Capacidade -= qtd;
                return 0; // retorna a quantidade de lixo que sobrou
            }
            else // diff <= 0
            {
                Capacidade = 0;
                if (Compactacoes == 2) return Math.Abs(diff); // caminhão cheio
                else
                {
                    if (Compactacoes == 0) Capacidade = 40;
                    else Capacidade = 20;
                    Compactacoes++;
                    return RecolherLixo(Math.Abs(diff));
                }
            }
        }
    }
}
