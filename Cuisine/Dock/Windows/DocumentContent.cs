/**********************************************************

Part of the Synergy code created by Ashish Kaila (https://www.codeproject.com/Articles/140209/Building-a-Docking-Window-Management-Solution-in-W),
code which is licensed under The Code Project Open License (CPOL).
Details of the license can be found in the accompanying file : cpol_license.htm

**********************************************************/

//
// Copyright(C) MixModes Inc. 2011
// 

using System.Windows.Input;

namespace Cuisine.Windows
{
    /// <summary>
    /// Document content
    /// </summary>
    public class DocumentContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentContent"/> class.
        /// </summary>
        /// <param name="pane">The pane.</param>
        /// <param name="closeCommand">The close command.</param>
        public DocumentContent(DockPane pane, ICommand closeCommand)
        {
            Header = pane.Header;
            Content = pane.Content;
            DockPane = pane;
            CloseCommand = closeCommand;
            pane.Header = null;
            pane.Content = null;
        }

        /// <summary>
        /// Detaches the dock pane.
        /// </summary>
        public void DetachDockPane()
        {
            DockPane.Header = Header;
            DockPane.Content = Content;
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public object Header { get; private set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public object Content { get; private set; }

        /// <summary>
        /// Gets or sets the dock pane.
        /// </summary>
        /// <value>The dock pane.</value>
        public DockPane DockPane { get; private set; }

        /// <summary>
        /// Gets or sets the close command.
        /// </summary>
        /// <value>The close command.</value>
        public ICommand CloseCommand { get; private set; }
    }
}
