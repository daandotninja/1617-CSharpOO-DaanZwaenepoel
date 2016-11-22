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
        private Player[] ranking;


        public LogicImplementation(DataAccessImplementation dataAccessImplementation)
        {
            this.dataAccessImplementation = dataAccessImplementation;
            ranking = dataAccessImplementation.ReadRanking();
        }

        public Player[] Ranking
        {
            get
            {
                Player[] rankingCopy = new Player[ranking.Count()];
                Array.Copy(ranking, rankingCopy, ranking.Length);
                return rankingCopy;
            }
        }

        public void Close()
        {
            dataAccessImplementation.SaveRanking(ranking);
        }

        public bool IsValidMatch(Player playerA, Player playerB)
        {
            
            int posA = Array.IndexOf(Ranking, playerA);
            int posB = Array.IndexOf(Ranking, playerB);
            if ((posA - posB) < 3 && playerA.Name != playerB.Name)
            {
                return true;
            }
            else
            {
                return false;
            }
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
                    ranking[posWinner] = loser;
                    ranking[posLoser] = winner;
                }
                dataAccessImplementation.SaveRanking(ranking);
               

            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
            
        }
    }
}
