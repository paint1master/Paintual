using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects.Particles
{
    public static class Extensions
    {
        public static Engine.Calc.Vector Distance(this Engine.Effects.Particles.Attractor attractor, Engine.Effects.Particles.BaseParticle p)
        {
            Engine.Calc.Vector distance = attractor.Position - p.Position;

            return distance;
        }


        /*public static Engine.Calc.Vector ExpressiveForce(this Engine.Effects.Particles.Attractor attractor,
            Engine.Effects.Particles.ForceParticle p)
        {
            float G = 4f;

            Engine.Calc.Vector relDistance = attractor.Distance(p);

            float distance = relDistance.Norm; // Norm seems the magnitude in Processing

            float force = G / distance;

            force = Engine.Calc.Math.Sigmoid(force);

            relDistance.Normalize();
            Engine.Calc.Vector.Multiply(relDistance, force);

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
        public static Engine.Calc.Vector ModularForce(this Engine.Effects.Particles.Attractor attractor,
            Engine.Effects.Particles.ForceParticle p)
        {
            Engine.Calc.Vector relDistance = attractor.Distance(p);

            double distance = System.Math.Sqrt(relDistance.X * relDistance.X + relDistance.Y * relDistance.Y);

            double force = (attractor.Intensity * attractor.Force * distance) - 10d / System.Math.Pow(attractor.Expression, -1 * (distance / attractor.Force));

            relDistance.Normalize();

            relDistance *= force;

            return relDistance;
        }

        public static void Draw(this Engine.Effects.Particles.ForceParticle fp, Engine.Surface.Canvas c, Engine.Color.Cell color)
        {
            // TODO a strange case where position is way out of bounds which causes LinearInterpolation to build a list that
            // has so many items that it reaches "out of bounds (int)"
            // this problem may have disappeared since I changed to Engine.Calc.Vector
            if (double.IsNaN(fp.Position.X) || double.IsNaN(fp.Position.Y))
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
