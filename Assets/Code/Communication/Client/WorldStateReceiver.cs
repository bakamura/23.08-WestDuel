using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Net;
using System.Linq;
using System.Net.Sockets;

public class WorldStateReceiver : DataReceiver<WorldStateDataPack>
{
    [SerializeField] private NetworkReferenceContainer _container;
    private WorldStateDataPack _dataPack;
    private UdpClient _udpClient;
    private IPEndPoint _endPoint;
    private bool[] _bulletsShoot = new bool[4];//size is playerCount * MaxBulletPerPlayer

    protected override void Awake()
    {
        _udpClient = new UdpClient(WorldStateDataPack.Port);
        base.Awake();
    }
    protected override void ReceivePack()
    {
        while (true)
        {
            _memoryStream = new MemoryStream(_udpClient.Receive(ref _endPoint));
            if (_endPoint.Address == ClientConnectionHandler.ServerEndPoint)
            {
                _dataPack = CheckDataPack<WorldStateDataPack>(DataPacksIdentification.GamStateDataPack);
                if (_dataPack != null)
                {
                    if (!_ipToData.ContainsKey(ClientConnectionHandler.ServerEndPoint)) _ipToData.Add(ClientConnectionHandler.ServerEndPoint, _dataPack);
                    else _ipToData[_endPoint.Address] = _dataPack;
                }
            }
        }
    }

