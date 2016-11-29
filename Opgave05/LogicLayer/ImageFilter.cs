using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Globals;

namespace LogicLayer
{
    public class ImageFilter : IFilter
    {
        public Bitmap OriginalImage { set; get; }
        // auto-implemented. Do not change declaration

        public Bitmap FilteredImage { get; }
        // return the private field containing the filtered image

        public void Load(string file)
        {
            // Load the requested picture into the 'OriginalImage' field 
            // and copy it to a private field that will contain the filtered bitmap

        }

        public void ApplyFilter(Filter filterMode)
        {
            // call ExecuteFilter with a suitable 'filterOperation' parameter 
            // based on  the value of the 'filterMode' parameter.
            // declare the 'filterOperation' parameters as lambda expressions
        }

        public void ExecuteFilter(FilterOperation operation)
        {
            // calculate the new image by looping through all the pixels 
            // in the (original) image & apply the passed operation to them  
        }




    }
}
