using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Cards;
using Assets.Scripts.Common;
namespace Assets.Scripts.Game
{
    public class Player : MonoBehaviour
    {
        private List<Card> cardsInHand;
        private List<Card> cardsOnTable;

        public List<Card> AllCards { get { return cardsInHand.Concat(cardsOnTable).ToList(); } }

        // Use this for initialization
        void Start()
        {
            cardsInHand = new List<Card>();
            cardsOnTable = new List<Card>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void GiveCard(Card card, bool inHand)
        {
            if (inHand)
                cardsInHand.Add(card);
            else
                cardsOnTable.Add(card);
        }

    }
}
