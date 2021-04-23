using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace SD.ModuloV.Atividade1.Server
{
    public class Cliente_B : ICliente
    {
        public void HandleConnection(TcpClient tcpClient, BinaryReader binaryReader, BinaryWriter binaryWriter)
        {
            while (tcpClient.Connected)
            {
                var @string = binaryReader.ReadString();

                if (double.TryParse(@string, out double @double))
                {
                    var squareRoot = Math.Sqrt(@double);

                    var response = $"√{@double} = {squareRoot}";
                    var log = $"[{Thread.CurrentThread.Name}] Respondendo com a mensagem: {response}";

                    Console.WriteLine(log);

                    binaryWriter.Write(response);
                }
                else
                {
                    throw new ApplicationException();
                }
            }
        }
    }
}
