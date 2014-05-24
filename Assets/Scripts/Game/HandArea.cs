using Assets.Scripts.Cards;
using System.Collections.Generic;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class HandArea : MonoBehaviour
    {
        private const float PositionalRandomness = .5f;
        private const float RotationalRandomness = .1f;

        private const float DepthSpacing = .1f;

        private List<Card> _handCards;

        public bool CanAddMore { get { return _handCards.Count < 10; } }

        private Deck _deck;
        // Use this for initialization
        void Start()
        {
            _deck = FindObjectOfType<Deck>();
            _handCards = new List<Card>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnMouseDown()
        {
            if (CanAddMore)
                AddCard(_deck.GetTopCard());
        }

        void AddCard(Card card)
        {
            if (_handCards.Count < 10)
            {
                card.transform.position = new Vector3(card.transform.position.x, card.transform.position.y, -10);
                card.MoveTo(transform.position + new Vector3(Util.NextGaussian(PositionalRandomness), Util.NextGaussian(PositionalRandomness), -_handCards.Count * DepthSpacing));
                card.RotateTo(Util.NextGaussian(RotationalRandomness) * 180);
                _handCards.Add(card);
                card.transform.parent = transform;
            }
        }
    }
}
