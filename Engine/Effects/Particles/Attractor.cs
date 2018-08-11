using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects.Particles
{
    public class Attractor : BaseParticle
    {
        public Attractor()
        {
            this.Force = 0;
        }

        public Attractor(Accord.Math.Vector3 position) : base(position)
        {

        }

        public Attractor(Accord.Math.Vector3 position, double force) : base(position)
        {
            Force = force;
        }

        public double Force { get; set ; }
    }
}
