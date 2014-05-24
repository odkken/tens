using System;
using UnityEngine;
using Assets.Scripts.Cards;
using System.Collections;
namespace Assets.Scripts.Cards
{
    public class Card : MonoBehaviour
    {

        public static float AnimTime = .5f;
        public bool IsFaceUp { get { return Vector3.Dot(transform.forward, Vector3.forward) > 0; } }

        public CardSuit Suit { get; private set; }
        public CardRank Rank { get; private set; }

        public string Name { get; private set; }

        private bool _flipping;
        private bool _moving;
        private bool _rotating;
        void Start()
        {
            _flipping = false;
            _moving = false;
            _rotating = false;
        }

        void Update()
        {
        }

        public void SetInfo(CardRank rank, CardSuit suit)
        {
            Rank = rank;
            Suit = suit;
            var rankChar = "";
            if ((int)rank < 9)
                rankChar = ((int)rank + 2).ToString();
            else
            {
                switch ((int)rank)
                {
                    case 9:
                        rankChar = "J";
                        break;
                    case 10:
                        rankChar = "Q";
                        break;
                    case 11:
                        rankChar = "K";
                        break;
                    case 12:
                        rankChar = "A";
                        break;
                }
            }

            var cardString = "card" + suit + rankChar;
            var frontSprite = Resources.Load<Sprite>("Cards/" + cardString);
            if (frontSprite == null)
                throw new Exception("Couldn't load " + cardString);
            transform.FindChild("Front").GetComponent<SpriteRenderer>().sprite = frontSprite;
        }

        public void Flip()
        {
            if (!_flipping)
                StartCoroutine(AnimateFlip(AnimTime));
        }

        IEnumerator AnimateFlip(float flipTime)
        {
            _flipping = true;
            var startAngle = transform.rotation.eulerAngles.y;
            var endAngle = startAngle - 180;
            var time = 0f;
            while (time < 1)
            {
                time += Time.deltaTime / flipTime;
                transform.rotation = Quaternion.Euler(0, Mathf.Lerp(startAngle, endAngle, Mathf.Pow(time, 2)), 0);
                yield return null;
            }
            transform.rotation = Quaternion.Euler(0, endAngle, 0);
            _flipping = false;
        }

        public void MoveTo(Vector3 to)
        {
            if (!_moving)
                StartCoroutine(AnimateMove(to, AnimTime));
        }

        IEnumerator AnimateMove(Vector3 newPosition, float flipTime)
        {
            _moving = true;
            var startPos = transform.position;
            var time = 0f;
            while (time < 1)
            {
                time += Time.deltaTime / flipTime;
                transform.position = Vector3.Lerp(startPos, newPosition, Mathf.Pow(time, 2));
                yield return null;
            }
            transform.position = newPosition;
            _moving = false;
        }

        public void RotateTo(float angle)
        {
            if (!_rotating)
                StartCoroutine(AnimateRotate(angle, AnimTime));
        }
        IEnumerator AnimateRotate(float to, float flipTime)
        {
            _rotating = true;
            var startAngle = transform.rotation.eulerAngles.z;
            var time = 0f;
            while (time < 1)
            {
                time += Time.deltaTime / flipTime;
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Mathf.Lerp(startAngle, to, 1 - Mathf.Log10(time * 9 + 1)));
                yield return null;
            }
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, to);
            _rotating = false;
        }

        public enum CardSuit
        {
            Spades, Hearts, Clubs, Diamonds
        }

        public enum CardRank
        {
            Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace
        }

    }
}
