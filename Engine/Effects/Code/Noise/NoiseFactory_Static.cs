using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Surface;
using Engine.Effects.Noise;

namespace Engine.Effects.Code.Noise
{
    public static class NoiseFactory_Static
    {
        private static string paramName_module = "module";
        private static string paramName_canvas = "canvas";

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
    }
}
