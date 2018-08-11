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
        protected Accord.Math.Vector3 t_velocity;
        protected Accord.Math.Vector3 t_acceleration;

        public ForceParticle()
        {
            t_acceleration = new Accord.Math.Vector3(0, 0, 0);
        }

        public ForceParticle(Accord.Math.Vector3 position) : base(position)
        {
            t_previousPoint = new Point((int)position.X, (int)position.Y);
        }

        public ForceParticle(Accord.Math.Vector3 position, Accord.Math.Vector3 velocity) : base(position)
        {
            t_previousPoint = new Point((int)position.X, (int)position.Y);
            t_velocity = velocity;
        }

        public ForceParticle(int x, int y, float velX, float velY)
        {
            t_position = new Accord.Math.Vector3(x, y, 0);
            t_velocity = new Accord.Math.Vector3(velX, velY, 0);
        }

        public void Update()
        {
            t_previousPoint = new Point((int)Math.Round(t_position.X), (int)Math.Round(t_position.Y));
            t_velocity += t_acceleration;
            t_position += t_velocity;
        }

        /// <summary>
        /// Updates the position of the particule according to its own position, velocity, and the force (and location) of the
        /// specified attractor.
        /// </summary>
        /// <param name="attractor"></param>
        public void Update(Engine.Effects.Particles.Attractor attractor)
        {
            t_acceleration = attractor.ForceInverseProportional_SqDistance(this);
            Update();
        }

        /// <summary>
        /// Updates the position of the particule according to its own position, velocity, and the force (and location) of the
        /// specified attractors.
        /// </summary>
        /// <param name="attractors"></param>
        public void Update(Engine.Effects.Particles.Attractor[] attractors)
        {
            Accord.Math.Vector3 sum = new Vector3(0, 0, 0);

            for (int i = 0; i < attractors.Length; i++)
            {
                sum += attractors[i].ForceInverseProportional_SqDistance(this);
            }
            t_acceleration = sum;
            Update();
        }

        public override void Move(Accord.Math.Vector3 direction)
        {
            throw new InvalidOperationException("In ForceParticle.Move(), use any overload of the Update() method instead.");
        }

        public Accord.Math.Vector3 Velocity
        {
            get => t_velocity;
            set => t_velocity = value;
        }

        public Accord.Math.Vector3 Acceleration
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
        protected Accord.Math.Vector3 t_position;
        protected Accord.Math.Vector3 t_velocity;
        //protected Accord.Math.Vector3 t_acceleration;


        public ForceParticle_O()
        {

        }

        public void Update()
        {
            //t_velocity += t_acceleration;
            t_position += t_velocity;
            //t_acceleration = t_acceleration * 0;
        }

        /*public void ApplyForce(Accord.Math.Vector3 force)
        {
            t_acceleration += force;
        }*/

        public virtual void Move(Accord.Math.Vector3 direction)
        {
            t_position += direction;
        }

        public Accord.Math.Vector3 Position
        {
            get { return t_position; }
        }
    }
}


