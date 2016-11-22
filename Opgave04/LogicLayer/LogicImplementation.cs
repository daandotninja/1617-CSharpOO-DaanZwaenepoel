using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Globals;
using DataAccessLayer;

namespace LogicLayer
{
    public class LogicImplementation : ILogic
    {
        private DataAccessImplementation dataAccessImplementation;

  

        public LogicImplementation(DataAccessImplementation dataAccessImplementation)
        {
            this.dataAccessImplementation = dataAccessImplementation;
        }

        public Player[] Ranking
        {
            get
            {
                return dataAccessImplementation.ReadRanking();
            }
        }

        public void Close()
        {
            dataAccessImplementation.SaveRanking(Ranking);
        }

        public bool IsValidMatch(Player playerA, Player playerB)
        {
            if((playerA.Age - playerB.Age)<3 && playerA.Name != playerB.Name)
            {
                return true;
            }
            return false;
        }

        public void RegisterMatch(Player winner, Player loser)
        {
            if(IsValidMatch(winner, loser))
            {
                Match match = new Match(winner, loser);
                dataAccessImplementation.LogMatch(match);
                int posWinner = Array.IndexOf(Ranking, winner);
                int posLoser = Array.IndexOf(Ranking, loser);
                if (posLoser > posWinner)
                {
                    Ranking[posWinner] = loser;
                    Ranking[posLoser] = winner;
                }
               

            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
            
        }
    }
}
