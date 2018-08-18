/**********************************************************

MIT License

Copyright (c) 2018 Michel Belisle

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

**********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Color
{
    public class ColorVariance
    {
        private Engine.Color.Cell t_referenceColor;

        // if left to 0, the frequency calculation for the channel will be off.
        private byte t_redRange;
        private byte t_greenRange;
        private byte t_blueRange;
        private byte t_alphaRange;

        private int t_redFrequency;
        private int t_greenFrequency;
        private int t_blueFrequency;
        private int t_alphaFrequency;

        /// <summary>
        /// the step value on the draw point "timeline" that is used to calculate
        /// the frequency values at that point.
        /// </summary>
        private int t_step = 0;

        /// <summary>
        /// A parameterless constructor, required by a ITPropertyControl to build this instance
        /// </summary>
        public ColorVariance()
        {
            t_referenceColor = Engine.Colors.Black;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c">The reference color around which all variances will be constructed.</param>
        public ColorVariance(Engine.Color.Cell c)
        {
            t_referenceColor = c;
        }

        public void SetColor(Engine.Color.Cell c)
        {
            t_referenceColor = c;
        }

        public void SetRanges(byte redRange, byte greenRange, byte blueRange, byte alphaRange)
        {
            t_redRange = redRange;
            t_greenRange = greenRange;
            t_blueRange = blueRange;
            t_alphaRange = alphaRange;
        }

        /// <summary>
        /// Sets the same range value for each color channel; except for the
        /// alpha channel which is automatically set to 0.
        /// </summary>
        /// <param name="r"></param>
        public void SetRanges(byte r)
        {
            SetRanges(r, r, r, 0);
        }

        public void SetFrequencies(int redF, int greenF, int blueF, int alphaF)
        {
            t_redFrequency = ValidateFrequencyInput(redF);
            t_greenFrequency = ValidateFrequencyInput(greenF);
            t_blueFrequency = ValidateFrequencyInput(blueF);
            t_alphaFrequency = ValidateFrequencyInput(alphaF);
        }

        /// <summary>
        /// Sets the same frequency value for each color channel; except for the
        /// alpha channel which is automatically set to 0.
        /// </summary>
        /// <param name="f"></param>
        public void SetFrequencies(int f)
        {
            SetFrequencies(f, f, f, 0);
        }

        private byte CalculateValue(byte colorChannel, int colorFreq, int colorRange)
        {
            if (colorRange == 0 || colorFreq == 0)
            {
                return colorChannel;
            }

            // 360d is necessary otherwise the whole calculations gives and int (a rounded value) which
            // causes the frequency cycle to be incomplete
            double angle = (360d / colorFreq) * (t_step % colorFreq);

            int v = (int)(Math.Sin(angle * Engine.Calc.Math.DEG_TO_RAD) * (colorRange / 2));

            v = colorChannel + v;

            if (v < 0)
            {
                return 0;
            }

            if (v > 255)
            {
                return 255;
            }

            return (byte)v;
        }

        public void Step()
        {
            t_step++;
        }

        /// <summary>
        /// Resets the internal step counter to 0;
        /// </summary>
        public void Reset()
        {
            t_step = 0;
        }

        private int ValidateFrequencyInput(int f)
        {
            if (f < 0)
            {
                return 0;
            }

            return f;
        }

        public byte Red { get => CalculateValue(t_referenceColor.Red, t_redFrequency, t_redRange); }
        public byte Green { get => CalculateValue(t_referenceColor.Green, t_greenFrequency, t_greenRange); }
        public byte Blue { get => CalculateValue(t_referenceColor.Blue, t_blueFrequency, t_blueRange); }
        public byte Alpha { get => CalculateValue(t_referenceColor.Alpha, t_alphaFrequency, t_alphaRange); }

        public Engine.Color.Cell ColorVariation { get => new Engine.Color.Cell(Blue, Green, Red, Alpha); }
    }
}
