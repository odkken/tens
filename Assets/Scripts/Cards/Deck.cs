using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game;
using Assets.Scripts.Player;
using UnityEngine;
using Assets.Scripts.Common;

namespace Assets.Scripts.Cards
{
    public class Deck : MonoBehaviour
    {
        public Card TemplateCard;
        public List<Card> Cards { get; private set; }
        public int CardsLeft { get { return Cards.Count; } }
        public float DealTime = 10f;
        private const float CardSpacing = .01f;
        public bool Dealing { get; private set; }

        // Use this for initialization
        void Start()
        {
            Initialize();
        }

        void Update()
        {

            switch (TensGame.CurrentState)
            {
                case TensGame.GameState.Deal:
                    if (CardsLeft == 0)
                    {
                        TensGame.CurrentState = TensGame.GameState.Bid;
                        transform.position = new Vector3(-999, -999, -999);
                    }
                        
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

        void Initialize()
        {
            Dealing = false;
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
        }


        void OnMouseDown()
        {
            if (TensGame.CurrentState == TensGame.GameState.Deal && !Dealing)
                Deal();
        }

        private void Deal()
        {
            Dealing = true;
            StartCoroutine(AnimateDeal(DealTime));
        }

        IEnumerator AnimateDeal(float dealTime)
        {
            int dealTo = 0;
            int totalCards = CardsLeft;
            var players = FindObjectsOfType<Player.Player>().ToList();
            while (CardsLeft > 0)
            {
                float waitTime = 0;
                var receivingPlayer = players[dealTo++];
                if (receivingPlayer.CanAddMore)
                {
                    receivingPlayer.GiveCard(GetTopCard());
                    waitTime = dealTime / totalCards;
                }
                if (dealTo == players.Count())
                    dealTo = 0;
                yield return new WaitForSeconds(waitTime);
            }
            Dealing = false;
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
