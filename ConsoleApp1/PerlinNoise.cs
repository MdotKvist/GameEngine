using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Threading;
using System.Drawing.Imaging;

namespace OpenGLMinecraft
{
    public class PerlinNoise
    {
        private const int GradientSizeTable = 256;
        private readonly Random _random;
        private readonly double[] _gradients = new double[GradientSizeTable * 3];
        private readonly byte[] _perm = new byte[GradientSizeTable * 2];

        public PerlinNoise(int seed)
        {
            _random = new Random(seed);
            InitGradients();
            InitPermutation();
        }

        private void InitGradients()
        {
            for (int i = 0; i < GradientSizeTable; i++)
            {
                double z = 1f - 2f * _random.NextDouble();
                double r = Math.Sqrt(1f - z * z);
                double theta = 2 * Math.PI * _random.NextDouble();
                _gradients[i * 3] = r * Math.Cos(theta);
                _gradients[i * 3 + 1] = r * Math.Sin(theta);
                _gradients[i * 3 + 2] = z;
            }
        }

        private void InitPermutation()
        {
            for (int i = 0; i < GradientSizeTable; i++)
            {
                _perm[i] = (byte)i;
            }

            for (int i = 0; i < GradientSizeTable; i++)
            {
                int source = _random.Next(GradientSizeTable - i);
                byte sourceValue = _perm[source];
                _perm[source] = _perm[GradientSizeTable - i - 1];
                _perm[GradientSizeTable - i - 1] = sourceValue;
            }

            for (int i = 0; i < GradientSizeTable; i++)
            {
                _perm[GradientSizeTable + i] = _perm[i];
            }
        }

        private int Perm(int i)
        {
            return _perm[i & (GradientSizeTable - 1)];
        }

        private int Index(int ix, int iy, int iz)
        {
            return Perm(ix) + Perm(iy + Perm(iz));
        }

        private double Lerp(double t, double value0, double value1)
        {
            return value0 + t * (value1 - value0);
        }

        private double Grad(int hash, double x, double y, double z)
        {
            int h = hash & 15;
            double u = h < 8 ? x : y;
            double v = h < 4 ? y : h == 12 || h == 14 ? x : z;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        public double Noise(double x, double y, double z)
        {
            int ix = (int)Math.Floor(x);
            int iy = (int)Math.Floor(y);
            int iz = (int)Math.Floor(z);
            double fx0 = x - ix;
            double fy0 = y - iy;
            double fz0 = z - iz;
            double fx1 = fx0 - 1;
            double fy1 = fy0 - 1;
            double fz1 = fz0 - 1;
            ix &= GradientSizeTable - 1;
            iy &= GradientSizeTable - 1;
            iz &= GradientSizeTable - 1;
            int ix1 = ix + 1;
            int iy1 = iy + 1;
            int iz1 = iz + 1;

            double value0 = Grad(Index(ix, iy, iz), fx0, fy0, fz0);
            double value1 = Grad(Index(ix1, iy, iz), fx1, fy0, fz0);
            double value2 = Grad(Index(ix, iy1, iz), fx0, fy1, fz0);
            double value3 = Grad(Index(ix1, iy1, iz), fx1, fy1, fz0);
            double value4 = Grad(Index(ix, iy, iz1), fx0, fy0, fz1);
            double value5 = Grad(Index(ix1, iy, iz1), fx1, fy0, fz1);
            double value6 = Grad(Index(ix, iy1, iz1), fx0, fy1, fz1);
            double value7 = Grad(Index(ix1, iy1, iz1), fx1, fy1, fz1);

            double tx = Smooth(fx0);
            double ty = Smooth(fy0);
            double tz = Smooth(fz0);

            double nx0 = Lerp(tx, value0, value1);
            double nx1 = Lerp(tx, value2, value3);
            double nx2 = Lerp(tx, value4, value5);
            double nx3 = Lerp(tx, value6, value7);

            double nxy0 = Lerp(ty, nx0, nx1);
            double nxy1 = Lerp(ty, nx2, nx3);

            return Lerp(tz, nxy0, nxy1);
        }

        private double Smooth(double t)
        {
            return t * t * (3 - 2 * t);
        }
    }
}
