using SD.ModuloV.Atividade5.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

IEnumerable<Pessoa> GetPessoas()
{
    FileInfo fileInfo = new(typeof(Pessoa).Assembly.Location);
    var path = Path.Combine(fileInfo.Directory!.FullName, "Assets", "Dados.csv");

    using StreamReader streamReader = new(path);

    while (streamReader.ReadLine() is string @string)
    {
        var values = @string.Split(';', StringSplitOptions.RemoveEmptyEntries);

        yield return new()
        {
            Altura = decimal.Parse(values[0]),
            Genero = Enum.Parse<PessoaGenero>(values[1])
        };
    }
}

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
    Console.WriteLine($"Aguardando conexões -> {socketListener.LocalEndPoint}");

    int id = 1;
    var pessoas = GetPessoas();

    while (true)
    {
        Socket socketClient = await socketListener.AcceptAsync();

        Thread thread = new(async @object =>
        {
            if (@object is Socket { Connected: true } socket)
            {
                Console.WriteLine($"[{Thread.CurrentThread.Name}] Cliente conectado...");

                StringBuilder stringBuilder = new();
                stringBuilder.AppendLine("Atividade 5");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Pretende-se construir uma aplicação distribuída cliente/servidor cuja comunicação será feita através de Sockets. Tem-se um conjunto de dados contendo a altura e o sexo (masculino=1, feminino=0) de 50 pessoas.");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Faça uma aplicação distribuída que calcule e escreva:");
                stringBuilder.AppendLine("• A maior e a menor altura do grupo;");
                stringBuilder.AppendLine("• A média de altura das mulheres;");
                stringBuilder.AppendLine("• O número de homens.");

                Console.WriteLine($"[{Thread.CurrentThread.Name}] Enviando sumário da aplicação...");
                await socket.SendAsync(Encoding.UTF8.GetBytes(stringBuilder.ToString()), SocketFlags.None);

                var maiorAltura = pessoas.Max(x => x.Altura);
                var menorAltura = pessoas.Min(x => x.Altura);

                stringBuilder.Clear();
                stringBuilder.AppendLine($"A maior e a menor altura do grupo é respectivamente {maiorAltura} e {menorAltura} metros.");

                var mediaAlturaMulheres = pessoas
                    .Where(pessoa => pessoa.Genero == PessoaGenero.Feminino)
                    .Average(x => x.Altura);

                stringBuilder.AppendLine($"A média de altura das mulheres é {mediaAlturaMulheres:#.##} metros.");

                var numeroHomens = pessoas
                    .Where(pessoa => pessoa.Genero == PessoaGenero.Masculino)
                    .Count();

                stringBuilder.AppendLine($"O número de homens é {numeroHomens}.");

                Console.WriteLine($"[{Thread.CurrentThread.Name}] Enviando processamento da aplicação...");
                await socket.SendAsync(Encoding.UTF8.GetBytes(stringBuilder.ToString()), SocketFlags.None);

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        });
        thread.Name = id.ToString().PadLeft(3, '0');
        thread.Start(socketClient);

        id++;
    }
}
catch (Exception exception)
{
    Console.WriteLine(exception);
}