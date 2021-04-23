using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

int port = 2021;

IPHostEntry entry = await Dns.GetHostEntryAsync(Dns.GetHostName());
IPAddress? address = entry.AddressList.FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork);

if (address is null)
{
    throw new NullReferenceException();
}

IPEndPoint endPoint = new(address, port);

Socket socketListener = new(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

socketListener.Bind(endPoint);
socketListener.Listen(10);

try
{
    while (true)
    {
        Console.WriteLine("Aguardando conexão...");

        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine("Atividade 3");
        stringBuilder.AppendLine();
        stringBuilder.AppendLine("Pretende-se construir uma aplicação distribuída cliente/servidor cuja comunicação será feita através de Sockets. Faça uma aplicação distribuída que escreva todos os números pares entre 1 e 1001 que não sejam múltiplos de 3 ou que não sejam múltiplos de 5.");
        stringBuilder.AppendLine();

        var socketClient = await socketListener.AcceptAsync();

        await socketClient.SendAsync(Encoding.UTF8.GetBytes(stringBuilder.ToString()), SocketFlags.None);

        var values = Enumerable.Range(1, 1001)
            .Where(x => x % 2 == 0)
            .Where(x => x % 3 != 0)
            .Where(x => x % 5 != 0)
            .ToList();

        stringBuilder.Clear();

        values.ForEach(x =>
        {
            stringBuilder.Append($"{x};");
        });

        await socketClient.SendAsync(Encoding.UTF8.GetBytes(stringBuilder.ToString()), SocketFlags.None);

        socketClient.Shutdown(SocketShutdown.Both);
        socketClient.Close();
    }
}
catch (Exception exception)
{
    Console.WriteLine(exception);
}