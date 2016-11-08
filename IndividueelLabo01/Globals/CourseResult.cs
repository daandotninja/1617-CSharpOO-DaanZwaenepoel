using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals
{
    public class CourseResult
    {


        public Grades Grade{
            get
            {
                if(Score >= 8.5)
                {
                    return Grades.Excellent;
                }
                if (Score >= 6.5)
                {
                    return Grades.Good;
                }
                if (Score >= 5)
                {
                    return Grades.Poor;
                }
                else
                {
                    return Grades.Insufficient;
                }
            }
        }
        public string Name{get;}
        public int NrOfParticipants { get; set; }
        public double Score { get; set; }
        public CourseResult(string name ,int score)
        {
            this.Name = name;
            if(score < 0 || score >10)
            {
                throw (new ArgumentOutOfRangeException());
            }
            else
            {
                this.Score = score;
            }
            NrOfParticipants++;
        }
        public void AddScore(int newScore)
        {
            NrOfParticipants++;
            Score = ((this.Score + newScore) / NrOfParticipants);
           
        }
        public int CompareTo(CourseResult other)
        {
            return string.Compare(this.Name, other.Name);
         

        }
        public string ToString() { return this.Name +" - "+ this.Score + "("+this.Grade+") - "+this.NrOfParticipants+" participants";}

        
        
    }
}
