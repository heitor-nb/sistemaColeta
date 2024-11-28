using System.Collections.Concurrent;
using System.Security.Cryptography;
using TesteProjetoGrafos;

var endereco = "C:/Users/heito/source/repos/TesteProjetoGrafos/TesteProjetoGrafos/config.txt";
var fileReader = new FileReader(endereco);
var lista = fileReader.GetLinhas(); // pega apenas as linhas de configuração
foreach (var item in lista) Console.WriteLine(item);

try
{
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
            Console.WriteLine(aux);
            grafo.AddNode(aux);
            aux = string.Empty; // **
        }
        index++;
    }

    grafo.ExibirNodeCount();

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
            Console.WriteLine($"{p1} - {p2} - {peso}");
            grafo.AddVizinho(p1, p2, peso);
            virgula = 0;
        }
        index++;
    }

    Console.WriteLine($"Número de arestas: {count}");

    grafo.SP = grafo.FloydWarshall(); // **

    grafo.ExibirAnimais();
    for (int i = 1; i < n; i++) grafo.DistribuirAnimais(i);
    grafo.ExibirAnimais();

    //grafo.ExibirPesos();

    //grafo.ExibirVizinhos();

    //grafo.ExibirDistancias();

    //grafo.ExibirLixo();

    //grafo.ExibirCaminhosMinimos();

    // --------------------

    var cts = new CancellationTokenSource();

    var counterTask = Contador.StartCounterAsync(cts.Token);

    Console.WriteLine("Contador iniciado.");

    var alfabeto = "abcdefghijklmnopqrstuvwxyz";

    var carrocinhas = new ConcurrentQueue<Carrocinha>();

    for (int i = 0; i < 3; i++) carrocinhas.Enqueue(new Carrocinha(alfabeto[25 - i]));

    ConcurrentDictionary<int, bool> visitados = new();

    visitados.TryAdd(0, true); // marca o 0 (aterro) como visitado

    var tasks = new List<Task>();

    for (int i = 0; i < 5; i++)
    {
        var task = Task.Run(() =>
        {
            grafo.Percurso(alfabeto[i], 0, visitados, carrocinhas); // problema do símbolo errado
        });
        
        tasks.Add(task);

        Thread.Sleep(i * 200); // tempo de saída do aterro entre um caminhão e outro
    }

    await Task.WhenAll(tasks);

    Console.WriteLine("Percuso dos caminhões finalizados");

    // Sinaliza para o contador parar
    cts.Cancel();

    // Aguarda a finalização do contador
    await counterTask;

    Console.WriteLine("Contador finalizado.");

    // --------------------
}
catch (Exception ex)
{
    Console.WriteLine($"Erro: {ex.Message}");
}

//Task task1 = Task.Run(() => grafo.Percurso('a', 0, visitados));

//Task task2 = Task.Run(() =>
//{
//    Thread.Sleep(200); // representa que o caminhão dois saiu em segundo lugar
//    grafo.Percurso('b', 0, visitados);
//});

//Task task3 = Task.Run(() =>
//{
//    Thread.Sleep(400); // representa que o caminhão dois saiu em segundo lugar
//    grafo.Percurso('c', 0, visitados);
//});

//Task task4 = Task.Run(() =>
//{
//    Thread.Sleep(600); // representa que o caminhão dois saiu em segundo lugar
//    grafo.Percurso('d', 0, visitados);
//});

//var grafo = new Graph(8);
//grafo.AddNode('0');
//grafo.AddNode('1');
//grafo.AddNode('2');
//grafo.AddNode('3');
//grafo.AddNode('4');
//grafo.AddNode('5');
//grafo.AddNode('6');
//grafo.AddNode('7');

//grafo.AddVizinho(0, 1, 5);
//grafo.AddVizinho(0, 2, 4);
//grafo.AddVizinho(1, 3, 2);
//grafo.AddVizinho(2, 3, 3);
//grafo.AddVizinho(2, 5, 1);
//grafo.AddVizinho(3, 4, 1);
//grafo.AddVizinho(3, 6, 4);
//grafo.AddVizinho(3, 7, 7);
//grafo.AddVizinho(6, 7, 2);

//grafo.ExibirPesos();

//ConcurrentDictionary<int, bool> visitados = new();

//Task bfsTask1 = Task.Run(() => grafo.BFS(0, visitados));
//Task bfsTask2 = Task.Run(() => grafo.BFS(6, visitados));

//await Task.WhenAll(bfsTask1, bfsTask2);
