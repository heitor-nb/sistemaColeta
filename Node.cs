using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteProjetoGrafos
{
    internal class Node
    {
        public int Index { get; set; } = -1;
        public string Symbol { get; set; }
        public int LatasLixo { get; set; }
        public int Rato { get; set; }
        public int Gato { get; set; }
        public int Cachorro { get; set; }
        public bool PossuiAnimal => Rato > 0 || Gato > 0 || Cachorro > 0;

        public List<Node> N { get; set; }
        public Node(string str)
        {
            Symbol = str;
            N = new List<Node>();
            var random = new Random();
            LatasLixo = random.Next(1, 11); // entre 1 e 10 latas por ponto
            // LatasLixo = n;
            Rato = random.Next(2) == 1 ? 1 : 0;
            Gato = random.Next(4) == 3 ? 1 : 0;
            Cachorro = random.Next(10) == 9 ? 1 : 0;
        }
    }
}
