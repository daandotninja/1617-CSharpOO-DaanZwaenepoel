using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Globals
{
    
    public interface IFilter
    {
        
        Bitmap OriginalImage { get; set; }
        Bitmap FilteredImage { get; }

        void Load(string file);
        void ApplyFilter(Filter filterMode);


    }
}
