// ID $Id$

using System.Drawing;
using System.Windows.Forms;

namespace Ruby_Rush
{
    public partial class Main : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Main"/> class.
        /// </summary>
        public Main()
        {
            InitializeComponent();
            SetStyle(ControlStyles.Opaque | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            _rubyRange.Move += RubyRange_Move;
            _rubyRange.SizeChanged += RubyRange_SizeChanged;
            _rubyRange.Shown += RubyRangeShown;

            refreshTimer.Start();
        }

        /// <summary>
        /// Handles the Shown event of the _rubyRange control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void RubyRangeShown(object sender, System.EventArgs e)
        {
            StartActionCascade();
        }

        /// <summary>
        /// Handles the SizeChanged event of the RubyRange control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void RubyRange_SizeChanged(object sender, System.EventArgs e)
        {
            StartActionCascade();
        }

        /// <summary>
        /// Handles the Move event of the RubyRange control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void RubyRange_Move(object sender, System.EventArgs e)
        {
            StartActionCascade();
        }

        /// <summary>
        /// Das Positions-Auswahlfenster
        /// </summary>
        readonly SelectionForm _rubyRange = new SelectionForm();

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            _rubyRange.Show(this);
        }

        /// <summary>
        /// Tut Dinge.
        /// </summary>
        private void StartActionCascade()
        {
            GetRangeBitmap();
            Invalidate();
        }

        /// <summary>
        /// Holt die Range-Bitmap
        /// </summary>
        private void GetRangeBitmap()
        {
            Point topLeft = _rubyRange.FrameTopLeft;
            Size size = _rubyRange.FrameSize;

            int left = topLeft.X;
            int top = topLeft.Y;
            int width = size.Width;
            int height = size.Height;

            Bitmap capture = CaptureScreen.GetDesktopImage(left, top, width, height);
            _rangeBitmap = ImageFilter.FilterBitmap(capture);
        }

        /// <summary>
        /// Das Range-Bitmap
        /// </summary>
        private Bitmap _rangeBitmap;

        /// <summary>
        /// Wird aufgerufen, wenn die Form gezeichnet wird
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_rangeBitmap == null) return;
            e.Graphics.DrawImage(_rangeBitmap, ClientRectangle);
        }

        /// <summary>
        /// Wird aufgerufen, wenn sich die Größe der Form ändert
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        /// <summary>
        /// Handles the Tick event of the refreshTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void RefreshTimerTick(object sender, System.EventArgs e)
        {
            StartActionCascade();
        }
    }
}
