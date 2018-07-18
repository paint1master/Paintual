/**********************************************************

Part of the Synergy code created by Ashish Kaila (https://www.codeproject.com/Articles/140209/Building-a-Docking-Window-Management-Solution-in-W),
code which is licensed under The Code Project Open License (CPOL).
Details of the license can be found in the accompanying file : cpol_license.htm

**********************************************************/

//
// Copyright(C) MixModes Inc. 2011
// 

using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Cuisine.Adorners
{
    /// <summary>
    /// Base class for adorners
    /// </summary>
    internal abstract class AdornerBase : Adorner, IDisposable
    {
        /// <summary>
        /// Initializes an Adorner
        /// </summary>
        /// <param name="adornedElement">The element to bind the adorner to</param>
        /// <exception cref="ArgumentNullException">adornedElement is null</exception>
        internal AdornerBase(UIElement adornedElement)
            : base(adornedElement)
        {
            _visualChildren = new VisualCollection(this);
        }

        /// <summary>
        /// Releases resources and disposes the object
        /// </summary>
        public void Dispose()
        {
            _visualChildren.Clear();
        }

        /// <summary>
        /// Number of visual child elements within this element
        /// </summary>
        /// <returns>The number of visual child elements for this element.</returns>
        protected override int VisualChildrenCount { get { return _visualChildren.Count; } }

        /// <summary>
        /// Returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection</param>
        /// <returns>The requested child element</returns>
        /// <exception cref="IndexOutOfBoundsException">Index out of bounds</exception>
        protected override Visual GetVisualChild(int index) { return _visualChildren[index]; }

        // To store and manage the adorner's visual children.
        protected VisualCollection _visualChildren;
    }
}
