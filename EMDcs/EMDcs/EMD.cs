// Author: Shengkun Fang
using EMDcs.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace EMDcs
{
    class EMD
    {
        double[] data;

        public double[] Data { get => data; set => data = value; }

        public List<double[]> compute()
        {
            var x = new double[Data.Length];
            Array.Copy(Data, 0, x, 0, Data.Length);
            var imf = new List<double[]>(); ///init imf buffer

            while (!ismonotonic(x))
            {
                var x1 = new double[x.Length];
                Array.Copy(x, 0, x1, 0, Data.Length);

                double sd = double.PositiveInfinity;

                while ((sd == 0) || !isimf(x1))  //check if x1 is an imf
                {
                    var s1 = getspline(x1);  //find the positive peaks, get the spline interpolation the peaks
                    var nx1 = new double[x1.Length]; 
                    for (int i = 0; i < x1.Length; ++i) { nx1[i] = -x1[i]; }
                    var s2 = getspline(nx1); 
                    for (int i = 0; i < s2.Length; ++i) { s2[i] = -s2[i]; }//the trough of the wave

                    double[] x2 = new double[x1.Length];
                    for (int i = 0; i < x2.Length; ++i) { x2[i] = x1[i] - 0.5 * (s1[i] + s2[i]); } //the original wave minus the means of the peaks and troughs
                    
                    double sum = 0;

                    for (int i = 0; i < x1.Length; ++i) { sum += (x1[i] - x2[i]); }

                    sd = sum / x1.Length;
                    ///check the error between the original wave and the interpolation
                    x1 = x2;
                }

                imf.Add(x1);

                for (int i = 0; i < x.Length; ++i) { x[i] = x[i] - x1[i]; }
            }

            return imf;
        }

        private bool ismonotonic(double[] x)
        {
            var pospeak = findpeaks(x);
            var nx = new double[x.Length];
            for (int i = 0; i < x.Length; ++i) { nx[i] = -x[i]; }
            var negpeak = findpeaks(nx);

            var u1 = pospeak.Count * negpeak.Count;

            if (u1 > 0)
                return false;
            else
                return true;
        }

        private List<int> findpeaks(double[] x)
        {
            var diffx = MathOp.diff(x);
            var signx = MathOp.sign(diffx);
            var diffsign = MathOp.diff(signx);
            var n = MathOp.find(diffsign, -2);

            double[] xx = new double[n.Count];
            for (int i = 0; i < n.Count; ++i)
            {
                xx[i] = x[n[i]];
            }

            var u = MathOp.find(xx);

            for (int i = 0; i < u.Count; ++i)
            {
                n[u[i]] = n[u[i]] + 1;
            }

            return n;
        }

        private bool isimf(double[] x)
        {
            var N = x.Length;
            var t = dotproduct(x, 0, N - 1, x, 1, N - 1);
            int u1 = 0;
            for (int i = 0; i < t.Length; ++i)
            {
                if (t[i] < 0)
                    u1++;
            }

            var l1 = findpeaks(x);
            var nx = new double[x.Length];
            for (int i = 0; i < x.Length; ++i) { nx[i] = -x[i]; }
            var l2 = findpeaks(nx);
            var u2 = l1.Count + l2.Count;

            if (Math.Abs(u1 - u2) <= 1)
                return true;
            else
                return false;
        }

        private double[] dotproduct(double[] x, int ix, int xlength, double[] y, int iy, int ylength)
        {
            if (xlength != ylength)
                return null;

            double[] output = new double[xlength];

            for (int i = ix, j = iy; i < (ix + xlength) && j < (iy + ylength); ++i, ++j)
            {
                output[i] = x[i] * y[j];
            }

            return output;
        }

        private double[] getspline(double[] x)  ///the cubic spline interpolation of peaks from x
        {
            var N = x.Length;
            var p = findpeaks(x);

            double[] xp = new double[p.Count];
            double[] yp = new double[p.Count];
            double[] ys = new double[N];

            for (int i = 0; i < p.Count; ++i)
            {
                xp[i] = p[i];
                yp[i] = x[p[i]];
            }

            Extreme.Mathematics.Curves.CubicSpline spline = new Extreme.Mathematics.Curves.CubicSpline(xp, yp);

            for (int i = 0; i < N; ++i)
            {
                ys[i] = spline.ValueAt(i);
            }
            return ys;
        }
        

        [STAThread]
        static void Main()
        {
            DataVisuForm form = new DataVisuForm();

            List<Chart> chs = new List<Chart>();

            TestEMD(ref chs);

            form.setChart(chs);

            form.ShowDialog();


        }

        private static void TestEMD(ref List<Chart> ch)  ///test code
        {
            double fs = 1000;
            double ts = 1 / fs;

            int N = 301;
            double[] t = new double[N];
            double[] z = new double[N];
            Random random = new Random();
            for (int i = 0; i < N; ++i)
            {
                t[i] = i * ts;
                ///simulate a signal
                z[i] = Math.Sin(2 * Math.PI * 10 * t[i]) + Math.Sin(2 * Math.PI * 100 * t[i]) + Math.Cos(2 * Math.PI * 20 * t[i]); // +random.NextDouble();// 
            }

            EMD emd = new EMD();
            emd.Data = z;
            var output = emd.compute();

            PlotEMD(ref ch, "EMD test", t, z, output);
        }

        private static void PlotEMD(ref List<Chart> chs, string title, double[] x, double[] z, List<double[]> emd)
        {
            chs = new List<Chart>();

            var chart = new Chart();
            chart.Size = new Size(700, 180);
            chart.Titles.Add(title);
            chart.Legends.Add(new Legend("Legend"));

            ChartArea ca = new ChartArea("DefaultChartArea");
            ca.AxisX.Title = "X";
            ca.AxisY.Title = "Y";
            chart.ChartAreas.Add(ca);

            // Series s1 = CreateSeries(chart, "Spline", CreateDataPoints(xs, ys), Color.Blue, MarkerStyle.None);
            Series s = CreateSeries(chart, "Original", CreateDataPoints(x, z), Color.Green, MarkerStyle.None);

            chart.Series.Add(s);
            chs.Add(chart);
            //chart.Series.Add(s1);
            int i = 1;
            foreach (var imf in emd)
            {
                var chart1 = new Chart();
                chart1.Size = new Size(700, 180);
                chart1.Titles.Add(title);
                String text = "imf" + i.ToString();
                if (i == emd.Count)
                    text = "res";
                chart1.Legends.Add(new Legend(text));

                ChartArea ca1 = new ChartArea("ChartArea");
                ca1.AxisX.Title = "X";
                ca1.AxisY.Title = "Y";
                chart1.ChartAreas.Add(ca1);

                Series s1 = CreateSeries(chart1, text, CreateDataPoints(x, imf), Color.Blue, MarkerStyle.None);

                chart1.Series.Add(s1);

                chs.Add(chart1);
                ++i;
            }

        }

        private static Series CreateSeries(Chart chart, string seriesName, IEnumerable<DataPoint> points, Color color, MarkerStyle markerStyle = MarkerStyle.None)
        {
            var s = new Series()
            {
                XValueType = ChartValueType.Double,
                YValueType = ChartValueType.Double,
                Legend = chart.Legends[0].Name,
                IsVisibleInLegend = true,
                ChartType = SeriesChartType.Line,
                Name = seriesName,
                ChartArea = chart.ChartAreas[0].Name,
                MarkerStyle = markerStyle,
                Color = color,
                MarkerSize = 8
            };

            foreach (var p in points)
            {
                s.Points.Add(p);
            }

            return s;
        }

        private static List<DataPoint> CreateDataPoints(double[] x, double[] y)
        {
            Debug.Assert(x.Length == y.Length);
            List<DataPoint> points = new List<DataPoint>();

            for (int i = 0; i < x.Length; i++)
            {
                points.Add(new DataPoint(x[i], y[i]));
            }

            return points;
        }
    }
}
