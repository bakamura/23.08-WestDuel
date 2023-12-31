using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

public static class ConnectionHandler {

    public static UdpClient udpClient;
    public static IPEndPoint serverIpEp;

    public static BinaryFormatter binaryFormatter = new BinaryFormatter();
    public static MemoryStream memoryStreamCache;
    public static byte byteCache;
    public static byte[] byteArrayCache;

    public static IPEndPoint ipEpCache;
    public static InputDataPack inputDataCache;
    public static WorldStateDataPack worldDataCache;
    public static GameStateDataPack gameStateDataCache;

    public const int port = 12369;

    static ConnectionHandler() {
        udpClient = new UdpClient(port);
    }

    public enum DataPacksIdentification {
        InputDataPack,
        WorldStateDataPack,
        GameStateDataPack,
        String
    }

}
