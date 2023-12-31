using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ServerWorldStateSender : Singleton<ServerWorldStateSender> {

    private WorldStateDataPack _dataPackCache;
    private Dictionary<IPEndPoint, List<Rigidbody>> playerBulletRigidBody = new Dictionary<IPEndPoint, List<Rigidbody>>();

    private void FixedUpdate() {
        PreparePack();
    }

    private void PreparePack() {
        if (ServerPlayerInfo.player.Count > 0) {
            foreach (IPEndPoint ip in ServerPlayerInfo.player.Keys) {
                _dataPackCache.playersPos[ip] = PackingUtility.Vector3ToFloatArray(ServerPlayerInfo.player[ip].transform.position);
                _dataPackCache.playersVelocity[ip] = PackingUtility.Vector3ToFloatArray(ServerPlayerInfo.player[ip].rigidBody.velocity);
                _dataPackCache.playersShootPoint[ip] = PackingUtility.Vector3ToFloatArray(ServerPlayerInfo.player[ip].shoot.aimPoint); // Create vector3 CurrentAim() in PlayerShoot
                _dataPackCache.playersHasBullet[ip] = ServerPlayerInfo.player[ip].shoot.CheckBullet();

                for (int i = 0; i < _dataPackCache.bulletsPos.Count; i++) {
                    _dataPackCache.bulletsPos[ip][i] = playerBulletRigidBody[ip][i].gameObject.activeSelf ? PackingUtility.Vector3ToFloatArray(playerBulletRigidBody[ip][i].transform.position) : _dataPackCache.deactivatePos;
                    _dataPackCache.bulletsVelocity[ip][i] = playerBulletRigidBody[ip][i].gameObject.activeSelf ? PackingUtility.Vector3ToFloatArray(playerBulletRigidBody[ip][i].velocity) : _dataPackCache.deactivatePos;
                }

                //for(int i = 0; i < 2 /* should be BulletPickup Amount */; i++) {
                // Update Each BulletPickup
                // IDK what is this
                //}
            }
            foreach(IPEndPoint ip in ServerPlayerInfo.player.Keys) if(ip != ConnectionHandler.serverIpEp) DataSendHandler.SendPack(_dataPackCache, (byte)ConnectionHandler.DataPacksIdentification.WorldStateDataPack, ip);
        }
    }

    public GameObject InstantiatePlayer(bool isServer) {
        return Instantiate(isServer ? InstantiateHandler.GetPlayer1HostPrefab() : InstantiateHandler.GetPlayer2HostPrefab(),
                    FindObjectOfType<SpawnPlayer>().GetPointFurthestFromOponent(isServer ? Vector3.zero : ServerPlayerInfo.player[ConnectionHandler.serverIpEp].transform.position),
                    Quaternion.identity);
    }

    public void AddBulletPool(IPEndPoint ipOwner, Bullet[] bulletPool) {
        List<Rigidbody> rbList = new List<Rigidbody>();
        foreach (Bullet bullet in bulletPool) rbList.Add(bullet.GetComponent<Rigidbody>());
        playerBulletRigidBody.Add(ipOwner, rbList);
    }

    public void AddPlayerIP(IPEndPoint[] playerIp) {
        _dataPackCache = new WorldStateDataPack(playerIp);
    }

}
