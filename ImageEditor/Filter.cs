using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;

namespace ImageEditor
{
    //Базовый класс фильтров
    public abstract class Filter
    {
        public int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        protected abstract Color calculateNewPixelColor(Bitmap sourceImage, int x, int y);

        public virtual Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for(int i = 0; i < sourceImage.Width; i ++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                    return null;
                for(int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(sourceImage, i, j));
                }
            }
            return resultImage;
        }
    }

    //Увеличение яркости
    class BrightnessFilter : Filter
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            int Brightness = 10;
            Color resultColor = Color.FromArgb(Clamp(sourceColor.R + Brightness, 0, 255), Clamp(sourceColor.G + Brightness, 0, 255), Clamp(sourceColor.B + Brightness, 0, 255));
            return resultColor;
        }
    }

    //Эффект стекла
    class GlassFilter : Filter
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            var rand = new Random();
            int ResultX = (int)(x + rand.Next(-5, 5));
            int ResultY = (int)(y + rand.Next(-5, 5));
            return sourceImage.GetPixel(Clamp(ResultX, 0, sourceImage.Width - 1), Clamp(ResultY, 0, sourceImage.Height - 1));

        }
    }

    //Фильтр серого
    class GrayScaleFilter : Filter
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            int intensity = (int)(0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.114 * sourceColor.B);
            Color resultColor = Color.FromArgb(intensity, intensity, intensity);
            return resultColor;
        }
    }

    //Инверсия
    class InvertFilter : Filter
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            Color resultColor = Color.FromArgb(255 - sourceColor.R, 255 - sourceColor.G, 255 - sourceColor.B);
            return resultColor;
        }
    }

    //Эффект движения
    class MoveFilter : Filter
    {
        int ShiftingX;
        int ShiftingY;
        double Rotation;

        public MoveFilter(int _ShiftingX, int _ShiftingY, double _Rotation)
        {
            ShiftingX = _ShiftingX;
            ShiftingY = _ShiftingY;
            Rotation = _Rotation;
        }
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            int ResultX = (int)((x + ShiftingX - sourceImage.Width / 2) * Math.Cos(Rotation) - (y + ShiftingY - sourceImage.Height / 2) * Math.Sin(Rotation) + sourceImage.Width / 2);
            int ResultY = (int)((x + ShiftingX - sourceImage.Width / 2) * Math.Sin(Rotation) + (y + ShiftingY - sourceImage.Height / 2) * Math.Cos(Rotation) + sourceImage.Height / 2);
            return sourceImage.GetPixel(Clamp(ResultX, 0, sourceImage.Width - 1), Clamp(ResultY, 0, sourceImage.Height - 1));

        }
    }

    //Фильтр сепия
    class SepiaFilter : Filter
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            int k = 30;
            int intensity = (int)(0.299 * sourceColor.R + 0.587 * sourceColor.G + 0.114 * sourceColor.B);
            Color resultColor = Color.FromArgb(Clamp(intensity + 2 * k, 0, 255), Clamp((int)(intensity + 0.5 * k), 0, 255), Clamp(intensity - k, 0, 255));
            return resultColor;
        }
    }


    //Эффект волн
    class WavesFilter : Filter
    {
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            //int ResultX = (int)(x + 20 * Math.Sin((2 * Math.PI * y)/60));
            //int ResultY = y;
            int ResultX = (int)(x + 20 * Math.Sin((2 * Math.PI * x) / 30));
            int ResultY = y;
            return sourceImage.GetPixel(Clamp(ResultX, 0, sourceImage.Width - 1), Clamp(ResultY, 0, sourceImage.Height - 1));

        }
    }

    //Фильтр Серый мир
    class GrayWorldFilter : Filter
    {
        int AvgR, AvgG, AvgB, Avg;
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            return(Color.FromArgb(Clamp(sourceColor.R * Avg / AvgR, 0, 255), Clamp(sourceColor.G * Avg / AvgG, 0, 255), Clamp(sourceColor.B * Avg / AvgB, 0, 255)));
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            for (int i = 0; i < sourceImage.Width; i++)
            {
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    AvgR += sourceImage.GetPixel(i, j).R;
                    AvgG += sourceImage.GetPixel(i, j).G;
                    AvgB += sourceImage.GetPixel(i, j).B;
                }
            }

            AvgR = AvgR / (sourceImage.Height * sourceImage.Width);
            AvgG = AvgG / (sourceImage.Height * sourceImage.Width);
            AvgB = AvgB / (sourceImage.Height * sourceImage.Width);

            Avg = (AvgR + AvgG + AvgB) / 3;

            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
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


