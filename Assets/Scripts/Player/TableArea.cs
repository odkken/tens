using System;
using System.Collections.Generic;
using System.Linq;
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
            get { return _deck ?? (_deck = FindObjectOfType<Deck>()); }
        }
        private TensGame game;
        public Player Player { get; private set; }
        public List<Card> Cards;

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
            //switch (game.CurrentState)
            //{
            //    case TensGame.GameState.Deal:
            //        var allPlayers = FindObjectsOfType<Player>();
            //        if (allPlayers.Any(a => a.Dealer) && allPlayers.Single(a => a.Dealer).networkView.isMine && CanAddMore && !Deck.Dealing)
            //            networkView.RPC("AddCard", RPCMode.All);
            //        break;
            //    case TensGame.GameState.Bid:
            //        break;
            //    case TensGame.GameState.HandPlay:
            //        break;
            //    case TensGame.GameState.TablePlay:
            //        break;
            //    default:
            //        throw new ArgumentOutOfRangeException();
            //}
        }
        [RPC]
        public void AddCard()
        {
            var card = Deck.GetTopCard();
            card.AssignOwner(Player);
            if (_bottomCards.Count < 5)
            {
                card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y, -10);
                card.MoveTo(transform.position + new Vector3(_horizontalSpacing * (-2 + _bottomCards.Count), _verticalOffset / 2, FaceDownZPos));
                _bottomCards.Add(card);
            }
            else if (_topCards.Count < 5)
            {
                card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y, -10);
                card.Flip();
                card.MoveTo(transform.position + new Vector3(_horizontalSpacing * (-2 + _topCards.Count), -_verticalOffset / 2, FaceUpZPos));
                _topCards.Add(card);
            }
            Cards.Add(card);
        }


    }
}
