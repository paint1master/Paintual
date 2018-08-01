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
        protected Accord.Math.Vector3 t_position;
        protected Accord.Math.Vector3 t_velocity;
        //protected Accord.Math.Vector3 t_acceleration;


        public BaseParticle()
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