    protected override void ImplementPack()
    {
        if (_ipToData.ContainsKey(ClientConnectionHandler.ServerEndPoint) && _ipToData[ClientConnectionHandler.ServerEndPoint].updated)
        {
            #region UpdatePlayers
            for (int i = 0; i < _ipToData[ClientConnectionHandler.ServerEndPoint].playersPos.Count; i++)
            {
                ClientConnectionHandler.PlayersList[i].Object.transform.position = PackingUtility.FloatArrayToVector3(_ipToData[ClientConnectionHandler.ServerEndPoint].playersPos[i]);
                ClientConnectionHandler.PlayersList[i].Rigidbody.velocity = PackingUtility.FloatArrayToVector3(_ipToData[ClientConnectionHandler.ServerEndPoint].playersVelocity[i]);
            }
            #endregion
            #region UpdateBullets
            if (_ipToData[ClientConnectionHandler.ServerEndPoint].bulletsPos.Count > ClientConnectionHandler.BulletsList.Count)
            {
                int newBullets = _ipToData[ClientConnectionHandler.ServerEndPoint].bulletsPos.Count - ClientConnectionHandler.BulletsList.Count;
                for (int a = _ipToData[ClientConnectionHandler.ServerEndPoint].bulletsPos.Count - newBullets; a < _ipToData[ClientConnectionHandler.ServerEndPoint].bulletsPos.Count; a++)
                {
                    Bullet temp = Instantiate(InstantiateHandler.GetBulletPrefab(), null).GetComponent<Bullet>();
                    temp.Shoot(PackingUtility.FloatArrayToVector3(_ipToData[ClientConnectionHandler.ServerEndPoint].bulletsPos[a]), PackingUtility.FloatArrayToVector3(_ipToData[ClientConnectionHandler.ServerEndPoint].bulletsVelocity[a]));
                    ClientConnectionHandler.BulletsList.Add(temp);
                }
            }
            for (int i = 0; i < _ipToData[ClientConnectionHandler.ServerEndPoint].bulletsPos.Count; i++)
            {
                if (_ipToData[ClientConnectionHandler.ServerEndPoint].bulletsPos[i] == _ipToData[ClientConnectionHandler.ServerEndPoint].deactivatePos)
                {
                    ClientConnectionHandler.BulletsList[i].UpdateState(false);
                }
                else
                {
                    ClientConnectionHandler.BulletsList[i].UpdateState(true);
                    ClientConnectionHandler.BulletsList[i].Shoot(PackingUtility.FloatArrayToVector3(_ipToData[ClientConnectionHandler.ServerEndPoint].bulletsPos[i]), PackingUtility.FloatArrayToVector3(_ipToData[ClientConnectionHandler.ServerEndPoint].bulletsVelocity[i]));
                }
            }
            #endregion
            #region UpdateAmmoBoxes
            //if (_ipToData[ClientConnectionHandler.ServerEndPoint].boxesPos.Count > ClientConnectionHandler.AmmoBoxList.Count)
            //{
            //    int newBoxes = _ipToData[ClientConnectionHandler.ServerEndPoint].boxesPos.Count - ClientConnectionHandler.AmmoBoxList.Count;
            //    for (int i = 0; i < newBoxes; i++)
            //    {
            //        ClientConnectionHandler.AmmoBoxList.Add(_container.SpawnPickup.Spawn());
            //        //ClientConnectionHandler.AmmoBoxList.Add(Instantiate(InstantiateHandler.GetAmmoBoxPrefab(),
            //        //    PackingUtility.FloatArrayToVector3(_ipToData[ClientConnectionHandler.ServerEndPoint].boxesPos[i]),
            //        //    Quaternion.identity).GetComponent<BulletPickup>());
            //    }
            //}
            //else if (_ipToData[ClientConnectionHandler.ServerEndPoint].boxesPos.Count < ClientConnectionHandler.AmmoBoxList.Count)
            //{
            //    BulletPickup[] temp = ClientConnectionHandler.AmmoBoxList.Select(x => !_ipToData[ClientConnectionHandler.ServerEndPoint].boxesPos.Contains(PackingUtility.Vector3ToFloatArray(x.GetComponent<Transform>().position))).ToArray();
            //    for (int i = 0; i < temp.Length; i++)
            //    {
            //        ClientConnectionHandler.AmmoBoxList[i].CollectBullet();
            //    }
            //}
            //else
            //{
            // TALVEZ TENHA PROBLEMA DE VER CAIXAS TELEPORTANDO J� Q ELE SEMPRE PEGA O 1 DISPONIVEL
            for (int i = 0; i < _ipToData[ClientConnectionHandler.ServerEndPoint].boxesPos.Count; i++)
            {
                if (_ipToData[ClientConnectionHandler.ServerEndPoint].boxesPos[i] == _ipToData[ClientConnectionHandler.ServerEndPoint].deactivatePos)
                {
                    _container.SpawnPickup.Spawn().CollectBullet();
                }
                else
                {
                    _container.SpawnPickup.Spawn().SetPosition(PackingUtility.FloatArrayToVector3(_ipToData[ClientConnectionHandler.ServerEndPoint].boxesPos[i]));
                    //BulletPickup temp = _container.SpawnPickup.Spawn();
                    //    temp.transform.position = PackingUtility.FloatArrayToVector3(_ipToData[ClientConnectionHandler.ServerEndPoint].boxesPos[i]);
                    //    ClientConnectionHandler.AmmoBoxList[i] = temp;
                }
            }
            //}
            #endregion
            #region UpdatePlayersAnimations
            for (int i = 0; i < _ipToData[ClientConnectionHandler.ServerEndPoint].playersPos.Count; i++)
            {
                ClientConnectionHandler.PlayersList[i].AnimationsUpdate.SetDirection(PackingUtility.FloatArrayToVector3(_dataPack.playersVelocity[i]));
                ClientConnectionHandler.PlayersList[i].AnimationsUpdate.SetMousePosition(PackingUtility.FloatArrayToVector3(_dataPack.playersMousePosition[i]));
                for (int a = _ipToData[ClientConnectionHandler.ServerEndPoint].playersPos.Count * i; a < _bulletsShoot.Length; a++)
                {
                    ClientConnectionHandler.PlayersList[i].AnimationsUpdate.TriggerShootAnim();
                    _bulletsShoot[a] = false;
                }
            }
            #endregion
            _ipToData[ClientConnectionHandler.ServerEndPoint].updated = false;
        }
    }
}