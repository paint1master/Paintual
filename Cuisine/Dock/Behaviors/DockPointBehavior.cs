/**********************************************************

Part of the Synergy code created by Ashish Kaila (https://www.codeproject.com/Articles/140209/Building-a-Docking-Window-Management-Solution-in-W),
code which is licensed under The Code Project Open License (CPOL).
Details of the license can be found in the accompanying file : cpol_license.htm

**********************************************************/

//
// Copyright(C) MixModes Inc. 2011
// 

using System;
using System.Windows.Controls;
using System.Windows.Input;
using Cuisine.Windows;

namespace Cuisine.Behaviors
{
    /// <summary>
    /// Illustrates the docking behavior for DockPanels within WindowsManager
    /// </summary>
    public class DockPointBehavior : VisualParentBehavior<WindowsManager>
    {                
        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>Override this to hook up functionality to the AssociatedObject.</remarks>
        /// <exception cref="InvalidOperationException">WindowsManager does not exist in logical tree</exception>        
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseEnter += OnMouseEnter;
            AssociatedObject.MouseUp += OnMouseUp;
            AssociatedObject.MouseLeave += OnMouseLeave;                
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>Override this to unhook functionality from the AssociatedObject.</remarks>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.MouseEnter -= OnMouseEnter;
            AssociatedObject.MouseUp -= OnMouseUp;
            AssociatedObject.MouseLeave -= OnMouseLeave;
        }

        /// <summary>
        /// Called when mouse enters the associated object
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void OnMouseEnter(object sender, MouseEventArgs args)
        {
            Dock dock = DockPanel.GetDock(AssociatedObject);

            WindowsManager windowsManager = VisualParent;

            // Retrieve the current illustration if any
            if ((windowsManager.DockingIllustrationPanel.Children.Count > 0) &&
                (DockPanel.GetDock(windowsManager.DockingIllustrationPanel.Children[0]) == dock))
            {
                return;
            }

            windowsManager.DockingIllustrationPanel.Children.Clear();

            Grid dockPaneIllustratingGrid = new Grid();

            dockPaneIllustratingGrid.Style = WindowsManager.GetDockPaneIllustrationStyle(windowsManager);

            if ((dock == Dock.Bottom) || (dock == Dock.Top))
            {
                dockPaneIllustratingGrid.Width = double.NaN;
                dockPaneIllustratingGrid.Height = windowsManager.DraggedPane.ActualHeight;
            }
            else
            {
                dockPaneIllustratingGrid.Width = windowsManager.DraggedPane.ActualWidth;
                dockPaneIllustratingGrid.Height = double.NaN;
            }

            DockPanel.SetDock(dockPaneIllustratingGrid, dock);
            windowsManager.DockingIllustrationPanel.Children.Add(dockPaneIllustratingGrid);
        }

        /// <summary>
        /// Called when mouse is up on the associated object
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            WindowsManager windowsManager = VisualParent;

            windowsManager.FloatingPanel.Children.Remove(windowsManager.DraggedPane);
            windowsManager.StopDockPaneStateChangeDetection();
            windowsManager.AddPinnedWindow(windowsManager.DraggedPane, DockPanel.GetDock(AssociatedObject));
            windowsManager.StartDockPaneStateChangeDetection();
        }

        /// <summary>
        /// Called when mouse leaves the associated object
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void OnMouseLeave(object sender, MouseEventArgs args)
        {
            VisualParent.DockingIllustrationPanel.Children.Clear();
        }        
    }
}
