using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TesteProjetoGrafos
{
    internal static class Contador
    {
        public static async Task StartCounterAsync(CancellationToken cancellationToken)
        {
            int minutos = 0;
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    //Console.WriteLine($"Contador: {minutos} minutos");
                    minutos++;
                    await Task.Delay(100, cancellationToken); // espera 1 minuto segundo, respeitando o token de cancelamento
                }
            }
            catch (TaskCanceledException)
            {
                // captura o cancelamento se necessário
                Console.WriteLine($"Contador foi cancelado.\nMinutos totais: {minutos}");
            }
        }
    }
}
