using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2
{
    [Serializable]
    public class Ranking
    {
        public List<Player> Members { get; set; }
        public Ranking()
        {

            Members = new List<Player>();
            this.Members.OrderBy(p => p.Clicks);
        } 
        public void Add(Player p)
        {
            Members.Add(p);
        }
    }
}
