using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game;
using Assets.Scripts.Network;
using Assets.Scripts.Player;
using UnityEngine;
using Assets.Scripts.Common;

namespace Assets.Scripts.Cards
{
    public class Deck : MonoBehaviour
    {
        private List<Card> _cards;
        public List<Card> Cards
        {
            get { return _cards ?? (_cards = GetComponentsInChildren<Card>().ToList()); }
        }
        public int CardsLeft { get { return Cards != null ? Cards.Count : 0; } }
        public float DealTime = 1f;
        private const float CardSpacing = .01f;
        public bool Dealing { get; private set; }
        public bool DoneDealing { get; private set; }
        public bool Shuffled;
        private TensGame game;

        public Player.Player Dealer { get; private set; }

        // Use this for initialization
        void Start()
        {
            game = FindObjectOfType<TensGame>();
            Shuffled = false;
            Dealing = false;
            DoneDealing = false;
        }

        void Update()
        {

            switch (game.CurrentState)
            {
                case TensGame.GameState.Deal:
                    if (CardsLeft == 0)
                    {
                        transform.position = new Vector3(-999, -999, -999);
                        Dealing = false;
                        DoneDealing = true;
                    }

                    break;
                case TensGame.GameState.Bid:
                    break;
                case TensGame.GameState.HandPlay:
                    break;
                case TensGame.GameState.TablePlay:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void Initialize(int rngSeed)
        {
            Dealing = false;
        }


        void OnMouseDown()
        {
            if (game.CurrentState == TensGame.GameState.Deal && !Dealing && networkView.isMine && gameObject.GetComponent<Deck>() != null)
                networkView.RPC("Deal", RPCMode.AllBuffered);
        }

        [RPC]
        private void Deal()
        {
            Dealing = true;
            StartCoroutine(AnimateDeal(DealTime));
        }

        IEnumerator AnimateDeal(float dealTime)
        {
            int dealTo = 0;
            int totalCards = CardsLeft;
            var players = FindObjectsOfType<Player.Player>().ToList();
            while (CardsLeft > 0)
            {
                float waitTime = 0;
                var receivingPlayer = players[dealTo++];
                if (receivingPlayer.CanAddMore)
                {
                    receivingPlayer.GiveCard();
                    waitTime = dealTime / totalCards;
                }
                if (dealTo == players.Count())
                    dealTo = 0;
                yield return new WaitForSeconds(waitTime);
            }
            Dealing = false;
        }

        public void Shuffle(int seed)
        {
            Cards.Shuffle(seed);
            Shuffled = true;
            for (int i = 0; i < Cards.Count; i++)
            {
                Cards[i].transform.position = transform.position + new Vector3(0, 0, -CardSpacing * i);
            }
        }

        [RPC]
        public void ShuffleRPC(int seed)
        {
            Shuffle(seed);
        }

        public Card GetTopCard()
        {
            var card = Cards.Pop();
            card.transform.parent = null;
            return card;
        }
    }
}
