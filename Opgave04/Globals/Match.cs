using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals
{

    public struct Match
    {
        private Player[] players;
      
        public Match(Player winner, Player loser)
        {
            players = new Player[] {winner, loser};
        }              

        public List<Player> Players
        {
            get
            {
                return players.ToList();
            }
        }

        public Player this[MatchResult status]
        {
            get
            {
                if (status == MatchResult.Winner) return players[0];
                return players[1];
            }
        }

    }
}
