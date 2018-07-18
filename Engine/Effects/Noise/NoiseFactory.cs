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
using Engine.Surface;
using Engine.Tools;

namespace Engine.Effects.Noise
{
    public enum NoiseTypes
    {
        Undefined = 0,
        Billow = 1,
        FastBillow = 2,
        FastNoise = 3,
        Perlin = 4
    }

    /// <summary>
    /// The entry point of the Engine.Effects.Noise module
    /// </summary>
    public class NoiseFactory : EffectBase
    {
        // TODO : indicate origin of code 
        private NoiseTypes t_noiseType = NoiseTypes.Undefined;

        private int t_seed;
        private int t_octaves;
        private double t_frequency;
        private double t_lacunarity;
        private double t_persistence;

        public NoiseFactory()
        {
            t_visualProperties = new VisualProperties("Noise Factory", typeof(NoiseFactory));

            // these are default values, may be overwritten by UI
            t_seed = 0;
            t_octaves = 6;
            t_frequency = 0.05;
            t_lacunarity = 2.0;
            t_persistence = 0.5;
        }

        public override IGraphicActivity Duplicate(Viome w)
        {
            NoiseFactory nf = new NoiseFactory();
            nf.Initialize(w);

            return nf;
        }

        public override void Process()
        {
            if (t_noiseType == NoiseTypes.Undefined)
            {
                t_noiseType = NoiseTypes.FastNoise;
            }

            Engine.Effects.Noise.IModule module;

            // TODO : indicate source of this portion of code

            switch (t_noiseType)
            {
                case NoiseTypes.Billow:
                    module = new Engine.Effects.Noise.Billow();

                    ((Billow)module).Frequency = t_frequency;
                    ((Billow)module).NoiseQuality = NoiseQuality.Standard;
                    ((Billow)module).Seed = t_seed;
                    ((Billow)module).OctaveCount = t_octaves;
                    ((Billow)module).Lacunarity = t_lacunarity;
                    ((Billow)module).Persistence = t_persistence;
                    break;

                case NoiseTypes.FastBillow:
                    module = new Engine.Effects.Noise.FastBillow();

                    ((FastBillow)module).Frequency = t_frequency;
                    ((FastBillow)module).NoiseQuality = NoiseQuality.Standard;
                    ((FastBillow)module).Seed = t_seed;
                    ((FastBillow)module).OctaveCount = t_octaves;
                    ((FastBillow)module).Lacunarity = t_lacunarity;
                    ((FastBillow)module).Persistence = t_persistence;
                    break;

                case NoiseTypes.FastNoise:
                    module = new Engine.Effects.Noise.FastNoise();

                    ((FastNoise)module).Frequency = t_frequency;
                    ((FastNoise)module).NoiseQuality = NoiseQuality.Standard;
                    ((FastNoise)module).Seed = t_seed;
                    ((FastNoise)module).OctaveCount = t_octaves;
                    ((FastNoise)module).Lacunarity = t_lacunarity;
                    ((FastNoise)module).Persistence = t_persistence;
                    break;

                default:
                    module = new Engine.Effects.Noise.Perlin();

                    ((Perlin)module).Frequency = t_frequency;
                    ((Perlin)module).NoiseQuality = NoiseQuality.Standard;
                    ((Perlin)module).Seed = t_seed;
                    ((Perlin)module).OctaveCount = t_octaves;
                    ((Perlin)module).Lacunarity = t_lacunarity;
                    ((Perlin)module).Persistence = t_persistence;
                    break;
            }
            
            t_imageProcessed = new Engine.Surface.Canvas(t_imageSource.Width, t_imageSource.Height);
            
            int height = t_imageProcessed.Height;

            int divide = Engine.Calc.Math.Division_Threading(height);
            
            Del_Process[] dels = new Del_Process[divide];
            IAsyncResult[] cookies = new IAsyncResult[divide];
            
            int quarterHeight = height / divide;

            int[] ys = new int[divide];
            int[] heights = new int[divide];

            for (int i = 0; i < divide; i++)
            {
                dels[i] = Thread_Process;
                ys[i] = (quarterHeight * i);
                heights[i] = quarterHeight * (i + 1);
            }

            for (int i = 0; i < divide; i++)
            {
                cookies[i] = dels[i].BeginInvoke(t_imageProcessed, module, ys[i], heights[i], null, null);
            }

            for (int i = 0; i < divide; i++)
            {
                dels[i].EndInvoke(cookies[i]);
            }

            base.Process();
        }

        private delegate void Del_Process(Engine.Surface.Canvas image, Engine.Effects.Noise.IModule module, int yStart, int yEnd);

        private static void Thread_Process(Engine.Surface.Canvas image, Engine.Effects.Noise.IModule module, int yStart, int yEnd)
        {
            double value = 0;

            for (int y = yStart; y < yEnd; y++)
            {
                for (int x = 0; x < image.Width - 1; x++)
                {
                    value = (module.GetValue(x, y, 10) + 1) / 2.0;

                    if (value < 0) value = 0;
                    if (value > 1.0) value = 1.0;
                    byte intensity = (byte)(value * 255.0);
                    Engine.Color.Cell c = Engine.Color.Cell.ShadeOfGray(intensity);
                    Engine.Surface.Ops.SetPixel(c, image, x, y);
                }
            }
        }

        [Engine.Attributes.Meta.DisplayName("Seed")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Int, "")]
        public int Seed
        {
            get { return t_seed; }
            set { t_seed = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Octaves")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Int, "")]
        public int Octaves
        {
            get { return t_octaves; }
            set { t_octaves = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Frequency")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Double)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Double, "")]
        public double Frequency
        {
            get { return t_frequency; }
            set { t_frequency = value; }
        }


        [Engine.Attributes.Meta.DisplayName("Noise Type")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.RadioButtons)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Enum, typeof(Engine.Effects.Noise.NoiseTypes))]
        [Engine.Attributes.Meta.ValueList(typeof(NoiseFactoryValueList))]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.ValueList, "")]
        [Engine.Attributes.Meta.DefaultValue(Engine.Effects.Noise.NoiseTypes.FastNoise)]
        public Engine.Effects.Noise.NoiseTypes NoiseType
        {
            get { return t_noiseType; }
            set { t_noiseType = value; }

        }
    }

    public class NoiseFactoryValueList : Engine.Attributes.ValueList
    {

        public NoiseFactoryValueList() : base()
        {
            valueDict.Add("Undefined", NoiseTypes.Undefined);
            valueDict.Add("Billow", NoiseTypes.Billow);
            valueDict.Add("Fast Billow", NoiseTypes.FastBillow);
            valueDict.Add("Fast Noise", NoiseTypes.FastNoise);
            valueDict.Add("Perlin", NoiseTypes.Perlin);

            hiddenValue = "Undefined";
        }

    }
}
