using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;

namespace ImageEditor
{
    class Dilation : MatrixFilter
    {

        public Dilation()
        {
            kernel = new float[3, 3];
            kernel[0, 0] = 0.0f; kernel[0, 1] = 1.0f; kernel[0, 2] = 0.0f;
            kernel[1, 0] = 1.0f; kernel[1, 1] = 1.0f; kernel[1, 2] = 1.0f;
            kernel[2, 0] = 0.0f; kernel[2, 1] = 1.0f; kernel[2, 2] = 0.0f;
        }

        public Dilation(float[,] kernel)
        {
            this.kernel = kernel;
        }

        protected override System.Drawing.Color calculateNewPixelColor(System.Drawing.Bitmap sourceImage, int x, int y)
        {
            // определяем радиус действия фильтра по оси X
            int radiusX = kernel.GetLength(0) / 2;

            // определяем радиус действия фильтра по оси Y
            int radiusY = kernel.GetLength(1) / 2;

            Color resultColor = Color.Black;

            byte max = 0;
            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color color = sourceImage.GetPixel(idX, idY);
                    int intensity = color.R;
                    if (color.R != color.G || color.R != color.B || color.G != color.B)
                    {
                        intensity = (int)(0.36 * color.R + 0.53 * color.G + 0.11 * color.R);
                    }
                    if (kernel[k + radiusX, l + radiusY] > 0 && intensity > max)
                    {
                        max = (byte)intensity;
                        resultColor = color;
                    }
                }
            return resultColor;
        }
    }

    class Erosion : MatrixFilter
    {
        public Erosion()
        {
            kernel = new float[3, 3];
            kernel[0, 0] = 0.0f; kernel[0, 1] = 1.0f; kernel[0, 2] = 0.0f;
            kernel[1, 0] = 1.0f; kernel[1, 1] = 1.0f; kernel[1, 2] = 1.0f;
            kernel[2, 0] = 0.0f; kernel[2, 1] = 1.0f; kernel[2, 2] = 0.0f;
        }

        public Erosion(float[,] kernel)
        {
            this.kernel = kernel;
        }

        protected override System.Drawing.Color calculateNewPixelColor(System.Drawing.Bitmap sourceImage, int x, int y)
        {
            // определяем радиус действия фильтра по оси X
            int radiusX = kernel.GetLength(0) / 2;

            // определяем радиус действия фильтра по оси Y
            int radiusY = kernel.GetLength(1) / 2;

            Color resultColor = Color.White;

            byte min = 255;
            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color color = sourceImage.GetPixel(idX, idY);
                    int intensity = color.R;
                    if (color.R != color.G || color.R != color.B || color.G != color.B)
                    {
                        intensity = (int)(0.36 * color.R + 0.53 * color.G + 0.11 * color.R);
                    }
                    if (kernel[k + radiusX, l + radiusY] > 0 && intensity < min)
                    {
                        min = (byte)intensity;
                        resultColor = color;
                    }
                }
            return resultColor;
        }
    }

    class Opening : MatrixFilter
    {
        public Opening()
        {
            this.kernel = null;
        }

        public Opening(float[,] kernel)
        {
            this.kernel = kernel;
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Dilation dilation;
            Erosion erosion;
            if (kernel != null)
            {
                dilation = new Dilation(this.kernel);
                erosion = new Erosion(this.kernel);
            }
            else
            {
                dilation = new Dilation();
                erosion = new Erosion();
            }
            return dilation.processImage(erosion.processImage(sourceImage, worker), worker);
        }
    }

    class Closing : MatrixFilter
    {
        public Closing()
        {
            this.kernel = null;
        }

        public Closing(float[,] kernel)
        {
            this.kernel = kernel;
        }

        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Dilation dilation;
            Erosion erosion;
            if (kernel != null)
            {
                dilation = new Dilation(this.kernel);
                erosion = new Erosion(this.kernel);
            }
            else
            {
                dilation = new Dilation();
                erosion = new Erosion();
            }
            return erosion.processImage(dilation.processImage(sourceImage, worker), worker);
        }
    }

    class Grad : MatrixFilter
    {
        public override Bitmap processImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Dilation dilation = new Dilation();
            Erosion erosion = new Erosion();
            Subtraction subtraction = new Subtraction(dilation.processImage(sourceImage, worker));
            return subtraction.processImage(erosion.processImage(sourceImage, worker), worker);
        }

    }

    class Subtraction : Filter
    {
        Bitmap FirstImage;
        public Subtraction(Bitmap _FirstImage)
        {
            FirstImage = _FirstImage;
        }
        protected override Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color FirstColor = FirstImage.GetPixel(x, y);
            Color SecondColor = sourceImage.GetPixel(x, y);
            return Color.FromArgb(Clamp(FirstColor.R - SecondColor.R, 0, 255),
                                  Clamp(FirstColor.G - SecondColor.G, 0, 255),
                                  Clamp(FirstColor.B - SecondColor.B, 0, 255));
        }
    }


}
