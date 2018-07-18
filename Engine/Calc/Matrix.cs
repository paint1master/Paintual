using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Numerics;

using Accord.Math;

namespace Engine.Calc
{
    public class Matrix
    {

        public static double[,] RotationMatrix0degrees()
        {
            double[,] matrix =
            {
                {1, 0},
                {0, 1}
            };

            return matrix;
        }

        public static double[,] RotationMatrix90degrees()
        {
            double[,] matrix =
            {
                {0, 1},
                {-1, 0}
            };

            return matrix;
        }

        public static double[,] RotationMatrix180degrees()
        {
            double[,] matrix =
            {
                {-1, 0},
                {0, -1}
            };

            return matrix;
        }

        public static double[,] RotationMatrix270degrees()
        {
            double[,] matrix =
            {
                {0, -1},
                {1, 0}
            };

            return matrix;
        }

        public static int NumericsVectorCount()
        {
            // returns 8 on ZeFactory computer
            return System.Numerics.Vector<int>.Count;
        }

        /// <summary>
        /// vector dot matrix
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static System.Windows.Point Dot(System.Windows.Point p, double[,] matrix)
        {
            double[] vector = new double[] { p.X, p.Y };
            double[] r = vector.Dot(matrix);

            System.Windows.Point result = new System.Windows.Point(r[0], r[1]);
            return result;
        }
    }
}
