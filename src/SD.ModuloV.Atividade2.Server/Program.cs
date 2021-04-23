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
                stringBuilder.AppendLine("Atividade 2");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Pretende-se construir uma aplicação distribuída cliente/servidor cuja comunicação será feita através de RPC. Faça uma aplicação distribuída que leia três valores reais a, b e c para verificar e escrever se eles podem ser valores dos lados de um triângulo e, se for, se é um triângulo qualquer, equilátero ou isósceles.");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Insira os valores das vértices A, B e C separados por espaço: ");

                binaryWriter.Write(stringBuilder.ToString());

                while (tcpClient.Connected)
                {
                    string @string = binaryReader.ReadString();

                    if (string.IsNullOrEmpty(@string))
                    {
                        throw new ArgumentNullException(nameof(@string));
                    }

                    var input = @string.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    if (input.Count() == 3)
                    {
                        var values = input.Select(x => int.Parse(x)).ToArray();

                        int a = values[0];
                        int b = values[1];
                        int c = values[2];

                        Console.WriteLine($"[{Thread.CurrentThread.Name}] Recebeu os valores: {a} {b} {c}");

                        bool isValid = a <= b + c && b <= a + c && c <= a + b;

                        stringBuilder.Clear();
                        stringBuilder.AppendLine($"Os valores informados são válidos para um triângulo? {(isValid ? "SIM" : "NÃO")}");

                        if (isValid)
                        {
                            string? triangleType = null;

                            if (a == b && b == c && c == a)
                            {
                                triangleType += "Triângulo Equilátero";
                            }
                            else if (a != b && b != c && c != a)
                            {
                                triangleType += "Triângulo Escaleno";
                            }
                            else
                            {
                                triangleType += "Triângulo Isósceles";
                            }

                            stringBuilder.AppendLine($"O triângulo informado é do tipo: {triangleType}");

                            Console.WriteLine($"[{Thread.CurrentThread.Name}] {a} {b} {c} é um {triangleType}");
                        }

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
            }
        }
    });

    thread.Name = id.ToString().PadLeft(3, '0');
    thread.Start(client);

    id++;
}