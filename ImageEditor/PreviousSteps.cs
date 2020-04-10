using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageEditor
{
    public class PreviousSteps
    {
        int ImageCount = 3;
        public Bitmap[] Images;

        public PreviousSteps()
        {
            Images = new Bitmap[ImageCount];
        }

        public void AddImage(Bitmap SourceImage)
        {
            for(int i = ImageCount - 1; i > 0; i--)
            {
                Images[i] = Images[i - 1];
            }
            Images[0] = SourceImage;
        }
    }
}
