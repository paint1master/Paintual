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

using Engine.Tools;
using Engine.Effects.Code.Particles;
using System.Runtime.InteropServices;

namespace Engine.Effects
{
    public class Flow_2 : Effect
    {
        private int t_cellSize = 20;
        private int t_steps = 10;
        private int t_flowLength = 5;
        private byte t_alpha = 100;

        private bool t_invertLuminance = false;
        private bool t_glassBlock = false;

        private PressureGrid t_pressureGrid;
        private Engine.Effects.Code.Particles.AutonomousParticle[] t_particles;

        public Flow_2()
        {
            t_visualProperties = new VisualProperties(Name, typeof(Flow_2));
        }


        public override IGraphicActivity Duplicate(Engine.Workflow w)
        {
            Flow_2 f2 = new Flow_2();
            f2.Initialize(w);

            return f2;
        }

        public override void Process()
        {
            t_workflow.AllowInvalidate = true;

            t_workflow.ThreadingQueue.RunAndForget(new Action(ThreadedProcess));
        }

        private void ThreadedProcess()
        {
            t_imageProcessed = Engine.Surface.Ops.Copy(t_imageSource);

            for (int i = 0; i < t_steps; i++)
            {
                t_pressureGrid = new PressureGrid(t_imageProcessed, t_cellSize, t_cellSize, t_invertLuminance);
                t_particles = t_pressureGrid.GetParticles();
                Loop();
            }

            base.PostProcess();
        }

        private void Loop()
        {
            Engine.Threading.ThreadedLoop loop = new Threading.ThreadedLoop();

            for (int i = 0; i < t_flowLength; i++)
            {
                Engine.Threading.ParamList paramList = new Threading.ParamList();
                paramList.Add("counter", typeof(int), i);
                loop.Loop(t_particles.Length, Threaded_Loop, paramList);
            }

            loop.Dispose();
        }

        private int Threaded_Loop(int start, int end, Threading.ParamList paramList)
        {
            int counter = (int)paramList.Get("counter").Value;

            Engine.Threading.ThreadedLoop loop = new Threading.ThreadedLoop();

            for (int ii = start; ii < end; ii++)
            {
                // collect pixels in each grid cell
                Engine.Color.Cell[] cells = new Engine.Color.Cell[t_cellSize * t_cellSize];

                AutonomousParticle p = t_particles[ii];

                Engine.Threading.ParamList paramList2 = new Threading.ParamList();
                paramList2.Add("particle", typeof(AutonomousParticle), p);
                paramList2.Add("cells", typeof(Engine.Color.Cell[]), cells);
                paramList2.Add("counter", typeof(int), counter);

                loop.Loop(t_cellSize, Threaded_Loop_Cell_Part_1, paramList2);

                p.Move();

                loop.Loop(t_cellSize, Threaded_Loop_Cell_Part_2, paramList2);
            }

            loop.Dispose();

            return 0;
        }

        private int Threaded_Loop_Cell_Part_1(int start, int end, Threading.ParamList paramList)
        {
            AutonomousParticle p = (AutonomousParticle)paramList.Get("particle").Value;
            Engine.Color.Cell[] cells = (Engine.Color.Cell[])paramList.Get("cells").Value;
            int counter = (int)paramList.Get("counter").Value;

            int offset = 0;

            if (!t_glassBlock)
            {
                offset = Engine.Surface.Ops.GetGridOffset(0, start, t_cellSize, t_cellSize);
            } 

            for (int y = start; y < end; y++)
            {
                for (int x = 0; x < t_cellSize; x++)
                {
                    cells[offset] = t_imageProcessed.GetPixel((int)(p.Position.X - (t_cellSize / 2) + x), (int)(p.Position.Y - (t_cellSize / 2) + y), Surface.PixelRetrievalOptions.ReturnNeutralGray);
                    cells[offset].Alpha = (byte)(t_alpha - (2 * counter));
                    offset++;
                }
            }

            return 0;
        }

        private int Threaded_Loop_Cell_Part_2(int start, int end, Threading.ParamList paramList)
        {
            AutonomousParticle p = (AutonomousParticle)paramList.Get("particle").Value;
            Engine.Color.Cell[] cells = (Engine.Color.Cell[])paramList.Get("cells").Value;
            int counter = (int)paramList.Get("counter").Value;

            int offset = 0;

            if (!t_glassBlock)
            {
                offset = Engine.Surface.Ops.GetGridOffset(0, start, t_cellSize, t_cellSize);
            } 

            for (int y = start; y < end; y++)
            {
                for (int x = 0; x < t_cellSize; x++)
                {
                    cells[offset] = Engine.Calc.Color.FastAlphaBlend(cells[offset], t_imageProcessed.GetPixel((int)(p.Position.X - (t_cellSize / 2) + x), (int)(p.Position.Y - (t_cellSize / 2) + y), Surface.PixelRetrievalOptions.ReturnNeutralGray));
                    t_imageProcessed.SetPixel(cells[offset], (int)(p.Position.X - (t_cellSize / 2) + x), (int)(p.Position.Y - (t_cellSize / 2) + y), Surface.PixelSetOptions.Ignore);
                    offset++;
                }
            }

            return 0;
        }

        public override string Name { get => "Flow_2"; }

        [Engine.Attributes.Meta.DisplayName("Cell Size")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.Range(0, 200)]
        [Engine.Attributes.Meta.DefaultValue(20)]
        public int DiffusionIterations { get => t_cellSize; set => t_cellSize = value; }

        [Engine.Attributes.Meta.DisplayName("Steps")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.Range(0, 100)]
        [Engine.Attributes.Meta.DefaultValue(2)]
        public int Steps { get => t_steps; set => t_steps = value; }

        [Engine.Attributes.Meta.DisplayName("Flow Length")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.Range(0, 100)]
        [Engine.Attributes.Meta.DefaultValue(8)]
        public int FlowLength { get => t_flowLength; set => t_flowLength = value; }

        [Engine.Attributes.Meta.DisplayName("Alpha")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Textbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Int)]
        [Engine.Attributes.Meta.Validator(Engine.Attributes.Meta.ValidatorTypes.Int, "")]
        [Engine.Attributes.Meta.DefaultValue(200)]
        [Engine.Attributes.Meta.Range(0, 255)]
        public int Alpha
        {
            get { return t_alpha; }
            set { t_alpha = (byte)value; }
        }

        [Engine.Attributes.Meta.DisplayName("Invert Luminance")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Checkbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Boolean, typeof(bool))]
        public bool InvertLuminance
        {
            get { return t_invertLuminance; }
            set { t_invertLuminance = value; }
        }

        [Engine.Attributes.Meta.DisplayName("Glass Block (large cell sizes)")]
        [Engine.Attributes.Meta.DisplayControlType(Engine.Attributes.Meta.DisplayControlTypes.Checkbox)]
        [Engine.Attributes.Meta.DataType(PropertyDataTypes.Boolean, typeof(bool))]
        public bool GlassBlock
        {
            get { return t_glassBlock; }
            set { t_glassBlock = value; }
        }
    }
}
