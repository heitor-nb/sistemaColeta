﻿using System.Collections.Concurrent;
using System.Security.Cryptography;
using TesteProjetoGrafos;

var endereco = "C:/Users/heito/source/repos/TesteProjetoGrafos/TesteProjetoGrafos/config.txt";
var fileReader = new FileReader(endereco);
var lista = fileReader.GetLinhas(); // pega apenas as linhas de configuração
foreach (var item in lista) Console.WriteLine(item);

try
{
    // ----- Monta o grafo a partir do arquivo de configuração -----

    var aux = string.Empty;
    var index = 1;
    while (lista[0][index] != ' ')
    {
        aux += lista[0][index];
        index++;
    }
    var n = int.Parse(aux);
    var grafo = new Graph(n);

    index = 1;
    aux = string.Empty;
    while(lista[1][index] != '-')
    {
        var c = lista[1][index];
        if (c != ',') aux += c;
        else
        {
            //Console.WriteLine(aux);
            grafo.AddNode(aux);
            aux = string.Empty; // **
        }
        index++;
    }

    //Console.WriteLine($"Número de nós: {grafo.NodeCount()}");

    var count = 0;
    index = 1;
    var virgula = 0;
    aux = string.Empty;
    int p1 = 0, p2 = 0, peso = 0; // **
    while (lista[2][index] != '-')
    {
        var c = lista[2][index];

        if (c != ',' && c != ' ') aux += c;
        else
        {
            var i = int.Parse(aux);
            aux = string.Empty;

            if (virgula == 0) p1 = i;
            if (virgula == 1) p2 = i;
            else peso = i;
            virgula++;
        }

        if (virgula == 3)
        {
            count++;
            //Console.WriteLine($"{p1} - {p2} - {peso}");
            grafo.AddVizinho(p1, p2, peso);
            virgula = 0;
        }
        index++;
    }

    //Console.WriteLine($"Número de arestas: {count}");

    // --------------------

    // Após adicionar os nós e estabelecer as relações de vizinhança,
    // chama o algoritmo de caminhos mínimos:
    grafo.SP = grafo.FloydWarshall();

    // Para cada nó do grafo, distribui os animais
    // ali presentes de acordo com a especificação:
    for (int i = 1; i < n; i++) grafo.DistribuirAnimais(i);

    // Monta vetor auxiliar:
    var arr = new NodeAux[grafo.NodeCount()];
    for(int i = 0; i < arr.Length; i++)
    {
        var node = grafo.Vetor[i];
        arr[i] = new NodeAux(node.Rato, node.Gato, node.Cachorro, node.LatasLixo);
    }

    // ----- Funcões para exibir as informações do grafo: -----

    //grafo.ExibirPesos(); // Matriz de pesos

    //grafo.ExibirVizinhos();

    grafo.ExibirLixo();

    grafo.ExibirAnimais();

    //grafo.ExibirDistancias(); // Valor das distâncias entre cada nó e os demais (ordem crescente)

    //grafo.ExibirCaminhosMinimos();

    // ----- Lógica para inicialização dos percusos dos caminhões: -----

    int tempo; // Guarda o tempo que levou para a coleta ser concluída
               // (se > 480, o algoritmo adiciona mais um caminhão e repete a coleta)

    int caminhoes = 0;

    var alfabeto = "abcdefghijklmnopqrstuvwxyz";

    // obs.: a convenção utilizada é que o aterro é o ponto 0
    //       e o centro de zoonoses é o último (nesse caso, 29).

    do
    {
        // A partir da segunda iteração do while, é necessário resetar as
        // informações dos nós, pois essas são alteradas durante o percuso.
        if (caminhoes != 0)
        {
            foreach(var node in grafo.Vetor)
            {
                var nodeAux = arr[node.Index];
                node.Rato = nodeAux.Rato;
                node.Gato = nodeAux.Gato;
                node.Cachorro = nodeAux.Cachorro;
                node.LatasLixo = nodeAux.LatasLixo;
            }
        }

        grafo.ExibirLixo();
        grafo.ExibirAnimais();

        var carrocinhas = new ConcurrentQueue<Carrocinha>();

        for (int i = 0; i < 3; i++) carrocinhas.Enqueue(new Carrocinha(alfabeto[25 - i]));

        ConcurrentDictionary<int, bool> visitados = new();

        visitados.TryAdd(0, true); // marca o 0 (aterro) como visitado

        var tasks = new List<Task>();

        // Inicia contador: -----

        var cts = new CancellationTokenSource();

        var counterTask = Contador.StartCounterAsync(cts.Token);

        Console.WriteLine("Contador iniciado.");

        // ----------

        caminhoes++;

        for (int i = 0; i < caminhoes; i++)
        {
            var task = Task.Run(() =>
            {
                grafo.Percurso(alfabeto[i], 0, visitados, carrocinhas); // !! problema do símbolo errado
            });

            tasks.Add(task);

            Thread.Sleep(i * 200); // tempo de saída do aterro entre um caminhão e outro
        }

        await Task.WhenAll(tasks);

        Console.WriteLine("Percuso dos caminhões finalizados");

        // Sinaliza para o contador parar
        cts.Cancel();

        // Aguarda a finalização do contador
        tempo = await counterTask;

        //Console.WriteLine("Contador finalizado.");
    }
    while (tempo > 480);

    Console.WriteLine("Encerrado");
    
    // --------------------
}
catch (Exception ex)
{
    Console.WriteLine($"Erro: {ex.Message}");
}
