using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Cards;
using Assets.Scripts.Common;
namespace Assets.Scripts.Game
{
    public class Player : MonoBehaviour
    {
        private List<CardObject> cardsInHand;
        private List<CardObject> cardsOnTable;

        public List<CardObject> AllCards { get { return cardsInHand.Concat(cardsOnTable).ToList(); } }

        // Use this for initialization
        void Start()
        {
            cardsInHand = new List<CardObject>();
            cardsOnTable = new List<CardObject>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void GiveCard(CardObject card, bool inHand)
        {
            if (inHand)
                cardsInHand.Add(card);
            else
                cardsOnTable.Add(card);
        }

    }
}
