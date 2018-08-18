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
    /// Interaction logic for ColorPlaneSelectionGlass.xaml
    /// </summary>
    public partial class ColorPlaneSelectionGlass : UserControl
    {
        private Engine.Surface.Canvas t_colorPlane;

        private System.Windows.Shapes.Ellipse t_cursor;
        private Point t_cursorLocation;
        private bool t_canMoveCursor;

        public ColorPlaneSelectionGlass(Engine.Surface.Canvas colorPlane)
        {
            InitializeComponent();

            t_colorPlane = colorPlane;

            t_cursorLocation = new Point(0, 0);

            t_cursor = new Ellipse();
            t_cursor.Stroke = System.Windows.Media.Brushes.Gray;
            t_cursor.Fill = System.Windows.Media.Brushes.Transparent;
            t_cursor.Width = 10;
            t_cursor.Height = 10;
            t_cursor.StrokeThickness = 2;
            t_cursor.HorizontalAlignment = HorizontalAlignment.Left;
            t_cursor.VerticalAlignment = VerticalAlignment.Top;
            Thickness t = new Thickness();
            t.Left = 0;
            t.Top = 0;
            t_cursor.Margin = t;

            t_cursor.MouseDown += T_cursor_MouseDown;
            t_cursor.MouseMove += T_cursor_MouseMove;
            t_cursor.MouseUp += T_cursor_MouseUp;

            this.Glass.Children.Add(t_cursor);
        }

        private void T_cursor_MouseUp(object sender, MouseButtonEventArgs e)
        {           
            t_canMoveCursor = false;

            RaisingColorChanged();
        }

        private void T_cursor_MouseMove(object sender, MouseEventArgs e)
        {
            if (!t_canMoveCursor)
                return;

            UpdateCursorPosition(e.GetPosition(this));
            RaisingColorChanged();
            this.InvalidateVisual();
        }

        private void T_cursor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            t_canMoveCursor = true;
        }

        private void Glass_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdateCursorPosition(e.GetPosition(this));

            t_canMoveCursor = true;
            this.InvalidateVisual();
        }

        private void Glass_MouseMove(object sender, MouseEventArgs e)
        {            
            if (!t_canMoveCursor)
                return;

            UpdateCursorPosition(e.GetPosition(this));
            this.InvalidateVisual();
        }

        private void Glass_MouseUp(object sender, MouseButtonEventArgs e)
        {            
            t_canMoveCursor = false;
        }

        private void UpdateCursorPosition(Point p)
        {
            t_cursorLocation = p;
            Thickness t = t_cursor.Margin;
            t.Left = p.X - 5;
            t.Top = p.Y - 5;
            t_cursor.Margin = t;
        }

        public void UpdateColorPlane(Engine.Surface.Canvas plane)
        {
            t_colorPlane = plane;
            RaisingColorChanged();
        }

        private void RaisingColorChanged()
        {
            // get color from plane
            Engine.Color.Cell c = t_colorPlane.GetPixel((int)t_cursorLocation.X, (int)t_cursorLocation.Y, Engine.Surface.PixelRetrievalOptions.ReturnEdgePixel);

            ColorChangedEventArgs ce = new ColorChangedEventArgs(c);
            OnColorChanged(ce);
        }

        /// <summary>
        /// Raised when mouse releases the color cursor 
        /// </summary>
        public event ColorChangedEventHandler ColorChanged;

        protected virtual void OnColorChanged(ColorChangedEventArgs e)
        {
            ColorChanged?.Invoke(this, e);
        }
    }
}
