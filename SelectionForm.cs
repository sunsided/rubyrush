// ID $Id$

using System.Drawing;
using System.Windows.Forms;

namespace Ruby_Rush
{
    public partial class SelectionForm : Form
    {
        public SelectionForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Position des Rahmens
        /// </summary>
        public Point FrameTopLeft { get; private set; }

        /// <summary>
        /// Größe des Rahmens
        /// </summary>
        public Size FrameSize { get; private set; }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Move"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnMove(System.EventArgs e)
        {
            base.OnMove(e);
            FrameTopLeft = sizeBox.PointToScreen(Point.Empty);
            FrameSize = sizeBox.ClientSize;
        }
    }
}
