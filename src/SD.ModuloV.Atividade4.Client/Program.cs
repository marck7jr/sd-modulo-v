using System;
using System.IO;
using System.Net.Sockets;

int port = 2021;

TcpClient tcpClient = new("localhost", port);

Console.WriteLine($"Cliente iniciado -> {tcpClient.Client.LocalEndPoint}");
Console.WriteLine($"Conectado ao servidor -> {tcpClient.Client.RemoteEndPoint}");

using BinaryReader reader = new(tcpClient.GetStream());

var @string = reader.ReadString();
Console.WriteLine(@string);

var values = reader.ReadString();
Console.WriteLine(values);

tcpClient.Close();

Console.WriteLine("Conexão encerrada...");