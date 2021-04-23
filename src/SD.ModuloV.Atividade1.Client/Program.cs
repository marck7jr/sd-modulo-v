using System;
using System.IO;
using System.Net.Sockets;

int port = 2021;

TcpClient tcpClient = new("localhost", port);

Console.WriteLine($"Cliente iniciado -> {tcpClient.Client.LocalEndPoint}");
Console.WriteLine($"Conectado ao servidor -> {tcpClient.Client.RemoteEndPoint}");

using BinaryReader reader = new(tcpClient.GetStream());
var question = reader.ReadString();

Console.WriteLine(question);
var answer = Console.ReadLine();

using BinaryWriter writer = new(tcpClient.GetStream());
writer.Write(answer!);

Console.WriteLine("Informe os valores desejados e quando desejar parar informe algo que não seja número:");

while (tcpClient.Connected)
{
    var input = Console.ReadLine();
    writer.Write(input!);

    var response = reader.ReadString();

    if (string.IsNullOrEmpty(response))
    {
        tcpClient.Close();
    }
    else
    {
        Console.WriteLine(response);
    }
}

Console.WriteLine("Conexão encerrada...");