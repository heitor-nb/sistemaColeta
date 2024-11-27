using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteProjetoGrafos
{
    internal class Retorno
    {
        public List<KeyValuePair<int, int>>[] Valor { get; set; }
        public List<Node>[,] Caminhos { get; set; }

        public Retorno(List<KeyValuePair<int, int>>[] valor, List<Node>[,] caminhos)
        {
            Valor = valor;
            Caminhos = caminhos;
        }
    }
}
