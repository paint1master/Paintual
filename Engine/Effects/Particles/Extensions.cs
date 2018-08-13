using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects.Particles
{
    public static class Extensions
    {
        public static Accord.Math.Vector3 Distance(this Engine.Effects.Particles.Attractor attractor, Engine.Effects.Particles.BaseParticle p)
        {
            Accord.Math.Vector3 distance = attractor.Position - p.Position;

            return distance;
        }


        public static Accord.Math.Vector3 ForceInverseProportional_SqDistance(this Engine.Effects.Particles.Attractor attractor,
            Engine.Effects.Particles.ForceParticle p)
        {
            float G = 4f; //50.5f;

            Accord.Math.Vector3 relDistance = attractor.Distance(p);

            float distance = relDistance.Norm; // Norm seems the magnitude in Processing

            float force = G / distance;

            force = Engine.Calc.Math.Sigmoid(force);

            relDistance.Normalize();
            Accord.Math.Vector3.Multiply(relDistance, force);

            return relDistance;
        }

        public static void Draw(this Engine.Effects.Particles.ForceParticle fp, Engine.Surface.Canvas c, Engine.Color.Cell color)
        {
            // TODO a strange case where position is way out of bounds which causes LinearInterpolation to build a list that
            // reaches "out of memory"
            if (float.IsNaN(fp.Position.X) || float.IsNaN(fp.Position.Y))
            {
                return;
            }

            // draw a line between last position and current position
            List<MousePoint> points = Engine.Calc.Math.LinearInterpolate(new MousePoint(fp.PreviousPoint.X, fp.PreviousPoint.Y),
                new MousePoint((int)fp.Position.X, (int)fp.Position.Y));

            foreach (MousePoint p in points)
            {
                int x = p.X;
                int y = p.Y;

                if (x < 0 || x > c.Width - 1)
                {
                    return;
                }

                if (y < 0 || y > c.Height - 1)
                {
                    return;
                }

                int offset = c.GetOffset(x, y);

                Engine.Color.Cell bg = Engine.Surface.Ops.GetPixel(c, x, y);
                Engine.Calc.Color.FastAlphaBlend(color, bg).WriteBytes(c.Array, offset);
            }
        }
    }
}
