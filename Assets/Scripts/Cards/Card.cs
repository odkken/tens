using System;
using Assets.Scripts.Game;
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

        private Vector3 InitClickOffset;

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
        }

        void OnMouseDown()
        {
            if (!IsFaceUp) return;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            InitClickOffset = hit.point - Camera.main.WorldToScreenPoint(transform.position);
        }

        void OnMouseDrag()
        {
            if (!IsFaceUp) return;
            var ray = Input.mousePosition;
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            if (hit.collider.transform == transform)
                transform.position = hit.point - InitClickOffset;
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
            if (!Flipping)
                StartCoroutine(AnimateFlip(AnimTime));
        }

        IEnumerator AnimateFlip(float flipTime)
        {
            Flipping = true;
            var startAngle = transform.rotation.eulerAngles.y;
            var endAngle = startAngle - 180;
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

        public void RotateTo(float angle)
        {
            if (!Rotating)
                StartCoroutine(AnimateRotate(angle, AnimTime));
        }
        IEnumerator AnimateRotate(float to, float flipTime)
        {
            Rotating = true;
            var startAngle = transform.rotation.eulerAngles.z;
            var time = 0f;
            while (time < 1)
            {
                time += Time.deltaTime / flipTime;
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Mathf.Lerp(startAngle, to, Mathf.Log10(time * 9 + 1)));
                yield return null;
            }
            Rotating = false;
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
