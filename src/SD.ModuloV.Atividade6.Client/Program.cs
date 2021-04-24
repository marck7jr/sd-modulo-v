using System;
using System.IO;
using System.Net.Sockets;

int port = 2021;

TcpClient tcpClient = new("localhost", port);

Console.WriteLine($"Cliente iniciado -> {tcpClient.Client.LocalEndPoint}");
Console.WriteLine($"Conectado ao servidor -> {tcpClient.Client.RemoteEndPoint}");

using BinaryReader reader = new(tcpClient.GetStream());
using BinaryWriter writer = new(tcpClient.GetStream());

var @string = reader.ReadString();

Console.WriteLine(@string);

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