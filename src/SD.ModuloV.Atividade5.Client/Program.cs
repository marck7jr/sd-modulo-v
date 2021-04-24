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

try
{
    string? @string = null;

    Socket socket = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

    await socket.ConnectAsync(endPoint);

    Console.WriteLine($"Socket conectado -> {socket.RemoteEndPoint}");

    byte[]? buffer = null;

    buffer = new byte[1024];
    var messageCount = await socket.ReceiveAsync(buffer, SocketFlags.None);
    @string = Encoding.UTF8.GetString(buffer);
    Console.WriteLine(@string);

    buffer = new byte[1024];
    var valuesCount = await socket.ReceiveAsync(buffer, SocketFlags.None);
    @string = Encoding.UTF8.GetString(buffer);
    Console.WriteLine(@string);

    socket.Shutdown(SocketShutdown.Both);
    socket.Close();

    Console.WriteLine("Conexão encerrada...");
}
catch (Exception exception)
{
    Console.WriteLine(exception);
}