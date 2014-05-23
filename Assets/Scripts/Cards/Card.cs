using UnityEngine;

namespace Assets.Scripts.Cards
{
    public class Card
    {
        public Card(CardRank rank, CardSuit suit)
        {
            Rank = rank;
            Suit = suit;
            Name = rank.ToString() + " of " + Suit.ToString();
            BackTexture = new Texture2D(10,10);
            FrontTexture = new Texture2D(10,10);
        }

        public Texture2D BackTexture { get; private set; }
        public Texture2D FrontTexture { get; private set; }
        
        public CardSuit Suit { get; private set; }
        public CardRank Rank { get; private set; }
        
        public string Name { get; private set; }
        
        public bool FacingUp { get; private set; }

        void Flip()
        {
            FacingUp = !FacingUp;
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
