using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace SD.ModuloV.Atividade1.Server
{
    public class Cliente_A : ICliente
    {
        public void HandleConnection(TcpClient tcpClient, BinaryReader binaryReader, BinaryWriter binaryWriter)
        {
            while (tcpClient.Connected)
            {
                var @string = binaryReader.ReadString();

                if (int.TryParse(@string, out int integer))
                {
                    var squared = (int)Math.Pow(integer, 2);

                    var response = $"{integer} ^ 2 = {squared}";
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
