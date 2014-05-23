using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Common;

namespace Assets.Scripts.Cards
{
    public class Deck : MonoBehaviour
    {

        public List<Card> Cards { get; private set; }  

        // Use this for initialization
        void Start () {
            Cards = new List<Card>();
            foreach (Card.CardRank rank in Enum.GetValues(typeof(Card.CardRank)))
            {
                foreach (Card.CardSuit suit in Enum.GetValues(typeof(Card.CardSuit)))
                {
                    Cards.Add(new Card(rank, suit));
                }
            }
        }
	
        // Update is called once per frame
        void Update () {
	
        }

        void Shuffle()
        {
            Util.Shuffle(Cards);
        }

    }
}
