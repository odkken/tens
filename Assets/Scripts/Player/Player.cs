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

        private static int _playerNumber = 0;

        public string NetworkId;

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
        public bool HasPickedUpCards { get; private set; }

        public float HandSpread = .7f;
        public float HandRotation = 2;

        public bool CanAddMore { get { return Hand.CanAddMore || Table.CanAddMore; } }

        public bool Dealer = false;
        public bool IsLocalPlayer = false;


        // Use this for initialization
        void Start()
        {
            Debug.Log("Spawning on " + (UnityEngine.Network.isServer ? "Server" : "Client") + " " + networkView.viewID);
            Number = _playerNumber++;
            if (Number == 1)
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
            networkView.RPC("PickUpHandRPC", RPCMode.AllBuffered);
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
                dummyObject.transform.position = straightLinePos;
                dummyObject.transform.RotateAround(Hand.transform.position, transform.forward, HandSpread * (4.5f - i));
                var thisCard = Hand.Cards[i];
                thisCard.RotateTo(dummyObject.transform.rotation.eulerAngles.z * HandRotation * Vector3.Dot(Camera.main.transform.right, transform.right), true, IsLocalPlayer);
                thisCard.MoveTo(dummyObject.transform.position);
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

        public void SetDealer(int shuffleSeed)
        {
            var deck = Resources.Load<Deck>("Prefabs/Deck");
            var newDeck = UnityEngine.Network.Instantiate(deck, Vector3.zero, Quaternion.identity, 0) as Deck;
            newDeck.SetSeed(shuffleSeed);
            networkView.RPC("SetDealerRPC", RPCMode.AllBuffered, shuffleSeed);
        }
        [RPC]
        private void SetDealerRPC(int shuffleSeed)
        {
            Dealer = true;
            FindObjectOfType<TensGame>().DealerPlayer = GetComponent<Player>();
        }
    }

}
