/*
    TC Plyer
    Total Commander Audio Player plugin & standalone player written in C#, based on bass.dll components
    Copyright (C) 2016 Webmaster442 aka. Ruzsinszki Gábor

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TCPlayer.Controls
{
    public class CircleVisual : Shape
    {
        private const int CornerPoints = 360;

        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.Register(
                "RotationAngle",
                typeof(double),
                typeof(CircleVisual),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, null,
                                              OnCoerceRotationAngle));

        public double RotationAngle
        {
            get { return (double)GetValue(RotationAngleProperty); }
            set { SetValue(RotationAngleProperty, value); }
        }


        private static object OnCoerceRotationAngle(DependencyObject obj, object baseValue)
        {
            CircleVisual shape = (CircleVisual)obj;
            double value = (double)baseValue;

            if (value < 0)
                value = 0;

            if (value > 360)
                value = 360;

            return value;
        }

        public static readonly DependencyProperty InnerRadiusOffsetProperty =
            DependencyProperty.Register(
                "InnerRadiusOffset",
                typeof(int),
                typeof(CircleVisual),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender, null,
                                              OnCoerceInnerRadiusOffset));

        public int InnerRadiusOffset
        {
            get { return (int)GetValue(InnerRadiusOffsetProperty); }
            set { SetValue(InnerRadiusOffsetProperty, value); }
        }


        //Restrict OffSetValue between 0 and 200 %;

        private static object OnCoerceInnerRadiusOffset(DependencyObject obj, object baseValue)
        {
            CircleVisual shape = (CircleVisual)obj;
            int value = (int)baseValue;

            if (value < 0)
                value = 0;
            if (value > 200)
                value = 200;

            return value;
        }

        public short[] WaveData
        {
            get { return (short[])GetValue(WaveDataProperty); }
            set { SetValue(WaveDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaveData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaveDataProperty =
            DependencyProperty.Register(
                "WaveData",
                typeof(short[]),
                typeof(CircleVisual),
                new FrameworkPropertyMetadata(new short[1], FrameworkPropertyMetadataOptions.AffectsRender));

        protected override Geometry DefiningGeometry
        {
            get { return CreateGeometry(); }
        }

        private static double GetSin(double degAngle)
        {
            return Math.Sin(Math.PI * degAngle / 180);
        }

        private static double GetCos(double degAngle)
        {
            return Math.Cos(Math.PI * degAngle / 180);
        }

        private void Swap(ref double val1, ref double val2)
        {
            double temp = val1;
            val1 = val2;
            val2 = temp;
        }

        private static double Map(short sample, double out_min, double out_max)
        {
            return (sample - short.MinValue) * (out_max - out_min) / (short.MaxValue - short.MinValue) + out_min;
        }

        private StreamGeometry CreateGeometry()
        {
            // Twice as much points for the calculation of intermediate point between cornerpoints

            int cornerPoints = CornerPoints * 2;

            // Incrementing angle based on amount of cornerpoints 
            double incrementingAngle = 360d / cornerPoints;


            //Outer radius based on the minium widht or height of the shape

            double outerRadius = Math.Min(RenderSize.Width / 2 - StrokeThickness / 2,
                                         RenderSize.Height / 2 - StrokeThickness / 2);


            //innerRadius calculation taking innerRadiusOffset as a percentage offset into account 

            double innerRadiusOffset = (1 - InnerRadiusOffset / 100f);
            double innerRadius = GetCos(incrementingAngle) * outerRadius * innerRadiusOffset;

            //Calculate and store points for geometry

            double x, y, angle, mod;
            Point[] points = new Point[cornerPoints];

            int j = 0;
            int modifyStep = WaveData.Length / cornerPoints;

            for (int i = 0; i < cornerPoints; i++)
            {
                //Alternating point on outer and inner radius
                j = i * modifyStep;
                angle = i * incrementingAngle + RotationAngle;
                mod = Map(WaveData[j], 0.5d, 1.0d);
                x = GetCos(angle) * mod * outerRadius;
                y = GetSin(angle) * mod * outerRadius;

                points[i] = new Point(x, y);

                Swap(ref outerRadius, ref innerRadius);
            }

            //Create the geometry 

            StreamGeometry geometry = new StreamGeometry();

            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(points[0], true, true);

                for (int i = 1; i < cornerPoints; i++)
                {
                    ctx.LineTo(points[i], true, true);
                }
            }


            //Translate into shape center

            geometry.Transform = new TranslateTransform(outerRadius + StrokeThickness / 2,
                                                        outerRadius + StrokeThickness / 2);

            return geometry;
        }
    }
}
