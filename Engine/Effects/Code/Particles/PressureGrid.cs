using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects.Code.Particles
{
    public class PressureGrid
    {

        private Engine.Surface.Canvas t_imageSource;
        private Engine.Effects.Particles.FlowField t_flowField;
        private Engine.Effects.Code.Particles.PressureGridCell[,] t_cells;
        private Engine.Surface.Canvas t_vectorFieldSketch;

        private int t_gridCellWidth;
        private int t_gridCellHeight;

        public PressureGrid(Engine.Surface.Canvas c, int gridCellWidth, int gridCellHeight, bool invertLuminance)
        {
            t_imageSource = c;

            t_gridCellWidth = gridCellWidth;
            t_gridCellHeight = gridCellHeight;

            t_flowField = new Effects.Particles.FlowField(t_imageSource, invertLuminance);

            SetGrid();
        }

        private void SetGrid()
        {
            int numberRows = Width / t_gridCellWidth;
            int numberColumns = Height / t_gridCellHeight;

            /*
            // depending on image ratio, portion of image may not be modified because there is one row missing
            if (numberRows * t_gridCellWidth < Width)
            {
                numberRows++;
            }

            // depending on image ratio, portion of image may not be modified because there is one column missing
            if (numberColumns * t_gridCellHeight < Height)
            {
                numberColumns++;
            }*/

            t_cells = new PressureGridCell[numberRows, numberColumns];

            Engine.Threading.ThreadedLoop loop = new Threading.ThreadedLoop();

            Engine.Threading.ParamList paramList = new Threading.ParamList();
            paramList.Add("numCol", typeof(int), numberColumns);

            loop.Loop(numberRows, Threaded_SetGrid, paramList);
            loop.Dispose();
        }

        private int Threaded_SetGrid(int start, int end, Engine.Threading.ParamList paramList)
        {
            int numCol = (int)paramList.Get("numCol").Value;

            for (int x = start; x < end; x++)
            {
                for (int y = 0; y < numCol; y++)
                {
                    t_cells[x, y] = new PressureGridCell(t_gridCellWidth, t_gridCellHeight);
                    t_cells[x, y].CalculateAveragePressure(t_flowField, x * t_gridCellWidth, y * t_gridCellHeight);
                }
            }

            return 0;
        }

        public Engine.Effects.Code.Particles.AutonomousParticle[] GetParticles()
        {
            Engine.Effects.Code.Particles.AutonomousParticle[] particles = new Effects.Code.Particles.AutonomousParticle[t_cells.GetLength(0) * t_cells.GetLength(1)];

            Engine.Threading.ParamList paramList = new Threading.ParamList();
            paramList.Add("particles", typeof(int), particles);

            Engine.Threading.ThreadedLoop loop = new Threading.ThreadedLoop();
            loop.Loop(t_cells.GetLength(0), Threaded_GetParticles, paramList);
            loop.Dispose();

            return particles;
        }

        private int Threaded_GetParticles(int start, int end, Threading.ParamList paramlist)
        {
            Engine.Effects.Code.Particles.AutonomousParticle[] particles = (Engine.Effects.Code.Particles.AutonomousParticle[])paramlist.Get("particles").Value;

            for (int x = start; x < end; x++)
            {
                for (int y = 0; y < t_cells.GetLength(1); y++)
                {
                    int offset = Engine.Surface.Ops.GetGridOffset(x, y, t_cells.GetLength(0), t_cells.GetLength(1));

                    particles[offset] = new Effects.Code.Particles.AutonomousParticle(new Engine.Calc.Vector((x * t_gridCellWidth) + (t_gridCellWidth / 2),
                        (y * t_gridCellHeight) + (t_gridCellHeight / 2)));

                    Engine.Calc.Vector vel = t_cells[x, y].Pressure;
                    vel.Normalize();
                    vel.SetMagnitude(2);
                    particles[offset].Velocity = vel;

                    //t_cells[x, y].CalculateAveragePressure(t_flowField, x * t_gridCellWidth, y * t_gridCellHeight);
                }
            }

            return 0;
        }

        public Engine.Surface.Canvas GetVectorFieldSketch()
        {
            t_vectorFieldSketch = new Surface.Canvas(Width, Height, Engine.Colors.White);

            for (int x = 0; x < t_cells.GetLength(0); x++)
            {
                for (int y = 0; y < t_cells.GetLength(1); y++)
                {
                    PressureGridCell gc = t_cells[x, y];
                    Engine.Point pStart = new Point((x * gc.Width) - (gc.Width / 2), (y * gc.Height) - (gc.Height / 2));
                    Engine.Calc.Vector shortV = new Calc.Vector(0, 0);
                    shortV += gc.Pressure;
                    shortV.SetMagnitude(gc.Width / 2);
                    Engine.Point pEnd = new Point(pStart.X + (int)shortV.X, pStart.Y + (int)shortV.Y);
                    Engine.Tools.Drawing.DrawLine(t_vectorFieldSketch, pStart, pEnd, Engine.Colors.Black);
                }
            }

            return t_vectorFieldSketch;
        }


        public int Width { get => t_imageSource.Width; }
        public int Height { get => t_imageSource.Height; }
    }
}
