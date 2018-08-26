using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Effects.Code.Particles
{
    public class PressureGridCell
    {
        private int t_width;
        private int t_height;

        private Engine.Calc.Vector t_pressure;

        public PressureGridCell(int width, int height)
        {
            t_width = width;
            t_height = height;
        }

        public void CalculateAveragePressure(Engine.Effects.Particles.FlowField flowField, int startX, int startY)
        {
            t_pressure = new Calc.Vector(0, 0);

            for (int y = 0; y < t_height; y++)
            {
                for (int x = 0; x < t_width; x++)
                {
                    Engine.Calc.Vector v = flowField.GetVector(x + startX, y + startY);
                    t_pressure += v;

                }
            }

            //t_pressure.Normalize(Calc.CalculationStyles.Accord);
        }

        public Engine.Calc.Vector Pressure { get => t_pressure; }

        public int Width { get => t_width; }
        public int Height { get => t_height; }
    }
}
