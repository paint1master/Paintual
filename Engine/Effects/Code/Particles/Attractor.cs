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

        public Attractor(Engine.Calc.Vector position) : base(position)
        {

        }

        public Attractor(Engine.Calc.Vector position, double G) : base(position)
        {
            Force = G;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="G">The base value of the force of "gravity". G must be greater than 0. If not, value defaults to 0.1d.
        /// Suggested value 1.2d.</param>
        /// <param name="expression">A parameter that influences the intensity curve of the calculated gravity force. Cannot be less
        /// than 0. Sugested value 1.3d. The higher the value, the less intense the gravity curve is.</param>
        /// <param name="intensity">A positive value means attraction, a negative value means repulsion.</param>
        public Attractor(Engine.Calc.Vector position, double G, double expression, double intensity) : base(position)
        {
            Force = G > 0 ? G : 0.1d;
            Expression = expression > 0 ? expression : 0.1d;
            Intensity = intensity;
        }

        public double Force { get; set ; }
        public double Expression { get; set; }
        public double Intensity { get; set; }
    }
}
