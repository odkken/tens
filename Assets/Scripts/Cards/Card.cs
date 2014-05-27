using System;
using Assets.Scripts.Game;
using UnityEngine;
using Assets.Scripts.Cards;
using System.Collections;

namespace Assets.Scripts.Cards
{
    public class Card : MonoBehaviour
    {
        public static float AnimTimeout = 20;
        public static float AnimTime = .5f;
        public bool IsFaceUp { get { return Vector3.Dot(transform.forward, Vector3.forward) > 0; } }

        public Player.Player Owner { get; private set; }

        public CardSuit Suit { get; private set; }
        public CardRank Rank { get; private set; }

        public string Name { get; private set; }

        private Vector3 _initClickOffset;

        public bool Flipping { get; private set; }
        public bool Moving { get; private set; }
        public bool Rotating { get; private set; }
        void Start()
        {
            Flipping = false;
            Moving = false;
            Rotating = false;
        }

        void Update()
        {
            enabled = false;
        }

        void OnMouseDown()
        {
            if (!IsFaceUp) return;
            _initClickOffset = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
            RotateTo(Mathf.Abs(transform.rotation.eulerAngles.z - 180) < 90 ? 180 : 0, true);
        }

        void OnMouseDrag()
        {
            if (!IsFaceUp) return;
            Moving = true;
            var newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition - _initClickOffset);
            newPos.z = transform.position.z;
            transform.position = newPos;
        }

        void OnMouseUp()
        {
            Moving = false;
        }

        public void AssignOwner(Player.Player player)
        {
            Owner = player;
        }


        public int ToInt()
        {
            return (int)Suit * 13 + (int)Rank;
        }

        public static void FromInt(int encodedInfo, out CardRank rank, out CardSuit suit)
        {
            suit = (CardSuit)(int)Math.Floor(encodedInfo / 13f);
            rank = (CardRank)(encodedInfo % 13);
        }
        public void SetInfo(CardRank rank, CardSuit suit)
        {
            SetInfo((int)rank, (int)suit);
        }

        private void SetInfo(int rank, int suit)
        {
            Rank = (CardRank)rank;
            Suit = (CardSuit)suit;
            var rankChar = "";
            if ((int)Rank < 9)
                rankChar = ((int)Rank + 2).ToString();
            else
            {
                switch ((int)Rank)
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

            var cardString = "card" + Suit + rankChar;
            var frontSprite = Resources.Load<Sprite>("Cards/" + cardString);
            if (frontSprite == null)
                throw new Exception("Couldn't load " + cardString);
            transform.FindChild("Front").GetComponent<SpriteRenderer>().sprite = frontSprite;
        }

        public void Flip()
        {
            if (!Flipping)
                StartCoroutine(AnimateFlip(AnimTime));
        }

        IEnumerator AnimateFlip(float flipTime)
        {
            Flipping = true;
            var startAngle = transform.rotation.eulerAngles.y;
            var endAngle = Math.Abs(startAngle - 180) < 90 ? 0 : 180;
            var time = 0f;
            while (time < 1)
            {
                time += Time.deltaTime / flipTime;
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, Mathf.Lerp(startAngle, endAngle, Mathf.Pow(time, 2)), transform.rotation.eulerAngles.z);
                yield return null;
            }
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, endAngle, transform.rotation.eulerAngles.z);
            Flipping = false;
        }

        public void MoveTo(Vector3 to)
        {
            if (!Moving)
                StartCoroutine(AnimateMove(to, AnimTime));
        }

        IEnumerator AnimateMove(Vector3 newPosition, float flipTime)
        {
            Moving = true;
            var startPos = transform.position;
            var time = 0f;
            while (time < 1)
            {
                time += Time.deltaTime / flipTime;
                transform.position = Vector3.Lerp(startPos, newPosition, Mathf.Log10(time * 9 + 1));
                yield return null;
            }
            transform.position = newPosition;
            Moving = false;
        }

        public void RotateTo(float angle, bool shortestPath)
        {
            if (!Rotating)
                StartCoroutine(AnimateRotate(angle, AnimTime, shortestPath));
        }
        IEnumerator AnimateRotate(float to, float flipTime, bool shortestPath)
        {
            Rotating = true;
            var time = 0f;

            if (shortestPath)
            {
                var startRot = transform.rotation;
                var endRot = Quaternion.Euler(startRot.x, startRot.y, to);
                while (time < 1)
                {
                    time += Time.deltaTime / flipTime;
                    transform.rotation = Quaternion.Slerp(startRot, endRot, Mathf.Log10(time * 9 + 1));
                    yield return null;
                }
            }
            else
            {
                var startAngle = transform.rotation.eulerAngles.z;
                while (time < 1)
                {
                    time += Time.deltaTime / flipTime;
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                        transform.rotation.eulerAngles.y, Mathf.Lerp(startAngle, to, Mathf.Log10(time * 9 + 1)));
                    yield return null;
                }
            }
            Rotating = false;
        }


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
