using Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class DataAccessImplementation: IDataAccess
    {
        public List<CourseResult> SortedCourseList { get; }
        public void AddCourseScore(string courseName, int score)
        {

        }
        public void ImportScores(string fileName)
        {

        }
    }
}
