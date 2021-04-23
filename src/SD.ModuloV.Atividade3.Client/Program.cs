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
    Socket socket = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

    await socket.ConnectAsync(endPoint);

    Console.WriteLine($"Socket conectado -> {socket.RemoteEndPoint}");

    byte[] buffer = new byte[2048];

    var messageCount = await socket.ReceiveAsync(buffer, SocketFlags.None);
    Console.WriteLine(Encoding.UTF8.GetString(buffer));

    var valuesCount = await socket.ReceiveAsync(buffer, SocketFlags.None);
    Console.WriteLine(Encoding.UTF8.GetString(buffer));
}
catch (Exception exception)
{
    Console.WriteLine(exception);
}