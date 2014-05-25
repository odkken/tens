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

        public List<Card> Cards;
        public bool CanAddMore { get { return Cards.Count < 10; } }
        
        void Start()
        {
            Cards = new List<Card>();
            Player = transform.parent.GetComponent<Player>();
        }

        void Update()
        {

        }

        void OnMouseDown()
        {
            switch (TensGame.CurrentState)
            {
                case TensGame.GameState.Deal:
                    if (CanAddMore && !TensGame.Instance.Deck.Dealing)
                        AddCard(TensGame.Instance.Deck.GetTopCard());
                    break;
                case TensGame.GameState.Bid:
                    if (!Player.HasPickedUpCards && !Cards.Any(a=> a.Moving))
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

        public void AddCard(Card card)
        {
            if (Cards.Count < 10)
            {
                card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y, -10);
                card.MoveTo(transform.position + new Vector3(Util.NextGaussian(PositionalRandomness), Util.NextGaussian(PositionalRandomness), -Cards.Count * DepthSpacing));
                card.RotateTo(Util.NextGaussian(RotationalRandomness) * 180, false);
                Cards.Add(card);
                card.transform.parent = transform;
            }
        }

    }
}
