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

using Accord.Math;

namespace Engine.Effects.Particles
{
    /// <summary>
    /// Represents a particule that can move and that its movement is influenced by one or more attractors.
    /// </summary>
    public class ForceParticle : BaseParticle
    {
        protected Point t_previousPoint;
        protected Engine.Calc.Vector t_velocity;
        protected Engine.Calc.Vector t_acceleration = new Engine.Calc.Vector(0, 0);
        protected float t_maxMagnitude = 0;

        public ForceParticle(Engine.Calc.Vector position) : base(position)
        {
            t_previousPoint = new Point((int)position.X, (int)position.Y);
        }

        public ForceParticle(Engine.Calc.Vector position, Engine.Calc.Vector velocity) : base(position)
        {
            t_previousPoint = new Point((int)position.X, (int)position.Y);
            t_velocity = velocity;
        }

        public ForceParticle(int x, int y, float velX, float velY)
        {
            t_previousPoint = new Point(x, y);
            t_position = new Engine.Calc.Vector(x, y);
            t_velocity = new Engine.Calc.Vector(velX, velY);
        }

        public ForceParticle(int x, int y, float velX, float velY, float maxMagnitude) : this(x, y, velX, velY)
        {
            t_maxMagnitude = maxMagnitude;
        }

        public void Update()
        {
            t_previousPoint = new Point((int)Math.Round(t_position.X), (int)Math.Round(t_position.Y));
            t_velocity += t_acceleration;

            if (t_maxMagnitude > 0)
            {
                float mag = (float)System.Math.Sqrt(t_velocity.X * t_velocity.X + t_velocity.Y * t_velocity.Y);
                if (mag > t_maxMagnitude)
                {
                    double x = t_velocity.X * t_maxMagnitude / mag;
                    double y = t_velocity.Y * t_maxMagnitude / mag;

                    if (double.IsNaN(x) || double.IsNaN(y))
                    {
                        x = 0;
                        y = 0;
                    }

                    t_velocity = new Engine.Calc.Vector(x, y);
                }
            }

            t_position += t_velocity;
        }

        /// <summary>
        /// Updates the position of the particule according to its own position, velocity, and the force (and location) of the
        /// specified attractor.
        /// </summary>
        /// <param name="attractor"></param>
        public void Update(Engine.Effects.Particles.Attractor attractor)
        {
            //t_acceleration = attractor.ExpressiveForce(this);
            t_acceleration = attractor.ModularForce(this);
            Update();
        }

        /// <summary>
        /// Updates the position of the particule according to its own position, velocity, and the force (and location) of the
        /// specified attractors.
        /// </summary>
        /// <param name="attractors"></param>
        public void Update(Engine.Effects.Particles.Attractor[] attractors)
        {
            Engine.Calc.Vector sum = new Engine.Calc.Vector(0, 0);

            for (int i = 0; i < attractors.Length; i++)
            {
                sum += attractors[i].ModularForce(this);
                //sum += attractors[i].ExpressiveForce(this);
            }
            t_acceleration = sum;
            Update();
        }

        public override void Move(Engine.Calc.Vector direction)
        {
            throw new InvalidOperationException("In ForceParticle.Move(), use any overload of the Update() method instead.");
        }

        public Engine.Calc.Vector Velocity
        {
            get => t_velocity;
            set => t_velocity = value;
        }

        public Engine.Calc.Vector Acceleration
        {
            get => t_acceleration;
            set => t_acceleration = value;
        }

        public Point PreviousPoint
        {
            get { return t_previousPoint; }
        }
    }
}

namespace Engine.Effects.Particles.Obsolete
{
    [Obsolete("Kept for compatibility with Glitch, Radial effects and ThinLineTool and ParticlePen tools.")]
    public class ForceParticle_O
    {
        protected Engine.Calc.Vector t_position;
        protected Engine.Calc.Vector t_velocity;
        //protected Engine.Calc.Vector t_acceleration;


        public ForceParticle_O()
        {

        }

        public void Update()
        {
            //t_velocity += t_acceleration;
            t_position += t_velocity;
            //t_acceleration = t_acceleration * 0;
        }

        /*public void ApplyForce(Engine.Calc.Vector force)
        {
            t_acceleration += force;
        }*/

        public virtual void Move(Engine.Calc.Vector direction)
        {
            t_position += direction;
        }

        public Engine.Calc.Vector Position
        {
            get { return t_position; }
        }
    }
}


