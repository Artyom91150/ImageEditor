using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;

namespace ImageEditor
{
    class LinearGistFilter : Filter
    {

        int MaxIntensity, MinIntensity;
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);

            Color resultColor = Color.FromArgb(Clamp((int)(255.0 * (sourceColor.R - MinIntensity) / (MaxIntensity - MinIntensity)), 0, 255),
                                               Clamp((int)(255.0 * (sourceColor.G - MinIntensity) / (MaxIntensity - MinIntensity)), 0, 255),
                                               Clamp((int)(255.0 * (sourceColor.B - MinIntensity) / (MaxIntensity - MinIntensity)), 0, 255));
            return resultColor;
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);

            int Intencity;
            int[] IntencityArray = new int[256];
            MaxIntensity = 0;
            MinIntensity = 0;

            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    Intencity = (sourceImage.GetPixel(i, j).R + sourceImage.GetPixel(i, j).G + sourceImage.GetPixel(i, j).B) / 3;
                    IntencityArray[Intencity]++;
                }
            }

            for(int i = 0; i < 255; i++)
            {
                int max = 0, min = 0;
                if (IntencityArray[i] > max)
                {
                    max = IntencityArray[i];
                    MaxIntensity = i;
                }
                if (IntencityArray[i] < min)
                {
                    min = IntencityArray[i];
                    MinIntensity = i;
                }
            }



            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                    return null;
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }
            return resultImage;
        }
    }
}
