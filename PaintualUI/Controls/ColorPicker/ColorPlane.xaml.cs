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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PaintualUI.Controls.ColorPicker
{
    /// <summary>
    /// Interaction logic for ColorPlane.xaml
    /// </summary>
    public partial class ColorPlane : UserControl
    {
        private Dictionary<int, Engine.Surface.Canvas> t_bitmaps;
        private Engine.Surface.Canvas t_currentPlaneImage;
        private ColorPlaneSelectionGlass t_selectionGlass;

        public ColorPlane()
        {
            InitializeComponent();

            t_bitmaps = new Dictionary<int, Engine.Surface.Canvas>();


            this.Loaded += ColorPlane_Loaded;
        }

        private void ColorPlane_Loaded(object sender, RoutedEventArgs e)
        {
            // can't be set in Initialize !!??!!
            t_selectionGlass = new ColorPlaneSelectionGlass(t_currentPlaneImage);
            t_selectionGlass.Width = this.Width;
            t_selectionGlass.Height = this.Height;

            t_selectionGlass.Margin = new Thickness(0);
            t_selectionGlass.HorizontalAlignment = HorizontalAlignment.Left;
            t_selectionGlass.VerticalAlignment = VerticalAlignment.Top;
            

            this.ContainerGrid.Children.Add(t_selectionGlass);
            Canvas.SetZIndex(t_selectionGlass, 2);

            t_selectionGlass.ColorChanged += T_selectionGlass_ColorChanged;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (t_currentPlaneImage == null)
            {
                SetPlaneImage(new Engine.Color.Cell(0, 0, 255, 255));
            }

            drawingContext.DrawImage(ConvertPlaneImage(t_currentPlaneImage), new Rect(0, 0, this.Width, this.Height));

            base.OnRender(drawingContext);
        }

        private BitmapSource ConvertPlaneImage(Engine.Surface.Canvas canvas)
        {
            BitmapSource bmpSource = BitmapSource.Create(canvas.Width, canvas.Height, 96, 96, PixelFormats.Bgra32, null, canvas.Array, canvas.Stride);

            return bmpSource;
        }

        private void CreatePlaneImage(Engine.Color.Cell c)
        {
            Engine.Surface.ColorPickerPlane cpp = new Engine.Surface.ColorPickerPlane((int)this.Width, (int)this.Height);
            cpp.SetColors(new Engine.Color.Cell(255, 255, 255, 255), c, new Engine.Color.Cell(0, 0, 0, 255), new Engine.Color.Cell(0, 0, 0, 255));

            t_currentPlaneImage = cpp.Canvas;
        }

        private void SetPlaneImage(Engine.Color.Cell c)
        {
            if (t_bitmaps.TryGetValue(c.Int, out t_currentPlaneImage) == false)
            {
                CreatePlaneImage(c);

                t_bitmaps.Add(c.Int, t_currentPlaneImage);
            }
        }

        public void UpdatePlaneImage(Engine.Color.Cell c)
        {
            SetPlaneImage(c);
            t_selectionGlass.UpdateColorPlane(t_currentPlaneImage);
            this.InvalidateVisual();
        }

        public event ColorChangedEventHandler ColorChanged;

        private void T_selectionGlass_ColorChanged(object sender, ColorChangedEventArgs e)
        {
            ColorChanged?.Invoke(this, e);
        }
    }
}
