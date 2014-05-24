using Assets.Scripts.Cards;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class TableArea : MonoBehaviour
    {
        private float _horizontalSpacing = 1.7f;
        private float _verticalOffset = .2f;

        private const float FaceDownZPos = -.1f;
        private const float FaceUpZPos = -.2f;

        private List<Card> _bottomCards;
        private List<Card> _topCards;

        public bool CanAddMore { get { return _topCards.Count < 5; } }

        private Deck deck;
        // Use this for initialization
        void Start()
        {
            deck = FindObjectOfType<Deck>();
            _bottomCards = new List<Card>();
            _topCards = new List<Card>();
            _verticalOffset *= transform.position.y < 0 ? 1 : -1;
            _horizontalSpacing *= transform.position.y < 0 ? 1 : -1;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnMouseDown()
        {
            if (CanAddMore)
                AddCard(deck.GetTopCard());
        }

        void AddCard(Card card)
        {
            if (_bottomCards.Count < 5)
            {
                card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y, -10);
                card.MoveTo(transform.position + new Vector3(_horizontalSpacing * (-1 + _bottomCards.Count), _verticalOffset / 2, FaceDownZPos));
                _bottomCards.Add(card);
                card.transform.parent = transform;
            }
            else if (_topCards.Count < 5)
            {
                card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y, -10);
                card.Flip();
                card.MoveTo(transform.position + new Vector3(_horizontalSpacing * (-1 + _topCards.Count), -_verticalOffset / 2, FaceUpZPos));
                _topCards.Add(card);
                card.transform.parent = transform;
            }
        }
    }
}
