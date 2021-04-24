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
        Console.WriteLine($"[{Thread.CurrentThread.Name}] Cliente conectado. Aguardando dados...");

        if (@object is TcpClient { Connected: true } tcpClient)
        {
            using BinaryReader binaryReader = new(tcpClient.GetStream());
            using BinaryWriter binaryWriter = new(tcpClient.GetStream());

            try
            {
                StringBuilder stringBuilder = new();
                stringBuilder.AppendLine("Atividade 6");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Pretende-se construir uma aplicação distribuída cliente/servidor cuja comunicação será feita através de RPC. Faça uma aplicação distribuída para calcular a função estatística desvio padrão,, de cinco números.");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Insira os 5 valores separados por espaço: ");

                binaryWriter.Write(stringBuilder.ToString());

                while (tcpClient.Connected)
                {
                    string @string = binaryReader.ReadString();

                    if (string.IsNullOrEmpty(@string))
                    {
                        throw new ArgumentNullException(nameof(@string));
                    }

                    var input = @string.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    if (input.Length == 5)
                    {
                        Console.WriteLine($"[{Thread.CurrentThread.Name}] Dados recebidos com sucesso. Processando...");

                        var values = input
                            .Select(x => decimal.Parse(x))
                            .ToList();

                        var somatorioDosValores = values.Sum();
                        var mediaDosValores = values.Average();
                        var calculoDosValores = values
                            .Select(i => Math.Pow((double)(i - mediaDosValores), 2))
                            .Sum();
                        var mediaDoCalculo = calculoDosValores / input.Length;
                        var resultado = Math.Sqrt(mediaDoCalculo);

                        stringBuilder.Clear();
                        stringBuilder.Append("Valores recebidos: ");
                        values.ForEach(x =>
                        {
                            stringBuilder.Append($"{x} ");
                        });
                        stringBuilder.AppendLine();
                        stringBuilder.AppendLine($"Soma dos valores: {somatorioDosValores}");
                        stringBuilder.AppendLine($"Media dos valores: {mediaDosValores}");
                        stringBuilder.AppendLine("Passos do calculo:");
                        values.ForEach(x =>
                        {
                            stringBuilder.AppendLine($"({x} - {mediaDosValores})^2 = ");
                        });
                        stringBuilder.AppendLine();
                        values.ForEach(x =>
                        {
                            stringBuilder.AppendLine($"({x - mediaDosValores})^2 = {(decimal)Math.Pow((double)(x - mediaDosValores), 2)}");
                        });
                        stringBuilder.AppendLine();
                        values.ForEach(x =>
                        {
                            if (x == values.Last())
                            {
                                stringBuilder.Append($"{(decimal)Math.Pow((double)(x - mediaDosValores), 2)} = {(decimal)calculoDosValores}");
                            }
                            else
                            {
                                stringBuilder.Append($"{(decimal)Math.Pow((double)(x - mediaDosValores), 2)} + ");
                            }
                        });
                        stringBuilder.AppendLine();
                        stringBuilder.AppendLine($"√({(decimal)calculoDosValores} / {input.Length}) =");
                        stringBuilder.AppendLine($"√{(decimal)mediaDoCalculo} = {(decimal)resultado}");
                        stringBuilder.AppendLine();
                        stringBuilder.AppendLine($"O desvio padrão dos dados recebidos é: {(decimal)resultado}");

                        Console.WriteLine($"[{Thread.CurrentThread.Name}] Dados processados com sucesso. Enviando resposta...");

                        binaryWriter.Write(stringBuilder.ToString());
                    }
                    else
                    {
                        throw new ArgumentException("Valores informados são inválidos.");
                    }
                }
            }
            catch (ArgumentNullException)
            {
                binaryWriter.Write(string.Empty);
            }
            catch (ArgumentException)
            {
                binaryWriter.Write(string.Empty);
            }
            catch (ApplicationException)
            {
                binaryWriter.Write(string.Empty);
            }
            catch (FormatException)
            {
                binaryWriter.Write(string.Empty);
            }
            finally
            {
                tcpClient.Close();
                tcpClient.Dispose();

                Console.WriteLine($"[{Thread.CurrentThread.Name}] Conexão encerrada...");
            }
        }
    });

    thread.Name = id.ToString().PadLeft(3, '0');
    thread.Start(client);

    id++;
}