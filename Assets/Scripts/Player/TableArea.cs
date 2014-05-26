using System;
using System.Collections.Generic;
using Assets.Scripts.Cards;
using Assets.Scripts.Game;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class TableArea : MonoBehaviour
    {
        private float _horizontalSpacing = 1.7f;
        private float _verticalOffset = .2f;

        private const float FaceDownZPos = -.1f;
        private const float FaceUpZPos = -.2f;

        private List<Card> _bottomCards;
        private List<Card> _topCards;

        private Deck _deck;
        private Deck Deck
        {
            get
            {
                if (_deck == null) _deck = FindObjectOfType<Deck>();
                return _deck;
            }
        }
        private TensGame game;
        public Player Player { get; private set; }

        public bool CanAddMore { get { return _topCards.Count < 5; } }

        void Start()
        {
            _bottomCards = new List<Card>();
            _topCards = new List<Card>();
            _verticalOffset *= transform.position.y < 0 ? 1 : -1;
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
                    if (CanAddMore && !Deck.Dealing)
                        AddCard(Deck.GetTopCard());
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

        public void AddCard(Card card)
        {
            if (_bottomCards.Count < 5)
            {
                card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y, -10);
                card.MoveTo(transform.position + new Vector3(_horizontalSpacing * (-2 + _bottomCards.Count), _verticalOffset / 2, FaceDownZPos));
                _bottomCards.Add(card);
                card.transform.parent = transform;
            }
            else if (_topCards.Count < 5)
            {
                card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y, -10);
                card.Flip();
                card.MoveTo(transform.position + new Vector3(_horizontalSpacing * (-2 + _topCards.Count), -_verticalOffset / 2, FaceUpZPos));
                _topCards.Add(card);
                card.transform.parent = transform;
            }
        }


    }
}
