using UnityEngine;
using Assets.Scripts.Cards;
using System.Collections;
namespace Assets.Scripts.Cards
{
    public class CardObject : MonoBehaviour
    {

        private Card card;

        private bool faceUp;
        private bool flipping;
        // Use this for initialization
        void Start()
        {
            flipping = false;
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void SetCard(Card newCard)
        {
            card = newCard;
        }

        public void Flip(float flipTime)
        {
            if (!flipping)
            {
                card.Flip();
                faceUp = card.FacingUp;
                StartCoroutine(AnimateFlip(flipTime));
            }
        }

        public bool IsFaceUp()
        {
            return card.FacingUp;
        }

        public Card.CardRank Rank()
        {
            return card.Rank;
        }

        public Card.CardSuit Suit()
        {
            return card.Suit;
        }

        IEnumerator AnimateFlip(float flipTime)
        {
            flipping = true;
            var startingRotation = transform.rotation;
            var targetRotation = Quaternion.LookRotation(faceUp ? Vector3.up : Vector3.down);
            targetRotation.x = 0;
            targetRotation.z = 0;
            var time = 0f;
            while (time < 1)
            {
                time += Time.deltaTime/flipTime;
                transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, time);
                yield return null;
            }
            transform.rotation = targetRotation;
            flipping = false;
        }


    }
}
