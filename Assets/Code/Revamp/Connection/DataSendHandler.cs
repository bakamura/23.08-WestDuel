using System.IO;
using System.Net;

public static class DataSendHandler {

    public static void SendPack(ConnectionHandler.DataPacksIdentification dataType, object data, IPEndPoint ipReceiver) {
        ConnectionHandler.memoryStreamCache = new MemoryStream();
        ConnectionHandler.binaryFormatter.Serialize(ConnectionHandler.memoryStreamCache, data);
        ConnectionHandler.udpClient.Send(ConnectionHandler.byteArrayCache, ConnectionHandler.byteArrayCache.Length, ipReceiver);
    }

}