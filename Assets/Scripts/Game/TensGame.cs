using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Scripts.Cards;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class TensGame : MonoBehaviour
    {
        public enum GameState
        {
            Deal, Bid, HandPlay, TablePlay
        }

        public int MaxPlayers;

        public Player.Player PlayerTemplate;

        [HideInInspector]
        public List<Player.Player> AllPlayers;

        public Deck Deck;

        private TensGame _instance;

        public TensGame Instance { get { return _instance; } }

        public static GameState CurrentState = GameState.Deal;

        void Start()
        {
            for (int i = 0; i < MaxPlayers; i++)
            {
                var player = Instantiate(PlayerTemplate);
            }
            _instance = transform.GetComponent<TensGame>();
        }

        void Update()
        {
            
        }

    }
}
