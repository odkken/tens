using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets.Scripts.Game;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class NetworkManager : MonoBehaviour
    {
        public string GameTypeName = "Tens 2 Player";
        public string GameName = "Tens Test";

        public Player.Player PlayerTemplate;
        public TensGame Game;
        public List<Player.Player> Players;
        public Rect ServerListButtonRect;
        private HostData[] _hosts;
        private bool _refreshing;
        private Rect _connectRect;
        private Rect _startServerRect;
        // Use this for initialization
        void Start()
        {
            Players = new List<Player.Player>();
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
            if (UnityEngine.Network.isServer)
            {
                var players = FindObjectsOfType<Player.Player>();
                if (players.Count() == 1 && !players.Any(a => a.Dealer))
                    players[0].SetDealer(new System.Random().Next());
            }
        }

        private void StartServer()
        {
            UnityEngine.Network.InitializeServer(1, 25008, !UnityEngine.Network.HavePublicAddress());
            MasterServer.RegisterHost(GameTypeName, GameName);
        }

        private void SpawnPlayer()
        {
            UnityEngine.Network.Instantiate(PlayerTemplate, Vector3.zero, Quaternion.identity, 0);
        }

        void OnServerInitialized()
        {
            UnityEngine.Network.Instantiate(Game, Vector3.zero, Quaternion.identity, 0);
            SpawnPlayer();
        }

        void OnConnectedToServer()
        {
            SpawnPlayer();
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
