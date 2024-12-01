using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteProjetoGrafos
{
    internal class NodeAux
    {
        public int Rato { get; set; }
        public int Gato { get; set; }
        public int Cachorro { get; set; }
        public int LatasLixo { get; set; }

        public NodeAux(int rato, int gato, int cachorro, int latasLixo)
        {
            Rato = rato;
            Gato = gato;
            Cachorro = cachorro;
            LatasLixo = latasLixo;
        }
    }
}
