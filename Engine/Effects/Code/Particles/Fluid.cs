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

namespace Engine.Effects.Particles
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Code taken and adapted from : http://cowboyprogramming.com/2008/04/01/practical-fluid-mechanics/ </remarks>
    public class Fluid
    {
        private int t_diffusionIterations;

        //3.0f; //3.5f; // 3.5f works nicely
        private double t_velocityDiffusion;

        // With no pressure diffusion, waves of pressure keep moving
        // But only if pressure is advected ahead of velocity
        private double t_pressureDiffusion;

        //private double t_heatDiffusion;

        private double t_inkDiffusion;

        // friction applied to velocity.  As a simple fraction each time step
        // so 1.0f will give you no friction

        // Friction is very important to the realism of the simulation
        // changes to these coeffficinets will GREATLY affect the apperence of the simulation.
        // the high factor friction (a) wich is applied to the square of the velocity,
        // acts as a smooth limit to velocity, which causes streams to break up into turbulent flow 
        // The c term will make things stop, which then shows up our border artefacts
        // but a high enough (>0.01) c term is needed to stop things just dissipating too fast
        // Friction is dt*(a*v^2 + b*v + c)
        private double t_velocityFriction_A;
        private double t_velocityFriction_B;
        private double t_velocityFriction_C;

        // Pressure acceleration.  Larger values (>10) are more realistic (like water)
        // values that are too large lead to chaotic waves
        private double t_vorticity;
        private double t_pressureAcc;
        private double t_inkHeat;
        private double t_heatForce;
        private double t_heatFriction_A;
        private double t_heatFriction_B;
        private double t_heatFriction_C;

        // high ink advection allows fast moving nice swirlys
        private double t_inkAdvection;
        // seems nice if both velocity and pressure at same value, which makes sense
        private double t_velocityAdvection;
        private double t_pressureAdvection;
        private double t_heatAdvection;

        public Fluid()
        {

        }

        public int DiffusionIterations
        {
            get { return t_diffusionIterations; }
            set { t_diffusionIterations = value; }
        }

        public double VelocityDiffusion
        {
            get { return t_velocityDiffusion; }
            set { t_velocityDiffusion = value; }
        }

        public double PressureDiffusion
        {
            get { return t_pressureDiffusion; }
            set { t_pressureDiffusion = value; }
        }

        /*public double HeatDiffusion
        {
            get { return t_heatDiffusion; }
            set { t_heatDiffusion = value; }
        }*/

        /*
        public double InkDiffusion
        {
            get { return t_inkDiffusion; }
            set { t_inkDiffusion = value; }
        }*/

        public double VelocityFriction_A
        {
            get { return t_velocityFriction_A; }
            set { t_velocityFriction_A = value; }
        }

        public double VelocityFriction_B
        {
            get { return t_velocityFriction_B; }
            set { t_velocityFriction_B = value; }
        }

        public double VelocityFriction_C
        {
            get { return t_velocityFriction_C; }
            set { t_velocityFriction_C = value; }
        }

        public double Vorticity
        {
            get { return t_vorticity; }
            set { t_vorticity = value; }
        }

        public double PressureAcceleration
        {
            get { return t_pressureAcc; }
            set { t_pressureAcc = value; }
        }
        /*
        public double InkHeat
        {
            get { return t_inkHeat; }
            set { t_inkHeat = value; }
        }

        public double HeatForce
        {
            get { return t_heatForce; }
            set { t_heatForce = value; }
        }

        public double HeatFriction_A
        {
            get { return t_heatFriction_A; }
            set { t_heatFriction_A = value; }
        }

        public double HeatFriction_B
        {
            get { return t_heatFriction_B; }
            set { t_heatFriction_B = value; }
        }

        public double HeatFriction_C
        {
            get { return t_heatFriction_C; }
            set { t_heatFriction_C = value; }
        }

        public double InkAdvection
        {
            get { return t_inkAdvection; }
            set { t_inkAdvection = value; }
        }
        */
        public double VelocityAdvection
        {
            get { return t_velocityAdvection; }
            set { t_velocityAdvection = value; }
        }

        public double PressureAdvection
        {
            get { return t_pressureAdvection; }
            set { t_pressureAdvection = value; }
        }
        /*
        public double HeatAdvection
        {
            get { return t_heatAdvection; }
            set { t_heatAdvection = value; }
        }
        */

    }
}
