using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMDcs
{
    class MathOp
    {
        /// <summary>
        /// Find indices and values of nonzero elements
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static double[] diff(double[] data)
        {
            double[] diffout = new double[data.Length - 1];
            for (int i = 1; i < data.Length; ++i)
            {
                diffout[i - 1] = data[i] - data[i - 1];
            }
            return diffout;
        }

        public static int[] diff(int[] data)
        {
            int[] diffout = new int[data.Length - 1];
            for (int i = 1; i < data.Length; ++i)
            {
                diffout[i - 1] = data[i] - data[i - 1];
            }
            return diffout;
        }

        /// <summary>
        /// find array sign, 1: >0; 0: =0; -1: <0
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int[] sign(double[] data)
        {
            int[] signout = new int[data.Length];
            for (int i = 0; i < data.Length; ++i)
            {
                signout[i] = Math.Sign(data[i]);
            }
            return signout;
        }
        public static int[] sign(int[] data)
        {
            int[] signout = new int[data.Length];
            for (int i = 0; i < data.Length; ++i)
            {
                signout[i] = Math.Sign(data[i]);
            }
            return signout;
        }
        /// <summary>
        /// find array elements equal to value, return the index
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<int> find(double[] data, double value = 0)
        {
            List<int> index = new List<int>();
            for (int i = 0; i < data.Length; ++i)
            {
                if (data[i] == value)
                    index.Add(i);
            }
            return index;
        }

        public static List<int> find(int[] data, int value = 0)
        {
            List<int> index = new List<int>();
            for (int i = 0; i < data.Length; ++i)
            {
                if (data[i] == value)
                    index.Add(i);
            }
            return index;
        }
        /// <summary>
        /// find values do not equal to 0
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<int> find(double[] data)
        {
            List<int> index = new List<int>();
            for (int i = 0; i < data.Length; ++i)
            {
                if (data[i] != 0)
                    index.Add(i);
            }
            return index;
        }

        public static List<int> find(int[] data)
        {
            List<int> index = new List<int>();
            for (int i = 0; i < data.Length; ++i)
            {
                if (data[i] != 0)
                    index.Add(i);
            }
            return index;
        }
    }
}
