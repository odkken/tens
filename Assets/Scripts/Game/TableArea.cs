using Assets.Scripts.Cards;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class TableArea : MonoBehaviour
    {
        public float HorizontalSpacing = 2f;
        public float VerticalOffset = .2f;

        public float FaceDownZPos = -.1f;
        public float FaceUpZPos = -.2f;

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
                card.MoveTo(transform.position + new Vector3(HorizontalSpacing * (-2 + _bottomCards.Count), VerticalOffset / 2, FaceDownZPos));
                _bottomCards.Add(card);
                card.transform.parent = transform;
            }
            else if (_topCards.Count < 5)
            {
                card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y, -10);
                card.Flip();
                card.MoveTo(transform.position + new Vector3(HorizontalSpacing * (-2 + _topCards.Count), -VerticalOffset / 2, FaceUpZPos));
                _topCards.Add(card);
                card.transform.parent = transform;
            }
        }
    }
}
