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

        public Deck DeckTemplate;

        [HideInInspector]
        public Deck Deck;

        public static TensGame Instance { get; private set; }

        public static GameState CurrentState = GameState.Deal;

        void Start()
        {
            for (int i = 0; i < MaxPlayers; i++)
            {
                var player = Instantiate(PlayerTemplate);
            }
            Instance = gameObject.GetComponent<TensGame>();
            Deck = Instantiate(DeckTemplate, new Vector3(0, 0, -2), Quaternion.identity) as Deck;
        }

        void Update()
        {

        }

    }
}
