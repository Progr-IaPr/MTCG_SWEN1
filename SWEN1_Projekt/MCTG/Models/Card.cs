using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTCG.Models
{
    public class Card
    {
        //enum to determine whether it's a spell or monster card
        public enum CardType
        {
           Spell = 1,
           Goblin,
           Dragon,
           Wizzard,
           Ork,
           Knight,
           Kraken,
           Elf,
           Troll,
        }

        //enum to determine which element type the card has
        public enum CardElement
        {
            Regular = 1,
            Water,
            Fire,
        }

        public string Id { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public int Element { get; set; }
        public float Damage { get; set; }
    }
}


