using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals
{
    [Serializable]
    public struct Player
    {
        public string Name { get; }
        public int Age { get; }

        public Player(string name, int age)
        {
            Name = name;
            Age = age;
        }

        override public string ToString()
        {
            return $"{Name} ({Age})";
        }
        public static bool operator ==(Player a, Player b)
        {
            return ((a.Age == b.Age) && (a.Name == b.Name));
        }
        public static bool operator !=(Player a, Player b)
        {
            return !(a == b);
        }

    }
}
