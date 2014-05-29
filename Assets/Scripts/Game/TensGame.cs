using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.Cards;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class TensGame : MonoBehaviour
    {
        public enum GameState
        {
            Deal, Bid, HandPlay, TablePlay
        }

        public int MaxPlayers;

        public Player.Player PlayerTemplate;

        public Player.Player DealerPlayer;
        public int CardsPlayedFromHand { get; private set; }
        public int CardsPlayedFromTable { get; private set; }

        public float TimeLastCardPlayed;

        public List<Card> CardsPlayedThisRound;
        public List<Card> CardsPlayedThisHand;
        public CardSuit TrumpSuit { get; private set; }

        private Deck _deck;
        private Deck Deck
        {
            get
            {
                if (_deck == null) _deck = FindObjectOfType<Deck>();
                return _deck;
            }
        }

        [HideInInspector]
        public List<Player.Player> AllPlayers;

        public GameState CurrentState;

        void Start()
        {
            CardsPlayedThisHand = new List<Card>();
        }

        void Update()
        {
            if (!UnityEngine.Network.isServer) return;
            switch (CurrentState)
            {
                case GameState.Deal:
                    if (DealerPlayer != null && Deck.DoneDealing)
                        SetGameState((int)GameState.Bid);
                    break;
                case GameState.Bid:
                    if (AllPlayers.All(a => a.HasPickedUpCards))
                        SetGameState((int)GameState.HandPlay);
                    break;
                case GameState.HandPlay:
                    break;
                case GameState.TablePlay:
                    if (!AllPlayers.SelectMany(a => a.Hand.Cards.Concat(a.Table.Cards)).Any())
                    {
                        //tally up scores
                    }

                    else if (CardsPlayedThisRound.Count == 4 && Time.time - TimeLastCardPlayed > (Card.AnimTime * 2))
                    {
                        var winner = GetRoundWinner(CardsPlayedThisRound);
                        winner.AwardCards(CardsPlayedThisRound);
                        winner.SetMyTurn();
                        CardsPlayedThisRound = new List<Card>();
                        SetGameState((int)GameState.HandPlay);
                        networkView.RPC("UncoverCardsRPC", RPCMode.AllBuffered);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnPlayerAction(Player.Player player)
        {
            player.MyTurn = false;
            AllPlayers.Next(AllPlayers.IndexOf(player)).MyTurn = true;
            switch (CurrentState)
            {
                case GameState.Deal:
                    break;
                case GameState.Bid:
                    break;
                case GameState.HandPlay:
                    CardsPlayedFromHand++;
                    if (CardsPlayedFromHand == 2)
                    {
                        SetGameState((int)GameState.TablePlay);
                    }
                    break;
                case GameState.TablePlay:
                    if (CardsPlayedFromTable == 2)
                    {
                        SetGameState((int)GameState.HandPlay);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void PlayCard(Card card)
        {
            TimeLastCardPlayed = Time.time;
            CardsPlayedThisRound.Add(card);
            if (CardsPlayedThisHand.Count == 0)
            {
                SetTrumpSuit(card.Suit);
            }
            CardsPlayedThisHand.Add(card);
            card.MoveTo(Camera.main.transform.right * (CardsPlayedThisRound.Count - 2.5f) + new Vector3(0, 0, CardsPlayedThisRound.Count + 1) * -1);
            if (!card.IsFaceUp)
                card.Flip();
        }

        private void SetTrumpSuit(CardSuit suit)
        {
            TrumpSuit = suit;
            var spriteRenderers = GameObject.Find("TrumpGraphic").GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.transform.RotateAround(spriteRenderer.transform.position, Vector3.forward, Vector3.Angle(Camera.main.transform.up, spriteRenderer.transform.up));
                spriteRenderer.enabled = true;
                spriteRenderer.sprite = Resources.LoadAll<Sprite>("playing_card_suits")[((int)TrumpSuit == 3) ? 2 : ((int)TrumpSuit == 2) ? 3 : (int)TrumpSuit];
            }
        }

        private void ClearTrumpSuit()
        {
            var spriteRenderers = GameObject.Find("TrumpGraphic").GetComponentsInChildren<SpriteRenderer>().ToList();
            spriteRenderers.Sort((a, b) => a.name.CompareTo(b.name));
            for (int i = 0; i < spriteRenderers.Count(); i++)
            {
                var spriteRenderer = spriteRenderers[i];
                spriteRenderer.transform.RotateAround(spriteRenderer.transform.position, Vector3.forward, Vector3.Angle(Camera.main.transform.up, spriteRenderer.transform.up));
                spriteRenderer.sprite = Resources.LoadAll<Sprite>("playing_card_suits")[i];
            }
        }

        private Player.Player GetRoundWinner(List<Card> cards)
        {
            var trumpCards = cards.Where(a => a.Suit == TrumpSuit);
            var trumpCardsArray = trumpCards as Card[] ?? trumpCards.ToArray();
            if (trumpCardsArray.Any())
            {
                var maxTrump = trumpCardsArray.Select(a => a.Rank).Max();
                return trumpCardsArray.Single(a => a.Rank == maxTrump).Owner;
            }

            var cardsFollowingSuit = cards.Where(a => a.Suit == cards.First().Suit);
            var cardsFollowingSuitArray = cardsFollowingSuit as Card[] ?? cardsFollowingSuit.ToArray();
            var maxSuited = cardsFollowingSuitArray.Select(a => a.Rank).Max();
            return cardsFollowingSuitArray.Single(a => a.Rank == maxSuited).Owner;
        }

        [RPC]
        public void SetDealer(int playerIndex)
        {
            networkView.RPC("SetDealerRPC", RPCMode.AllBuffered, playerIndex);
        }

        [RPC]
        private void SetDealerRPC(int playerIndex)
        {
            DealerPlayer = FindObjectsOfType<Player.Player>()[playerIndex];
        }

        public void SetGameState(int state)
        {
            networkView.RPC("SetGameStateRPC", RPCMode.AllBuffered, state);
        }
        [RPC]
        private void SetGameStateRPC(int state)
        {
            CurrentState = (GameState)state;
            switch (CurrentState)
            {
                case GameState.Deal:
                    break;
                case GameState.Bid:
                    break;
                case GameState.HandPlay:
                    CardsPlayedThisRound = new List<Card>();
                    CardsPlayedFromTable = 0;
                    CardsPlayedFromHand = 0;
                    break;
                case GameState.TablePlay:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [RPC]
        private void UncoverCardsRPC()
        {
            var cardsToFlip = AllPlayers.SelectMany(a => a.Table.Cards.Where(b => !b.IsFaceUp && !Physics.RaycastAll(new Ray(b.transform.position, b.transform.forward)).Any(c => c.collider != b.collider)));
            foreach (var card in cardsToFlip)
            {
                Debug.DrawRay(card.transform.position, card.transform.forward);
                card.Flip();
            }
        }


    }
}
