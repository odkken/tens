﻿using System;
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

        public int Pointvalue
        {
            get
            {
                return Rank == CardRank.Ace || Rank == CardRank.Ten ? 10 : Rank == CardRank.Five ? 5 : 0;
            }
        }

        public bool Flipping { get; private set; }
        public bool Moving { get; private set; }
        public bool Rotating { get; private set; }
        void Start()
        {
            Flipping = false;
            Moving = false;
            Rotating = false;
            var splitName = name.Replace("of", ",").Split(',');
            Rank = (CardRank)Enum.Parse(typeof(CardRank), splitName[0], true);
            Suit = (CardSuit)Enum.Parse(typeof(CardSuit), splitName[1], true);
        }

        void Update()
        {
            enabled = false;
        }


        void OnMouseDown()
        {
            if (Owner.IsLocalPlayer && IsFaceUp)
                Owner.OnCardClicked(GetComponent<Card>());
        }

        //for sliding the thing around
        //void OnMouseDown()
        //{
        //    if (!IsFaceUp || !Owner.IsLocalPlayer) return;
        //    _initClickOffset = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        //    RotateTo(Mathf.Abs(transform.rotation.eulerAngles.z - 180) < 90 ? 180 : 0, true, false);
        //}

        //void OnMouseDrag()
        //{
        //    if (!IsFaceUp || !Owner.IsLocalPlayer) return;
        //    Moving = true;
        //    var newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition - _initClickOffset);
        //    newPos.z = transform.position.z;
        //    transform.position = newPos;
        //}

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

        public override string ToString()
        {
            return Rank + " of " + Suit;
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
            var startRot = transform.rotation;
            var dummyObject = new GameObject();
            dummyObject.transform.rotation = startRot;
            dummyObject.transform.RotateAround(dummyObject.transform.position, dummyObject.transform.up, 180);
            var endRot = dummyObject.transform.rotation;
            var time = 0f;
            while (time < 1)
            {
                time += Time.deltaTime / flipTime;
                transform.rotation = Quaternion.Slerp(startRot, endRot, Mathf.Pow(time, 2));
                yield return null;
            }
            Destroy(dummyObject);
            transform.rotation = endRot;
            Flipping = false;
        }

        public void MoveTo(Vector3 to)
        {
            if (!Moving)
            {
                Moving = true;
                StartCoroutine(AnimateMove(to, AnimTime));
            }
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

        public void RotateToQuat(Quaternion newRotation)
        {
            if (!Rotating)
            {
                Rotating = true;
                StartCoroutine(AnimateRotateQuat(newRotation, AnimTime));
            }
        }

        IEnumerator AnimateRotateQuat(Quaternion newRotation, float flipTime)
        {
            Rotating = true;
            var time = 0f;
            var startRot = transform.rotation;
            var endRot = newRotation;
            while (time < 1)
            {
                time += Time.deltaTime / flipTime;
                transform.rotation = Quaternion.Slerp(startRot, endRot, Mathf.Log10(time * 9 + 1));
                yield return null;
            }
            Rotating = false;
        }

        public void RotateTo(float angle, bool shortestPath, bool flip)
        {
            if (!Rotating)
            {
                Rotating = true;
                StartCoroutine(AnimateRotate(angle, AnimTime, shortestPath, flip));
            }
        }
        IEnumerator AnimateRotate(float to, float flipTime, bool shortestPath, bool flip)
        {
            Rotating = true;
            var time = 0f;

            if (shortestPath)
            {
                var startRot = transform.rotation;
                var dummyObject = new GameObject();
                dummyObject.transform.rotation = startRot;
                dummyObject.transform.Rotate(0, 0, to - startRot.eulerAngles.z);
                if (flip)
                    dummyObject.transform.RotateAround(dummyObject.transform.position, dummyObject.transform.up, 180);
                var endRot = dummyObject.transform.rotation;
                while (time < 1)
                {
                    time += Time.deltaTime / flipTime;
                    transform.rotation = Quaternion.Slerp(startRot, endRot, Mathf.Log10(time * 9 + 1));
                    yield return null;
                }
                Destroy(dummyObject);
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
