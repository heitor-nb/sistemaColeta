using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace TesteProjetoGrafos
{
    internal class Graph
    {
        private readonly int maxSize;
        private int nodeCount;
        private readonly int lixoPorLata = 5; // m^3 de lixo por lata **
        public Node[] Vetor { get; set; }
        public int[,] W { get; set; }

        public Retorno SP { get; set; } // shortest paths

        public Graph(int n)
        {
            Vetor = new Node[n];
            maxSize = n;
            nodeCount = 0;
            W = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    W[i, j] = -1;
                }
            }
        }

        public void ExibirNodeCount() => Console.WriteLine($"Node count: {nodeCount}");

        public void AddNode(string str)
        {

            if (nodeCount < maxSize)
            {
                var node = new Node(str) // organizar esse lógica
                {
                    Index = int.Parse(str),
                };
                var index = node.Index;
                if(index == 0) node.LatasLixo = 0; // **
                Vetor[index] = node;
                W[index, index] = 0;
                nodeCount++;
                Console.WriteLine("Adicionado.");
            }
            else Console.WriteLine("Vetor cheio.");
        }

        public void AddVizinho(int i, int j, int peso)
        {
            if (i < nodeCount && j < nodeCount)
            {
                Vetor[i].N.Add(Vetor[j]); // *
                Vetor[j].N.Add(Vetor[i]);
                W[i, j] = peso;
                W[j, i] = peso;
                Console.WriteLine("Vizinho adicionado.");
            }
            else Console.WriteLine("Algum ou ambos vértices não existem.");
        }

        public void ExibirPesos()
        {
            for (int i = 0; i < maxSize; i++)
            {
                for (int j = 0; j < maxSize; j++)
                {
                    if (W[i, j] == -1) Console.Write("-   ");
                    else Console.Write($"{W[i, j]}   ");
                }
                Console.WriteLine();
            }
        }

        public void ExibirVizinhos()
        {
            foreach (var node in Vetor)
            {
                Console.Write($"{node.Symbol}: ");
                foreach (var v in node.N) Console.Write($"{v.Symbol}, ");
                Console.WriteLine();
            }
        }

        public void ExibirLixo()
        {
            Console.WriteLine("Lixo dos pontos:");
            foreach (var node in Vetor)
            {
                Console.WriteLine($"{node.Index}: {node.LatasLixo * lixoPorLata}");
            }
        }

        public void ExibirAnimais()
        {
            Console.WriteLine("Animais dos pontos:");
            foreach (var node in Vetor)
            {
                Console.WriteLine($"{node.Index}: rato - {node.Rato} | gato - {node.Gato} | cachorro - {node.Cachorro}");
            }
        }

        public void DistribuirAnimais(int i)
        {
            var node = Vetor[i];
            if (node.Gato > 0)
            {
                if (node.Rato > 0)
                {
                    var destino = SP.Valor[node.Index].FirstOrDefault(kvp => Vetor[kvp.Key].Gato == 0);
                    //Console.WriteLine(destino.ToString());
                    if (destino.Key != 0)
                    {
                        Vetor[destino.Key].Rato += node.Rato - 1; // um rato morre
                        node.Rato = 0;
                    }
                    //else Console.WriteLine("Tem gato em todos os pontos");
                }
                if (node.Cachorro > 0)
                {
                    var destino = SP.Valor[node.Index].FirstOrDefault(kvp => Vetor[kvp.Key].Cachorro == 0);
                    //Console.WriteLine(destino.ToString());
                    if (Vetor[destino.Key] != null)
                    {
                        Vetor[destino.Key].Gato += node.Gato;
                        node.Gato = 0;
                    }
                    //else Console.WriteLine("Tem cachorro em todos os pontos");
                }
            }
        }

        public void Percurso(char symbol, int v, ConcurrentDictionary<int, bool> visitados)
        {
            if(v < nodeCount && nodeCount > 0)
            {
                var truck = new Truck();
                Console.WriteLine($"Capacidade do caminhão {symbol}: {truck.Capacidade}");
                //var aux = visitados.Count < 8 && !visitados.Values.Contains(false);
                var aux = true;
                while (aux)
                {
                    var node = Vetor[v];

                    if(v != 0)
                    {
                        Console.WriteLine($"({symbol}) Recolhendo lixo do ponto {v}");

                        int tempo = 1;

                        if (node.PossuiAnimal)
                        {
                            tempo = 2;
                            DistribuirAnimais(v);

                            Console.WriteLine($"({symbol}) Tempo dobrado em {v} pois lixo espalhado");
                        }

                        Thread.Sleep(node.LatasLixo * lixoPorLata * 100 * tempo); // tempo para recolher o lixo

                        var lixoRestante = truck.RecolherLixo(node.LatasLixo * lixoPorLata);
                        node.LatasLixo = (int)Math.Floor((decimal)lixoRestante / lixoPorLata); // **
                        Console.WriteLine($"({symbol}) Capacidade do caminhão: {truck.Capacidade}");

                        if (node.LatasLixo > 0)
                        {
                            visitados.TryRemove(v, out bool update); //visitados.TryUpdate(v, false, true);
                            // informa que ainda há lixo no ponto

                            Console.WriteLine($"({symbol}) Lixo no ponto {v}: {node.LatasLixo * lixoPorLata} - Tem lixo: {update}");
                        }

                        if (truck.Compactacoes == 2 && truck.Capacidade == 0)
                        {
                            Console.WriteLine($"({symbol}) Retornando ao aterro - Capacidade = {truck.Capacidade}");
                            Thread.Sleep(SP.Valor[v].FirstOrDefault(kvp => kvp.Key == 0).Value * 100); // tempo para retornar ao aterro
                            truck.Capacidade = 60;
                            truck.Compactacoes = 0;

                            v = 0;
                            node = Vetor[v];
                        }
                    }
                    else Console.WriteLine($"({symbol}) Caminhão no aterro. Ponto {v} - " +
                        $"aux = {visitados.Count}");

                    var ponto = SP.Valor[v][1].Key; // a primeira chave da lista sempre será o próprio v
                    int i = 2;

                    //while i < 8 && (nao foi visitado ainda ou ainda tem lixo)
                    while (i <= 8 && !visitados.TryAdd(ponto, true))//&& !visitados.TryUpdate(ponto, true, false))
                    {
                        if(i < 8) ponto = SP.Valor[v][i].Key;
                        i++;
                    } // **

                    if (i < 8)
                    {
                        Console.WriteLine($"({symbol}) Indo de {v} para {ponto}");
                        Thread.Sleep(SP.Valor[v].FirstOrDefault(kvp => kvp.Key == ponto).Value * 100); // tempo para chegar ao ponto

                        v = ponto;
                    }
                    else
                    {
                        aux = visitados.Count < 8; //visitados.Values.Contains(false);
                        if (!aux)
                        {
                            Console.WriteLine($"({symbol}) Nenhum ponto a ser visitado\n" +
                            $"({symbol}) Retornando ao aterro - Capacidade = {truck.Capacidade}");
                            Thread.Sleep(SP.Valor[v].FirstOrDefault(kvp => kvp.Key == 0).Value * 100); // tempo para retornar ao aterro
                        }
                    }
                }
            }
        }

        //public void PercusoCarrocinha(int destino, ConcurrentDictionary<int, bool> visitados)
        //{
        //    var path = SP.Caminhos[7, destino];
        //    int i = 0;
        //    while(path[i] != Vetor[destino])
        //    {
        //        Thread.Sleep(SP.Valor[i][i + 1].Value * 100); // tempo para chegar ao prox ponto do caminho
        //        i++;
        //        if (Vetor[i].Gato > 0)
        //    }
        //}

        public Retorno FloydWarshall()
        {
            var dist = new int[maxSize, maxSize];
            var prev = new Node?[maxSize, maxSize];

            for (int i = 0; i < maxSize; i++)
            {
                for (int j = 0; j < maxSize; j++)
                {
                    if (W[i, j] != -1)
                    {
                        dist[i, j] = W[i, j];
                        prev[i, j] = Vetor[i];
                    }
                    else
                    {
                        dist[i, j] = 1024;
                        prev[i, j] = null;
                    }
                }
            }

            for (int k = 0; k < maxSize; k++)
            {
                for (int i = 0; i < maxSize; i++)
                {
                    for (int j = 0; j < maxSize; j++)
                    {
                        if (dist[i, j] > dist[i, k] + dist[k, j])
                        {
                            dist[i, j] = dist[i, k] + dist[k, j];
                            prev[i, j] = prev[k, j];
                        }
                    }
                }
            }

            var arr = new List<KeyValuePair<int, int>>[maxSize];

            for(int u = 0; u < maxSize; u++)
            {
                arr[u] = new();
                for(int v = 0; v < maxSize; v++)
                {
                    arr[u].Add(new KeyValuePair<int, int>(v, dist[u, v]));
                }
                arr[u] = arr[u].OrderBy(kvp => kvp.Value).ToList();
            }

            // monta matriz de caminhos:
            var paths = new List<Node>[maxSize, maxSize];
            for (int u = 0; u < maxSize; u++)
            {
                for (int v = 0; v < maxSize; v++)
                {
                    paths[u, v] = Path(prev, u, v);
                    paths[u, v].Reverse();
                }
            }

            return new Retorno(arr, paths);
        }

        private List<Node> Path(Node?[,] prev, int u, int v)
        {
            var path = new List<Node>();
            if (prev[u, v] == null) return path;
            else
            {
                var aux = Vetor[v];
                path.Add(aux);
                while (Vetor[u] != aux)
                {
                    aux = prev[u, aux.Index];
                    path.Add(aux); // **
                }
                return path;
            }
        }

        public void ExibirCaminhosMinimos()
        {
            var paths = FloydWarshall().Caminhos;
            for (int i = 0; i < maxSize; i++)
            {
                for (int j = 0; j < maxSize; j++)
                {
                    Console.Write($"{i} -> {j}: ");
                    foreach (var node in paths[i, j]) Console.Write($"{node.Index} ");
                    Console.WriteLine();
                }
            }
        }

        public void ExibirDistancias()
        {
            for(int u = 0; u < maxSize; u++)
            {
                Console.WriteLine($"{u}: ");
                for(int v = 0; v < maxSize; v++)
                {
                    Console.WriteLine($"({SP.Valor[u][v].Key} - {SP.Valor[u][v].Value}), ");
                }
                Console.WriteLine();
            }
        }
    }
}
