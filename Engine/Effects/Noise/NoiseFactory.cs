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

        private const string paramName_module = "module";
        private const string paramName_canvas = "canvas";

        public NoiseFactory()
        {
            t_visualProperties = new VisualProperties(Name, typeof(NoiseFactory));

            // these are default values, may be overwritten by UI
            t_seed = 0;
            t_octaves = 6;
            t_frequency = 0.05;
            t_lacunarity = 2.0;
            t_persistence = 0.5;
        }

        public override IGraphicActivity Duplicate(Engine.Workflow w)
        {
            NoiseFactory nf = new NoiseFactory();
            nf.Initialize(w);

            return nf;
        }

        public override void Process()
        {
            t_workflow.Viome.ThreadingQueue.RunAndForget(new Action(ThreadedProcess));
        }

        private void ThreadedProcess()
        {
            if (t_noiseType == NoiseTypes.Undefined)
            {
                t_noiseType = NoiseTypes.FastNoise;
            }

            Engine.Effects.Noise.IModule module;

            // switch block : source and info at : https://libnoisedotnet.codeplex.com/downloads/get/720936
            // and http://libnoise.sourceforge.net/tutorials/tutorial8.html

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

            t_workflow.Viome.AllowInvalidate();

            Engine.Threading.ThreadedLoop loop = new Threading.ThreadedLoop();

            Engine.Threading.ParamList paramList = new Threading.ParamList();
            paramList.Add(paramName_module, typeof(Engine.Effects.Noise.IModule), module);

            loop.Loop(t_imageSource.Height, Threaded_Process, paramList);
            loop.Dispose();

            base.ProcessCompleted();
        }

        private int Threaded_Process(int start, int end, Engine.Threading.ParamList paramList)
        {
            Engine.Effects.Noise.IModule module = (Engine.Effects.Noise.IModule)paramList.Get(paramName_module).Value;

            double value = 0;

            // loop block : source and info at : https://libnoisedotnet.codeplex.com/downloads/get/720936
            // and http://libnoise.sourceforge.net/tutorials/tutorial8.html

            for (int y = start; y < end; y++)
            {
                for (int x = 0; x < t_imageProcessed.Width - 1; x++)
                {
                    value = (module.GetValue(x, y, 10) + 1) / 2.0;

                    if (value < 0) value = 0;
                    if (value > 1.0) value = 1.0;
                    byte intensity = (byte)(value * 255.0);
                    Engine.Color.Cell c = Engine.Color.Cell.ShadeOfGray(intensity);

                    t_imageProcessed.SetPixel(c, x, y, PixelSetOptions.Ignore);
                }
            }

            return 0;
        }

        public static Engine.Surface.Canvas CreatePerlinNoisePlane(Engine.Surface.Canvas source, double frequency, int seed, int octaves)
        {
            Engine.Effects.Noise.IModule module = new Engine.Effects.Noise.Perlin();

            ((Perlin)module).Frequency = frequency;
            ((Perlin)module).NoiseQuality = NoiseQuality.Standard;
            ((Perlin)module).Seed = seed;
            ((Perlin)module).OctaveCount = octaves;
            ((Perlin)module).Lacunarity = 2.0;
            ((Perlin)module).Persistence = 0.5;

            Engine.Surface.Canvas perlinSurface = new Canvas(source.Width, source.Height);

            Engine.Threading.ThreadedLoop loop = new Threading.ThreadedLoop();

            Engine.Threading.ParamList paramList = new Threading.ParamList();
            paramList.Add(paramName_module, typeof(Engine.Effects.Noise.IModule), module);
            paramList.Add(paramName_canvas, typeof(Engine.Surface.Canvas), perlinSurface);

            loop.Loop(source.Height, Threaded_CreatePerlinNoisePlane, paramList);
            loop.Dispose();

            return perlinSurface;
        }

        private static int Threaded_CreatePerlinNoisePlane(int start, int end, Engine.Threading.ParamList paramList)
        {
            Engine.Effects.Noise.IModule module = (Engine.Effects.Noise.IModule)paramList.Get(paramName_module).Value;
            Engine.Surface.Canvas canvas = (Engine.Surface.Canvas)paramList.Get(paramName_canvas).Value;

            double value = 0;

            // loop block : source and info at : https://libnoisedotnet.codeplex.com/downloads/get/720936
            // and http://libnoise.sourceforge.net/tutorials/tutorial8.html

            for (int y = start; y < end; y++)
            {
                for (int x = 0; x < canvas.Width - 1; x++)
                {
                    value = (module.GetValue(x, y, 10) + 1) / 2.0;

                    if (value < 0) value = 0;
                    if (value > 1.0) value = 1.0;
                    byte intensity = (byte)(value * 255.0);
                    Engine.Color.Cell c = Engine.Color.Cell.ShadeOfGray(intensity);
                    canvas.SetPixel(c, x, y, PixelSetOptions.Ignore);
                }
            }

            return 0;
        }

        public override string Name { get => "Noise Factory"; }

        [Engine.Attributes.Meta.DisplayName("Seed")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.DefaultValue(245)]
        public int Seed
        {
            get { return t_seed; }
            set { t_seed = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Octaves")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.Range(1, 30)]
        [Engine.Attributes.Meta.DefaultValue(2)]
        public int Octaves
        {
            get { return t_octaves; }
            set { t_octaves = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Frequency")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Double)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Double, "")]
        [Engine.Attributes.Meta.DefaultValue(0.05d)]
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
