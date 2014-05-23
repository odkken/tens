using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts.Cards
{
    public class Card
    {
        public Card(CardRank rank, CardSuit suit)
        {
            Rank = rank;
            Suit = suit;
            Name = rank + " of " + Suit;
            BackTexture = new Texture2D(10,10);
            FrontTexture = new Texture2D(10,10);
        }

        public Texture2D BackTexture { get; private set; }
        public Texture2D FrontTexture { get; private set; }
        
        public CardSuit Suit { get; private set; }
        public CardRank Rank { get; private set; }
        
        public string Name { get; private set; }
        
        public bool FacingUp { get; private set; }

        public void Flip()
        {
            FacingUp = !FacingUp;
        }

        public void SetFaceUp()
        {
            FacingUp = true;
        }

        public void SetFaceDown()
        {
            FacingUp = false;
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
