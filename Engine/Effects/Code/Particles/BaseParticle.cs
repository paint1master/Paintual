﻿/**********************************************************

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
    public class BaseParticle
    {
        protected Engine.Calc.Vector t_position;

        public BaseParticle()
        {

        }

        public BaseParticle(Engine.Calc.Vector position)
        {
            t_position = new Engine.Calc.Vector(position.X, position.Y);
        }

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

namespace Engine.Effects.Particles.Obsolete
{
    [Obsolete("Kept for compatibility with Glitch, Radial effects and ThinLineTool and ParticlePen tools.")]
    public class BaseParticle_O
    {
        protected Engine.Calc.Vector t_position;
        protected Engine.Calc.Vector t_velocity;


        public BaseParticle_O()
        {

        }

        public void Update()
        {
            t_position += t_velocity;
        }

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
