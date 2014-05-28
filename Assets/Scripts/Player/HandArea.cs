using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Cards;
using Assets.Scripts.Common;
using Assets.Scripts.Game;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class HandArea : MonoBehaviour
    {
        private const float PositionalRandomness = .5f;
        private const float RotationalRandomness = .1f;

        private const float DepthSpacing = .1f;

        public Player Player { get; private set; }

        private TensGame game;

        private Deck _deck;
        private Deck Deck
        {
            get
            {
                if (_deck == null) _deck = FindObjectOfType<Deck>();
                return _deck;
            }
        }
        public List<Card> Cards;
        public bool CanAddMore { get { return Cards.Count < 10; } }

        void Start()
        {
            Cards = new List<Card>();
            Player = transform.parent.GetComponent<Player>();
            game = FindObjectOfType<TensGame>();
        }

        void Update()
        {

        }

        void OnMouseDown()
        {
            switch (game.CurrentState)
            {
                case TensGame.GameState.Deal:
                    //var allPlayers = FindObjectsOfType<Player>();
                    //if (allPlayers.Any(a => a.Dealer) && allPlayers.Single(a => a.Dealer).networkView.isMine && CanAddMore && !Deck.Dealing)
                    //    networkView.RPC("AddCard", RPCMode.All);
                    break;
                case TensGame.GameState.Bid:
                    if (!Player.HasPickedUpCards && !Cards.Any(a => a.Moving))
                        Player.PickUpHand();
                    break;
                case TensGame.GameState.HandPlay:
                    break;
                case TensGame.GameState.TablePlay:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [RPC]
        public void AddCard()
        {
            var card = Deck.GetTopCard();
            card.AssignOwner(Player);
            if (Cards.Count < 10)
            {
                card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y, -10);
                card.MoveTo(transform.position + new Vector3(Util.NextGaussian(PositionalRandomness), Util.NextGaussian(PositionalRandomness), -DepthSpacing * (Cards.Count + 1)));
                card.RotateTo(Util.NextGaussian(RotationalRandomness) * 180, false, false);
                Cards.Add(card);
                card.transform.parent = transform;
            }
        }

    }
}
