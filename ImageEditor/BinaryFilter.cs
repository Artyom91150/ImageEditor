using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageEditor
{
    class BinaryFilter : Filter
    {
        Color WhiteOrBlack(Color col)
        {
            if (col.R + col.G + col.B > 384)
                return Color.White;
            else
                return Color.Black;
        }
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            return WhiteOrBlack(sourceColor);
        }
    }
}
