using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading;
using Assets.Scripts.Cards;
using Assets.Scripts.Game;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class Player : MonoBehaviour
    {

        public Rect PlusRect;
        public Rect MinusRect;
        public Rect BidRect;
        public Rect PassRect;

        public HandArea Hand { get; private set; }
        public TableArea Table { get; private set; }

        private TensGame _game;
        public TensGame Game
        {
            get { return _game ?? (_game = FindObjectOfType<TensGame>()); }
        }

        private static int _playerNumber = 0;
        public bool MyTurn;

        public string NetworkId;

        public int Points { get; private set; }

        public List<Card> WonCards { get; private set; }

        private Deck _deck;
        private Deck Deck
        {
            get
            {
                if (_deck == null) _deck = FindObjectOfType<Deck>();
                return _deck;
            }
        }
        public int Number;
        public bool IsLocalPlayer = false;

        public float HandSpread = .7f;

        public bool CanAddMore { get { return Hand.CanAddMore || Table.CanAddMore; } }


        public bool Dealer = false;
        public bool HasPickedUpCards { get; private set; }
        public bool KeptBid;
        public int Bid { get; private set; }
        private int _minBid = 50;


        // Use this for initialization
        void Start()
        {
            _minBid = 50;
            Bid = 50;
            var sWidth = Screen.width;
            var sHeight = Screen.height;
            var bidWith = 200f;
            var bidHeight = 100f;
            BidRect = new Rect((sWidth - bidWith) * .5f, (sHeight - bidHeight) * .5f, bidWith, bidHeight);
            PlusRect = new Rect(BidRect.xMin - bidWith / 2, BidRect.yMin, bidWith / 2, bidHeight / 2);
            MinusRect = new Rect(PlusRect.xMin, PlusRect.yMax, bidWith / 2, bidHeight / 2);
            PassRect = new Rect(BidRect.xMax, BidRect.yMin, bidWith, bidHeight);
            Debug.Log("Spawning on " + (UnityEngine.Network.isServer ? "Server" : "Client") + " " + networkView.viewID);
            Game.AllPlayers.Add(GetComponent<Player>());
            Number = _playerNumber++;
            if (Number == 1)
            {
                foreach (var tf in transform.GetComponentsInChildren<Transform>())
                {
                    tf.position = new Vector3(tf.position.x, -tf.position.y, tf.position.z);
                }
            }
            WonCards = new List<Card>();
            Hand = GetComponentInChildren<HandArea>();
            Table = GetComponentInChildren<TableArea>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void PickUpHand()
        {
            networkView.RPC("PickUpHandRPC", RPCMode.AllBuffered);
        }

        public void OnCardClicked(Card card)
        {
            if (!MyTurn) return;
            var canPlay = false;
            if (!Game.CardsPlayedThisRound.Any())
                canPlay = true;
            else if (card.Suit == Game.CardsPlayedThisRound.First().Suit)
                canPlay = true;
            else if (Hand.Cards.Contains(card))
            {
                canPlay = !Hand.Cards.Any(a => a.Suit == Game.CardsPlayedThisRound.First().Suit);
            }
            else
            {
                canPlay = !Table.Cards.Any(a => a.IsFaceUp && a.Suit == Game.CardsPlayedThisRound.First().Suit);
            }
            if (!canPlay) return;
            switch (Game.CurrentState)
            {
                case TensGame.GameState.Deal:
                    break;
                case TensGame.GameState.Bid:
                    break;
                case TensGame.GameState.HandPlay:
                    if (Hand.Cards.Contains(card))
                        networkView.RPC("PlayCardRPC", RPCMode.AllBuffered, card.ToInt());
                    break;
                case TensGame.GameState.TablePlay:
                    if (Table.Cards.Contains(card))
                        networkView.RPC("PlayCardRPC", RPCMode.AllBuffered, card.ToInt());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void AwardCards(List<Card> cards)
        {
            //Thread.Sleep((int)(Card.AnimTime * 2 * 1000));
            networkView.RPC("AwardCardsRPC", RPCMode.AllBuffered, cards.Select(a => a.ToInt()).ToArray());
        }

        [RPC]
        private void AwardCardsRPC(int[] cardInts)
        {
            var i = 0;
            foreach (var card in FindObjectsOfType<Card>().Where(a => cardInts.Contains(a.ToInt())))
            {
                WonCards.Add(card);
                card.MoveTo(Hand.transform.position + new Vector3(Util.NextGaussian(Hand.PositionalRandomness), Util.NextGaussian(Hand.PositionalRandomness), -(i + 1) * .5f));
                card.RotateTo(Util.NextGaussian(Hand.RotationalRandomness) * 180, true, true);
                ++i;
            }
        }

        [RPC]
        void PlayCardRPC(int cardData)
        {
            Card card;
            if (Hand.Cards.Any(a => a.ToInt() == cardData))
            {
                card = Hand.Cards.Single(a => a.ToInt() == cardData);
                Hand.Cards.Remove(card);
            }
            else
            {
                card = Table.Cards.Single(a => a.ToInt() == cardData);
                Table.Cards.Remove(card);
            }
            Game.PlayCard(card);
            Game.OnPlayerAction(GetComponent<Player>());
        }


        [RPC]
        public void PickUpHandRPC()
        {
            var suitedCards = Hand.Cards.GroupBy(a => a.Suit).Select(a => a.ToList()).ToList();
            suitedCards.Sort((a, b) => a[0].Suit.CompareTo(b[0].Suit));
            if (suitedCards.Count == 3)
            {
                if ((suitedCards[2].First().Suit - suitedCards[0].First().Suit) % 2 != 0)
                {
                    suitedCards.Swap(0, 1);
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
                var straightLinePos = Hand.transform.position + HandSpread * (-4.5f + i) * Hand.transform.right + new Vector3(0, 0, -(i + .5f));
                var dummyObject = new GameObject();
                dummyObject.transform.rotation = transform.rotation;
                if (!IsLocalPlayer)
                    dummyObject.transform.RotateAround(dummyObject.transform.position, dummyObject.transform.up, 180);
                dummyObject.transform.position = straightLinePos;
                dummyObject.transform.RotateAround(Hand.transform.position, transform.forward, HandSpread * 2 * (4.5f - i));
                var thisCard = Hand.Cards[i];
                thisCard.MoveTo(dummyObject.transform.position);
                thisCard.RotateToQuat(dummyObject.transform.rotation);
                Destroy(dummyObject);
            }
            HasPickedUpCards = true;
        }

        public void GiveCard()
        {
            if (Table.CanAddMore)
                Table.AddCard();
            else
                Hand.AddCard();
        }

        public void SetDealer(int[] shuffleSeeds)
        {
            var deck = Resources.Load<Deck>("Prefabs/Deck");
            var newDeck = UnityEngine.Network.Instantiate(deck, Vector3.zero, Quaternion.identity, 0) as Deck;
            newDeck.SetSeeds(shuffleSeeds);
            networkView.RPC("SetDealerRPC", RPCMode.AllBuffered);
        }
        [RPC]
        private void SetDealerRPC()
        {
            Dealer = true;
            FindObjectOfType<TensGame>().DealerPlayer = GetComponent<Player>();
        }

        public void SetMyTurn()
        {
            networkView.RPC("SetMyTurnRPC", RPCMode.AllBuffered);
        }
        [RPC]
        private void SetMyTurnRPC()
        {
            Game.AllPlayers.Single(a => a.MyTurn).MyTurn = false;
            MyTurn = true;
        }

        public void AddPoints(int points)
        {
            networkView.RPC("AddPointsRPC", RPCMode.AllBuffered, points);
        }
        [RPC]
        private void AddPointsRPC(int points)
        {
            Points += points;
        }

        [RPC]
        private void SetBidRPC(int bid)
        {
            MyTurn = false;
            Game.AllPlayers.Next(Game.AllPlayers.IndexOf(this)).MyTurn = true;
            Bid = bid;
            foreach (var player in Game.AllPlayers.Where(a => a != this))
            {
                player.KeptBid = false;
                player._minBid = bid + 5;
                player.Bid = bid + 5;
            }
            KeptBid = true;
        }

        [RPC]
        private void PassRPC()
        {
            MyTurn = false;
            Game.AllPlayers.Single(a => a.KeptBid).MyTurn = true;
            Game.SetGameState((int)TensGame.GameState.HandPlay);
        }

        void OnGUI()
        {
            if (!IsLocalPlayer) return;
            if (Game.CurrentState == TensGame.GameState.Bid && Game.AllPlayers.All(a => a.HasPickedUpCards))
            {
                GUI.enabled = MyTurn;
                var buttonStyle = GUI.skin.button;
                buttonStyle.fontSize = 50;
                if (GUI.Button(PlusRect, "+", buttonStyle))
                {
                    if (Bid < 100)
                        Bid += 5;
                    return;
                }
                if (GUI.Button(MinusRect, "-", buttonStyle))
                {
                    if (Bid > _minBid)
                        Bid -= 5;
                    return;
                }
                if (GUI.Button(BidRect, "Bid: " + Bid, buttonStyle))
                {
                    networkView.RPC("SetBidRPC", RPCMode.AllBuffered, Bid);
                    return;
                }
                if (GUI.Button(PassRect, "Pass", buttonStyle))
                {
                    networkView.RPC("PassRPC", RPCMode.AllBuffered);
                }
            }
            var scoreWidth = 200f;
            var scoreHeight = 100f;
            var labelStyle = GUI.skin.label;
            labelStyle.fontSize = 50;
            GUI.Label(new Rect((Screen.width - scoreWidth) / 2, Screen.height - scoreHeight, scoreWidth, scoreHeight), "Score: " + Points, labelStyle);
        }

    }

}
