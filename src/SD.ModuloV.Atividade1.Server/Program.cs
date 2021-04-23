using SD.ModuloV.Atividade1.Server;
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

TcpListener listener = new(address, port);

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
                stringBuilder.AppendLine("Atividade 1");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Pretende-se construir uma aplicação distribuída cliente/servidor cuja comunicação será feita através de RPC. O processo Servidor recebe um valor do tipo char que identifica o tipo de cliente que estar comunicando com ele.");
                stringBuilder.AppendLine("• Clientes do tipo ‘A’, depois de enviarem ao servidor o seu tipo, enviam um valor inteiro e recebem como resposta o quadrado desse valor.");
                stringBuilder.AppendLine("• Clientes do tipo ‘B’, depois de enviarem ao servidor o seu tipo, enviam um valor do tipo double e recebem como resposta a raiz quadrada desse valor.");
                stringBuilder.AppendLine("- Construa as classes Servidor, Cliente_A e Cliente_B de forma que os clientes possam fazer vários pedidos ao servidor até decidirem terminar. Escolha um critério de paragem.");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Qual é o seu tipo de Cliente? \"A\" ou \"B\"?");

                binaryWriter.Write(stringBuilder.ToString());

                ICliente cliente = binaryReader.ReadString().ToUpper() switch
                {
                    "A" => new Cliente_A(),
                    "B" => new Cliente_B(),
                    _ => throw new NotSupportedException()
                };

                cliente.HandleConnection(tcpClient, binaryReader, binaryWriter);
            }
            catch (ApplicationException)
            {
                binaryWriter.Write(string.Empty);
            }
            catch (NotSupportedException)
            {
                Console.Write($"[{Thread.CurrentThread.Name}] Cliente não suportado. Conexão encerrada...");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            finally
            {
                tcpClient.Close();
                tcpClient.Dispose();
            }
        }
    });

    thread.Name = id.ToString().PadLeft(3, '0');
    thread.Start(client);

    id++;
}