using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteProjetoGrafos
{
    internal class FileReader
    {
        private string _endereco;

        public FileReader(string endereco) => _endereco = endereco;

        public List<string> GetLinhas()
        {
            var lista = new List<string>();
            try
            {
                using var fluxoDeArquivo = new FileStream(_endereco, FileMode.Open);
                var leitor = new StreamReader(fluxoDeArquivo);
                while (!leitor.EndOfStream)
                {
                    var linha = leitor.ReadLine();
                    if (!string.IsNullOrWhiteSpace(linha) && linha[0] == '-') lista.Add(linha); // *
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }
            return lista;
        }
    }
}
