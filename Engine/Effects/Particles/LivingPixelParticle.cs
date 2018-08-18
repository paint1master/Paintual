using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;

namespace Engine.Effects.Particles.Obsolete
{
    [Obsolete("Kept for compatibility with Glitch, Radial effects and ThinLineTool and ParticlePen tools.")]
    public class LivingPixelParticle_O : PixelParticle_O
    {
        double t_life = 0;
        private Accord.Math.Vector3 t_direction;

        public LivingPixelParticle_O() : base()
        {

        }

        public LivingPixelParticle_O(Engine.Color.Cell c, int x, int y) : base(c, x, y)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="life"></param>
        /// <param name="expansion">A coefficient that increases the magnitude of the direction vector.</param>
        public LivingPixelParticle_O(Engine.Color.Cell c, int x, int y, double life, int expansion) : base(c, x, y)
        {
            t_life = life;

            float dirX = (float)(Engine.Calc.Math.Rand.NextDouble() - 0.5d);
            float dirY = (float)(Engine.Calc.Math.Rand.NextDouble() - 0.5d);

            t_direction = new Accord.Math.Vector3(expansion * dirX, expansion * dirY, 0);
        }


        public void Draw(Engine.Surface.Canvas c)
        {
            Engine.Color.Cell source = c.GetPixel((int)t_position.X, (int)t_position.Y, Surface.PixelRetrievalOptions.ReturnEdgePixel);
            Engine.Color.Cell dest = Engine.Calc.Color.FastAlphaBlend(t_cell, source);
            c.SetPixel(dest, (int)t_position.X, (int)t_position.Y, Surface.PixelSetOptions.Ignore);
        }

        public void Die()
        {
            t_life = 0;
        }

        public override void Move(Vector3 direction)
        {
            if(t_life <= 0)
            {
                // particle is dead
                return;
            }

            float previousX = t_position.X;
            float previousY = t_position.Y;

            Accord.Math.Vector3 tempDir = direction + t_direction;

            t_position += tempDir;

            float deltaX = System.Math.Abs(t_position.X - previousX);
            float deltaY = System.Math.Abs(t_position.Y - previousY);

            double distance = System.Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            t_life -= distance;
        }

        public double Life
        {
            get { return t_life; }
            set { t_life = value; }
        }
    }
}
