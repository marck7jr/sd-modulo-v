using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

int port = 2021;

IPHostEntry entry = await Dns.GetHostEntryAsync("localhost");
IPAddress? address = entry.AddressList.FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork);

if (address is null)
{
    throw new NullReferenceException();
}

IPEndPoint endPoint = new(address, port);

TcpListener listener = new(endPoint);

listener.Start();

Console.WriteLine($"Servidor aguardando conexões => {listener.LocalEndpoint}");

int id = 1;

while (true)
{
    TcpClient client = await listener.AcceptTcpClientAsync();
    Thread thread = new(@object =>
    {
        if (@object is TcpClient { Connected: true } tcpClient)
        {
            using BinaryReader binaryReader = new(tcpClient.GetStream());
            using BinaryWriter binaryWriter = new(tcpClient.GetStream());

            try
            {
                StringBuilder stringBuilder = new();
                stringBuilder.AppendLine("Atividade 4");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Pretende-se construir uma aplicação distribuída cliente/servidor cuja comunicação será feita através de Sockets. A conversão de graus Fahrenheit para Centígrados é obtida por:");
                stringBuilder.AppendLine("C=5/9(F - 32)");
                stringBuilder.AppendLine("Faça uma aplicação distribuída que calcule e escreva uma tabela de centígrados em função de graus Fahrenheit, que variam de 50 a 150 de 1 em 1.");

                binaryWriter.Write(stringBuilder.ToString());


                var values = Enumerable.Range(50, 150)
                    .Select(x => new { Fahrenheit = x, Celsius = 5.0 / 9.0 * (x - 32) })
                    .ToList();

                stringBuilder.Clear();
                stringBuilder.AppendLine("Fahrenheit\t\tCelsius");
                stringBuilder.AppendLine("==========================================");
                values.ForEach(x =>
                {
                    stringBuilder.AppendLine($"{x.Fahrenheit}\t\t{x.Celsius}");
                });

                binaryWriter.Write(stringBuilder.ToString());
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            finally
            {
                tcpClient.Close();
            }
        }
    });
    thread.Name = id.ToString().PadLeft(3, '0');
    thread.Start(client);

    id++;
}