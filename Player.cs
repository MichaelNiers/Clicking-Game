using System;
using System.Xml;

namespace Game2
{
    [Serializable]
    public class Player
    {
        public int Rank { get; set; }
        public string Name { get; set; }
        public int Clicks { get; set; }

        public Player(string newName)
        {
            this.Name = newName;
        }

        public Player()
        {
        }
    }
}