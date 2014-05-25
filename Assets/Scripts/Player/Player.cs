using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Cards;
using Assets.Scripts.Game;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class Player : MonoBehaviour
    {
        public HandArea Hand { get; private set; }
        public TableArea Table { get; private set; }

        private static int _playerNumber = 1;

        public int Number;
        public bool HasPickedUpCards { get; private set; }

        public float HandSpread = 1;
        public float HandArc = -5;

        public bool CanAddMore { get { return Hand.CanAddMore || Table.CanAddMore; } }

        public bool Dealer = false;


        // Use this for initialization
        void Start()
        {
            Number = _playerNumber++;
            if (Number == 2)
            {
                foreach (var tf in transform.GetComponentsInChildren<Transform>())
                {
                    tf.position = new Vector3(tf.position.x, -tf.position.y, tf.position.z);
                }
            }
            Hand = GetComponentInChildren<HandArea>();
            Table = GetComponentInChildren<TableArea>();
        }

        // Update is called once per frame
        void Update()
        {

        }


        public void PickUpHand()
        {
            var suitedCards = Hand.Cards.GroupBy(a => a.Suit).Select(a => a.ToList()).ToList();
            suitedCards.Sort((a, b) => a[0].Suit.CompareTo(b[0].Suit));
            if (suitedCards.Count == 3)
            {
                if ((suitedCards[2].First().Suit - suitedCards[0].First().Suit)%2 != 0)
                {
                    suitedCards.Swap(0,1);
                }

                //if the first swap didn't put the right one in the center
                if ((suitedCards[2].First().Suit - suitedCards[0].First().Suit) % 2 != 0)
                {
                    suitedCards.Swap(1, 2);
                }
            }

            Hand.Cards = new List<Card>();
            foreach (var suit in suitedCards)
            {
                suit.Sort((a, b) => a.Rank.CompareTo(b.Rank));
                Hand.Cards.AddRange(suit);
            }


            for (int i = 0; i < Hand.Cards.Count; i++)
            {
                var targetPos = Hand.transform.position + new Vector3(HandSpread * (-4.5f + i), -HandArc * (Mathf.Pow(.1f * (i - 4.5f), 2)), -i);
                var targetRot = HandArc * (4.5f - i);
                var thisCard = Hand.Cards[i];
                thisCard.Flip();
                thisCard.RotateTo(targetRot, true);
                thisCard.MoveTo(targetPos);
            }
            HasPickedUpCards = true;
        }

        public void GiveCard(Card card)
        {
            if (Table.CanAddMore)
                Table.AddCard(card);
            else
                Hand.AddCard(card);
        }

    }
}
