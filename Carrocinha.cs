using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TesteProjetoGrafos
{
    internal class Carrocinha
    {
        public char Symbol { get; set; }
        public int Capacidade { get; set; } = 5;

        public Carrocinha(char symbol) => Symbol = symbol; 
    }
}
