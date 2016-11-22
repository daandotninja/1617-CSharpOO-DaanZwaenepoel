using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals
{
    public interface ILogic
    {
        /// <summary>
        /// Current Ranking
        /// </summary>
        /// <remarks>
        /// Returns a clone of the actual ranking.
        /// </remarks>        /// 
        Player[] Ranking { get; }

        /// <summary>
        /// Registers a match result
        /// </summary>
        /// <param name="winner">player that has won</param>
        /// <param name="loser">player that has lost</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// is thrown if player combination is not valid
        /// </exception>
        void RegisterMatch(Player winner, Player loser);

        /// <summary>
        /// Checks if a player combination is valid for a match
        /// </summary>
        /// <param name="playerA"></param>
        /// <param name="playerB"></param>
        /// <returns>
        ///  true if difference in player ranks is in the range 1..2, e.g.:  rank 3 - rank 5 works, rank 3 - rank 6 does not
        /// </returns>
        bool IsValidMatch(Player playerA, Player playerB);

        /// <summary>
        /// Must be called when the application ends (performs cleanup actions).
        /// </summary>
        void Close();
    }
}
