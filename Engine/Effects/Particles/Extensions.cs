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

            force = Sigmoid(force);

            relDistance.Normalize();
            Accord.Math.Vector3.Multiply(relDistance, force);

            return relDistance;
        }

        private static float Sigmoid(float f)
        {
            float result = (float) (1d / (1d + Math.Exp(f * -1)));

            return result;
        }
    }
}
