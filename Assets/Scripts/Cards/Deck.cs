using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game;
using UnityEngine;
using Assets.Scripts.Common;

namespace Assets.Scripts.Cards
{
    public class Deck : MonoBehaviour
    {
        public Card TemplateCard;
        public List<Card> Cards { get; private set; }
        public int CardsLeft { get { return Cards.Count; } }

        public float CardSpacing = .01f;

        // Use this for initialization
        void Start()
        {
            Cards = new List<Card>();
            foreach (Card.CardRank rank in Enum.GetValues(typeof(Card.CardRank)))
            {
                if ((int)rank > 2)
                {
                    foreach (Card.CardSuit suit in Enum.GetValues(typeof(Card.CardSuit)))
                    {
                        var card = Instantiate(TemplateCard) as Card;
                        card.SetInfo(rank, suit);
                        Cards.Add(card);
                        card.transform.position = transform.position + new Vector3(0, 0, -CardSpacing * Cards.Count);
                        card.transform.parent = transform;
                        card.transform.rotation = Quaternion.Euler(card.transform.eulerAngles.x, card.transform.eulerAngles.y, UnityEngine.Random.Range(0f, 1f) > .5 ? 180 : 0);
                    }
                }
            }
            Shuffle();

            //while (CardsLeft > 0)
            //{
            //    var card = GetTopCard();
            //    card.rigidbody.velocity = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0);
            //}
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnMouseDown()
        {
            TensRules.Deal(null, null, transform.GetComponent<Deck>());
        }


        public void Shuffle()
        {
            Cards.Shuffle();
            for (int i = 0; i < Cards.Count; i++)
            {
                Cards[i].transform.position = transform.position + new Vector3(0, 0, -CardSpacing * i);
            }
        }

        public void ScrapCard(Card.CardRank rank, Card.CardSuit suit)
        {
            var card = Cards.FirstOrDefault(a => a.Rank == rank && a.Suit == suit);
            if (card == null) return;
            Cards.Remove(card);
            Destroy(card);
        }

        public Card GetTopCard()
        {
            var card = Cards.Pop();
            card.transform.parent = null;
            return card;
        }
    }
}
