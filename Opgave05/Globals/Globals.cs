using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Globals
{
    public enum Filter { Original, GreyScale, Red, Green, Blue }

    public delegate Color FilterOperation(Color pixel);
}
