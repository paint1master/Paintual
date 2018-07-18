/**********************************************************

Part of the Synergy code created by Ashish Kaila (https://www.codeproject.com/Articles/140209/Building-a-Docking-Window-Management-Solution-in-W),
code which is licensed under The Code Project Open License (CPOL).
Details of the license can be found in the accompanying file : cpol_license.htm

**********************************************************/

//
// Copyright(C) MixModes Inc. 2011
// 

namespace Cuisine.Windows
{
    /// <summary>
    /// Determines the docked position of a DockPanel within a DocumentContainer
    /// </summary>
    public enum ContentDockPoint
    {
        /// <summary>
        /// Docks as top content
        /// </summary>
        Top,

        /// <summary>
        /// Docks as left content
        /// </summary>
        Left,

        /// <summary>
        /// Docks as the right content
        /// </summary>
        Right,

        /// <summary>
        /// Docks to the bottom
        /// </summary>
        Bottom,

        /// <summary>
        /// Docks as tabbed content
        /// </summary>
        Content
    }
}
