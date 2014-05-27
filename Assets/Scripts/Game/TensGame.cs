using System;
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

        public Player.Player DealerPlayer;

        private Deck _deck;
        private Deck Deck
        {
            get
            {
                if (_deck == null) _deck = FindObjectOfType<Deck>();
                return _deck;
            }
        }

        [HideInInspector]
        public List<Player.Player> AllPlayers;

        public GameState CurrentState { get; private set; }

        void Start()
        {
        }

        void Update()
        {
            switch (CurrentState)
            {
                case GameState.Deal:
                    if (DealerPlayer != null && Deck.DoneDealing)
                        networkView.RPC("SetGameState", RPCMode.AllBuffered, (int)GameState.Bid);
                    break;
                case GameState.Bid:
                    break;
                case GameState.HandPlay:
                    break;
                case GameState.TablePlay:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [RPC]
        public void SetDealer(int playerIndex)
        {
            networkView.RPC("SetDealerRPC", RPCMode.AllBuffered, playerIndex);
        }

        [RPC]
        private void SetDealerRPC(int playerIndex)
        {
            DealerPlayer = FindObjectsOfType<Player.Player>()[playerIndex];
        }

        public void SetGameState(int state)
        {
            networkView.RPC("SetGameStateRPC", RPCMode.AllBuffered, state);
        }
        [RPC]
        private void SetGameStateRPC(int state)
        {
            CurrentState = (GameState)state;
        }

    }
}
