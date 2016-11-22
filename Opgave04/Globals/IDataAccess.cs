using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals
{
    public interface IDataAccess
    {
        /// <summary>
        /// Persists the player ranking to the binary file "resources\Ranking.dat"
        /// </summary>
        /// <param name="players">
        /// the ranking as an array of 'Player' objects, index 0 contains the top ranked player
        /// </param>
        void SaveRanking(Player[] players);

        /// <summary>
        /// Reads the player ranking from the binary file "resources\Ranking.dat"
        /// </summary>
        /// </summary>
        /// <returns>
        /// the ranking as an array of 'Player' objects, index 0 contains the top ranked player
        /// </returns>
        Player[] ReadRanking();

        /// <summary>
        /// Appends loginformation for a match to the text file "resources\Ranking.log"
        /// </summary>
        /// <remarks>
        ///  The log file should contain one line per match in the format <date & time> - <PlayerA> (wins) - <Player B>
        /// </remarks>
        /// <param name="match"></param>
        void LogMatch(Match match);
    }
}
