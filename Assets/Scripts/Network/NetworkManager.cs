using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets.Scripts.Common;
using Assets.Scripts.Game;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class NetworkManager : MonoBehaviour
    {
        public string GameTypeName = "Tens 2 Player";
        public string GameName = "Tens Test";

        private int MaxPlayers = 2;
        public Player.Player PlayerTemplate;
        public TensGame Game;
        public List<NetworkPlayer> NetworkPlayers;
        public Rect ServerListButtonRect;
        private HostData[] _hosts;
        private bool _refreshing;
        private bool gameStarted;
        private Rect _connectRect;
        private Rect _startServerRect;
        // Use this for initialization
        void Start()
        {
            gameStarted = false;
            NetworkPlayers = new List<NetworkPlayer>();
            var sWidth = Screen.width;
            var sHeight = Screen.height;
            _connectRect = new Rect(sWidth * .1f, sHeight * .1f, sWidth * .1f, sHeight * .1f);
            _startServerRect = new Rect(sWidth - (sWidth * .2f), sHeight * .1f, sWidth * .1f, sHeight * .1f);
            StartCoroutine(RefreshHosts());
            UnityEngine.Network.minimumAllocatableViewIDs = 250;
            //UnityEngine.Network.logLevel = NetworkLogLevel.Full;
        }



        // Update is called once per frame
        void Update()
        {
            if (UnityEngine.Network.isServer && !gameStarted && NetworkPlayers.Count == MaxPlayers)
            {
                gameStarted = true;
                //NetworkPlayers.Shuffle(UnityEngine.Random.Range(0, 255));
                networkView.RPC("SpawnPlayers", RPCMode.AllBuffered, string.Join(",", NetworkPlayers.Select(a => a.ToString()).ToArray()), UnityEngine.Random.Range(int.MinValue, int.MaxValue));
            }
        }


        private void StartServer()
        {
            UnityEngine.Network.InitializeServer(1, 25008, !UnityEngine.Network.HavePublicAddress());
            MasterServer.RegisterHost(GameTypeName, GameName);
        }

        private void RegisterPlayer()
        {
            //UnityEngine.Network.Instantiate(PlayerTemplate, Vector3.zero, Quaternion.identity, 0);
            networkView.RPC("RegisterPlayerRPC", RPCMode.AllBuffered);
        }
        [RPC]
        private void RegisterPlayerRPC(NetworkMessageInfo info)
        {
            if (!UnityEngine.Network.isServer) return;
            NetworkPlayers.Add(info.sender);
            Debug.Log("Registered " + info.sender);
        }


        [RPC]
        private void SpawnPlayers(string csvIds, int shuffleSeed)
        {
            var playerIds = new List<String>(csvIds.Split(','));
            for (var i = 0; i < playerIds.Count(); i++)
            {
                var playerId = playerIds[i];

                //some bullshit the server does
                if (playerId == "-1")
                    playerId = "0";

                if (playerId == UnityEngine.Network.player.ToString())
                {
                    var player = UnityEngine.Network.Instantiate(PlayerTemplate, new Vector3(0, -3, -1), Quaternion.identity, 0) as Player.Player;
                    player.transform.RotateAround(Vector3.zero, Vector3.forward, 180 * i);
                    Camera.main.transform.rotation = player.transform.rotation;
                    player.IsLocalPlayer = true;
                    if (i == 0)
                    {
                        player.SetDealer(shuffleSeed);
                        Debug.Log("Dealer is player " + playerId);
                    }
                }
            }

        }



        void OnServerInitialized()
        {
            UnityEngine.Network.Instantiate(Game, Vector3.zero, Quaternion.identity, 0);
            RegisterPlayer();
        }

        void OnConnectedToServer()
        {
            RegisterPlayer();
        }

        void OnDisconnectedFromServer()
        {
            Application.Quit();
        }

        private IEnumerator RefreshHosts()
        {
            if (_refreshing) yield break;
            MasterServer.RequestHostList(GameTypeName);
            _refreshing = true;
            _hosts = MasterServer.PollHostList();
            var startTime = Time.time;
            while (!_hosts.Any() && Time.time - startTime < 2f)
            {
                _hosts = MasterServer.PollHostList();
                yield return new WaitForSeconds(.1f);
            }
            _refreshing = false;
            foreach (var hostData in _hosts)
            {
                Debug.Log(hostData.gameName);
            }

        }

        // ReSharper disable once InconsistentNaming
        void OnGUI()
        {
            if (UnityEngine.Network.isClient || UnityEngine.Network.isServer) return;
            if (GUI.Button(_connectRect, "Refresh Hosts"))
                StartCoroutine(RefreshHosts());
            if (GUI.Button(_startServerRect, "Start Server"))
                StartServer();
            foreach (var hostData in _hosts)
            {
                if (
                    GUI.Button(
                        new Rect((Screen.width - ServerListButtonRect.width) / 2,
                            (Screen.height - ServerListButtonRect.height) / 2 + ServerListButtonRect.height * .2f,
                            ServerListButtonRect.width, ServerListButtonRect.height), hostData.gameName))
                {
                    UnityEngine.Network.Connect(hostData);
                    break;
                }
            }
        }

    }
}
