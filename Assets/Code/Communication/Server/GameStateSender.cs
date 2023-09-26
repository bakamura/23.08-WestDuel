using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Net;

public class GameStateSender : DataSender<GameStateDataPack> {

    [Header("Check")]

    private bool _sendSignal;

    [Header("Cache")]

    private BinaryFormatter _bf = new BinaryFormatter();
    private MemoryStream _ms;
    private UdpClient _udpClient;

    private void Awake()
    {
        _udpClient = new UdpClient(GameStateDataPack.PortClientReceive);
        _dataPackCache = new GameStateDataPack();
    }

    protected override void FixedUpdate() {
        PreparePack();
        if (_sendSignal) {
            _sendSignal = false;
            SendPack();
        }
    }

    protected override void PreparePack() {
        for (int i = 0; i < ServerConnectionHandler.players.Count; i++) {
            if (_dataPackCache.playerHealth[i] != ServerConnectionHandler.players[i].health.GetCurrentHealth()) {
                _sendSignal = true;
                _dataPackCache.playerHealth[i] = ServerConnectionHandler.players[i].health.GetCurrentHealth();
                _dataPackCache.gameState = _dataPackCache.playerHealth[i] > 0 ? GameStateDataPack.GameState.Continue : GameStateDataPack.GameState.Ended;
                if (_dataPackCache.gameState == GameStateDataPack.GameState.Ended) {
                    EndGame();
                    break; // Doesn't allow ties
                }
            }
        }
    }

    protected override void SendPack() {
        _ms = new MemoryStream();
        _bf.Serialize(_ms, _dataPackCache);
        _byteArrayCache = AddIdentifierByte(_ms.ToArray(), (byte)DataPacksIdentification.GamStateDataPack);
        for (int i = 0; i < ServerConnectionHandler.players.Count; i++) _udpClient.Send(_byteArrayCache, _byteArrayCache.Length, new IPEndPoint(ServerConnectionHandler.players[i].ip, GameStateDataPack.PortClientReceive));
    }

    public void QuitMatch() {
        _dataPackCache.gameState = GameStateDataPack.GameState.Quit;
        // Reset server IP etc data

        SendPack();
    }

    private void OnApplicationQuit() {
        QuitMatch();
    }

    private void EndGame() {
        // Maybe should be in other script
        // Pause all world behaviours
    }

    public void RestartGame() {
        // Maybe should be in other script
        // Set playerHealth to Max
        // Unpause Game
    }

    private void OnDestroy()
    {
        _udpClient.Close();
    }
}
