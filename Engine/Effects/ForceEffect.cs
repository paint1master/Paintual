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
using Engine.Tools;


namespace Engine.Effects
{
    public class ForceEffect : EffectBase
    {
        public ForceEffect()
        {
            t_visualProperties = new VisualProperties(Name, typeof(ForceEffect));
        }

        public override IGraphicActivity Duplicate(Engine.Workflow w)
        {
            ForceEffect fe = new ForceEffect();
            fe.Initialize(w);

            return fe;
        }

        public override void Process()
        {
            t_imageProcessed = Engine.Surface.Ops.Copy(t_imageSource);
            t_workflow.Viome.ThreadingQueue.RunAndForget(new Action(ThreadedProcess));

        }

        private void ThreadedProcess()
        {
            t_workflow.Viome.AllowInvalidate();

            Engine.Effects.Particles.Attractor attr1 = new Particles.Attractor(new Accord.Math.Vector3(400, 600, 0));
            Engine.Effects.Particles.Attractor attr2 = new Particles.Attractor(new Accord.Math.Vector3(800, 600, 0));

            Engine.Effects.Particles.Attractor[] attrs = new Particles.Attractor[2];
            attrs[0] = attr1;
            attrs[1] = attr2;

            Engine.Effects.Particles.ForceParticle[] fp = new Particles.ForceParticle[100];

            for (int i = 0; i < fp.Length; i++)
            {
                double variance = Engine.Calc.Math.Rand.NextDouble() + 1;

                fp[i] = new Particles.ForceParticle(600, 300, 10f + (float)variance, -50f +(float)variance);
            }

            // move the particles simulating animation
            for (int i = 0; i < 5000; i++)
            {
                for (int j = 0; j < fp.Length; j++)
                {
                    fp[j].Update(attrs);

                    Engine.Color.Cell c = Engine.Application.UISelectedValues.SelectedColor;
                    // draw a line between last position and current position
                    List<MousePoint> points = LinearInterpolate(fp[j].PreviousPoint, new Point((int)Math.Round(fp[j].Position.X), (int)Math.Round(fp[j].Position.Y)));

                    foreach (MousePoint p in points)
                    {
                        if(t_imageProcessed.IsOutOfBounds(p.X, p.Y))
                        {
                            continue;
                        }
                        Engine.Color.Cell img = t_imageProcessed.GetPixel(p.X, p.Y, Surface.PixelRetrievalOptions.ReturnEdgePixel);

                        t_imageProcessed.SetPixel(Engine.Calc.Color.FastAlphaBlend(c, img), p.X, p.Y, Surface.PixelSetOptions.Ignore);                       
                    }
                }
            }

            base.ProcessCompleted();
        }

        // see Tools.MotionAttribute where original method exists
        private List<MousePoint> LinearInterpolate(Point a, Point b)
        {
            List<MousePoint> points = new List<MousePoint>();

            int X1 = a.X;
            int Y1 = a.Y;
            int X2 = b.X;
            int Y2 = b.Y;

            int deltaX = Math.Abs(X2 - X1);
            int deltaY = Math.Abs(Y2 - Y1);

            int maxSteps = Math.Max(deltaX, deltaY);

            if (maxSteps <= 1)
            {
                points.Add(new MousePoint(a.X, a.Y));
                return points;
            }

            deltaX = X2 - X1;
            deltaY = Y2 - Y1;

            float increaseX = (float)deltaX / (float)maxSteps;
            float increaseY = (float)deltaY / (float)maxSteps;

            points.Add(new Engine.MousePoint(X1, Y1, Engine.MouseActionType.MouseMove, true));

            // don't add last point, it creates a duplicate when tracing next particle position
            for (int i = 1; i < maxSteps; i++)
            {
                int stepX = (int)((i * increaseX) + (float)X1);
                int stepY = (int)((i * increaseY) + (float)Y1);

                // have these coordinates modified to take into account zoom and pan
                MousePoint p2 = new Engine.MousePoint(stepX, stepY, Engine.MouseActionType.MouseMove, false);
                points.Add(p2);
            }

            points.Add(new Engine.MousePoint(X2, Y2, Engine.MouseActionType.MouseMove, true));


            return points;
        }

        public override string Name { get => "Force Effect"; }
    }
}
