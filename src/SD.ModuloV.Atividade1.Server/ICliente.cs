using System.IO;
using System.Net.Sockets;

namespace SD.ModuloV.Atividade1.Server
{
    public interface ICliente
    {
        public void HandleConnection(TcpClient tcpClient, BinaryReader binaryReader, BinaryWriter binaryWriter);
    }
}
