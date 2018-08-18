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


        /*public static Accord.Math.Vector3 ExpressiveForce(this Engine.Effects.Particles.Attractor attractor,
            Engine.Effects.Particles.ForceParticle p)
        {
            float G = 4f;

            Accord.Math.Vector3 relDistance = attractor.Distance(p);

            float distance = relDistance.Norm; // Norm seems the magnitude in Processing

            float force = G / distance;

            force = Engine.Calc.Math.Sigmoid(force);

            relDistance.Normalize();
            Accord.Math.Vector3.Multiply(relDistance, force);

            return relDistance;
        }*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attractor"></param>
        /// <param name="p"></param>
        /// <param name="G">The "Gravitational constant" of the force (suggested value 1.3)</param>
        /// <param name="expansion">Allows the force to be stronger or weaker on longer distances while remaining weak at very short distances.
        /// suggested value 2.8</param>
        /// <param name="direction">the direction of the force. positive makes the force attraction, negative makes the force repulsion. suggested value 0.5 or -0.5</param>
        /// <returns></returns>
        public static Accord.Math.Vector3 ModularForce(this Engine.Effects.Particles.Attractor attractor,
            Engine.Effects.Particles.ForceParticle p)
        {
            Accord.Math.Vector3 relDistance = attractor.Distance(p);

            double distance = System.Math.Sqrt(relDistance.X * relDistance.X + relDistance.Y * relDistance.Y);

            //double force = (attractor.Intensity * attractor.Force * distance) -attractor.Expression / (attractor.Expression * System.Math.Exp(distance /attractor.Force));
            double force = (attractor.Intensity * attractor.Force * distance) - 10d / System.Math.Pow(attractor.Expression, -1 * (distance / attractor.Force));

            relDistance.Normalize();
            Accord.Math.Vector3.Multiply(relDistance, (float)force);

            return relDistance;
        }

        public static void Draw(this Engine.Effects.Particles.ForceParticle fp, Engine.Surface.Canvas c, Engine.Color.Cell color)
        {
            // TODO a strange case where position is way out of bounds which causes LinearInterpolation to build a list that
            // has so many items that it reaches "out of bounds (int)"
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

                if (c.IsOutOfBounds(x, y))
                {
                    return;
                }

                Engine.Color.Cell bg = c.GetPixel(x, y, Surface.PixelRetrievalOptions.ReturnEdgePixel);

                c.SetPixel(Engine.Calc.Color.FastAlphaBlend(color, bg), x, y, Surface.PixelSetOptions.Ignore);
            }
        }
    }
}
