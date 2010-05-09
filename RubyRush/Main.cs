// ID $Id$

using System;
using System.Drawing;
using System.Windows.Forms;
using RubyElement;
using RubyImageInterpretation;

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
        void RubyRangeShown(object sender, EventArgs e)
        {
            SetNewCaptureBounds();
        }

        /// <summary>
        /// Handles the SizeChanged event of the RubyRange control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void RubyRange_SizeChanged(object sender, EventArgs e)
        {
            SetNewCaptureBounds();
        }

        /// <summary>
        /// Handles the Move event of the RubyRange control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void RubyRange_Move(object sender, EventArgs e)
        {
            SetNewCaptureBounds();
        }

        /// <summary>
        /// Das Positions-Auswahlfenster
        /// </summary>
        readonly SelectionForm _rubyRange = new SelectionForm();

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _rubyRange.Show(this);
        }

        /// <summary>
        /// Bild beziehen und verarbeiten
        /// </summary>
        private void StartActionCascade()
        {
            if (!GetRangeBitmap()) return;
            if (!BuildMap()) return;
            Invalidate();
        }

        /// <summary>
        /// Setzt den neuen Aufnahmebereich
        /// </summary>
        private void SetNewCaptureBounds()
        {
            Point topLeft = _rubyRange.FrameTopLeft;
            Size size = _rubyRange.FrameSize;

            Program.Grabber.CapturePosition = new Rectangle(topLeft, size);
        }

        /// <summary>
        /// Holt die Range-Bitmap
        /// </summary>
        private bool GetRangeBitmap()
        {
            _rawCapture = Program.Grabber.GetCapturedImage();
            if (_rawCapture == null) return false;

            _rangeBitmap = ImageFilter.FilterBitmap(_rawCapture);
            return _rangeBitmap != null;
        }

        /// <summary>
        /// Das Originalbild
        /// </summary>
        private Bitmap _rawCapture;

        /// <summary>
        /// Das gefilterte Bild
        /// </summary>
        private Bitmap _rangeBitmap;

        /// <summary>
        /// Wird aufgerufen, wenn die Form gezeichnet wird
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Bitmap source = _rawCapture;
            if (source == null) return;
            lock (source)
            {
                e.Graphics.DrawImage(source, ClientRectangle);

                if (_inDragDrop)
                {
                    Graphics gr = e.Graphics;

                    // Hauptgitter
                    gr.DrawLine(_gridPen, _mouseDownLocation.X, _mouseDownLocation.Y, _currentMouseLocation.X,
                                _mouseDownLocation.Y);
                    gr.DrawLine(_gridPen, _mouseDownLocation.X, _mouseDownLocation.Y, _mouseDownLocation.X,
                                _currentMouseLocation.Y);
                    gr.DrawLine(_gridPen, _currentMouseLocation.X, _mouseDownLocation.Y, _currentMouseLocation.X,
                                _currentMouseLocation.Y);
                    gr.DrawLine(_gridPen, _mouseDownLocation.X, _currentMouseLocation.Y, _currentMouseLocation.X,
                                _currentMouseLocation.Y);

                    const int elements = 7;

                    // Nebengitter: Horizontale Linien
                    int width = Math.Abs(_mouseDownLocation.X - _currentMouseLocation.X)/elements;
                    for (int i = 0; i < elements; ++i)
                    {
                        int x = _mouseDownLocation.X + width*i;
                        gr.DrawLine(_gridPenSmall, x, _mouseDownLocation.Y, x, _currentMouseLocation.Y);
                    }

                    // Nebengitter: Vertikale Linien
                    int height = Math.Abs(_mouseDownLocation.Y - _currentMouseLocation.Y)/elements;
                    for (int i = 0; i < elements; ++i)
                    {
                        int y = _mouseDownLocation.Y + height*i;
                        gr.DrawLine(_gridPenSmall, _mouseDownLocation.X, y, _currentMouseLocation.X, y);
                    }
                }
                else if (_gridApplied)
                {
                    float top = _topLeftItem.Y*(float) Height/_rawCapture.Height;
                    float left = _topLeftItem.X*(float) Width/_rawCapture.Width;

                    float width = Math.Abs(_bottomRightItem.X - _topLeftItem.X) * (float)Width / (source.Width);
                    float height = Math.Abs(_bottomRightItem.Y - _topLeftItem.Y) * (float)Height / (source.Height);

                    float widthSteps = width/7.25F;
                    float heightSteps = height/7.75F;

                    Graphics gr = e.Graphics;
                    gr.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Gray)), ClientRectangle);

                    const int size = 30;
                    const int halfSize = size/2;


                    for (int y = 0; y < 8; y++)
                    {
                        float ypos = heightSteps*y + top;

                        for (int x = 0; x < 8; x++)
                        {
                            float xpos = widthSteps*x + left;

                            Color color = _colorMap[x, y].StereotypeColor;
                            Brush brush = new SolidBrush(color);

                            RectangleF rect = new RectangleF(xpos - halfSize, ypos - halfSize, size, size);
                            gr.DrawEllipse(new Pen(Color.White, 2.0F), rect);
                            gr.FillEllipse(brush, rect);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Pen für das Hauptgitter
        /// </summary>
        private readonly Pen _gridPen = new Pen(Color.Red, 4.0f);

        /// <summary>
        /// Pen für das Hauptgitter
        /// </summary>
        private readonly Pen _gridPenSmall = new Pen(Color.Red, 2.0f);

        /// <summary>
        /// Wird aufgerufen, wenn sich die Größe der Form ändert
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        /// <summary>
        /// Handles the Tick event of the refreshTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void RefreshTimerTick(object sender, EventArgs e)
        {
            Text = string.Format("Ruby Rush, Capture FPS: {0:0.00}", Program.Grabber.FramesPerSecond);
            StartActionCascade();
        }

        /// <summary>
        /// Die aktuelle Mausposition
        /// </summary>
        private Point _currentMouseLocation;

        /// <summary>
        /// Die Position, an der die Maus gedrückt wurde
        /// </summary>
        private Point _mouseDownLocation;

        /// <summary>
        /// Die Position, an der die Maus losgelassen wurde
        /// </summary>
        private Point _mouseUpLocation;

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseMove"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Kein Bild? --> Ende der Fahrt.
            if (_rangeBitmap == null) return;

            // Position merken
            _currentMouseLocation = e.Location;
            if (_inDragDrop) Invalidate();

            // Mausposition auf Bildgröße skalieren
            int scaledX = e.X*_rangeBitmap.Width/ClientSize.Width;
            int scaledY = e.Y*_rangeBitmap.Height/ClientSize.Height;

            // Idiotentest
            if (scaledX < 0 || scaledX >= _rangeBitmap.Width) return;
            if (scaledY < 0 || scaledY >= _rangeBitmap.Height) return;

            // Farbe ermitteln
            Color color = _rawCapture.GetPixel(scaledX, scaledY);
            int lightness = (color.R + color.G + color.B)/3;

            // Farbe ausgeben
            //pictureBoxColor.BackColor = color;

            int rPercent = 100*color.R/255;
            int gPercent = 100*color.G/255;
            int bPercent = 100*color.B/255;

            labelColor.Text = String.Format("R: {0,-3}/{1,-2}% G: {2,-3}/{3,-2}% B: {4,-3}/{5,-2}%, Total: {6,-3}", color.R, rPercent, color.G, gPercent, color.B, bPercent, lightness);
        }

        /// <summary>
        /// Gibt an, ob eine Drag&Drop-Operation gestartet wurde
        /// </summary>
        private bool _inDragDrop;

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseDown"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _inDragDrop = true;
            _gridApplied = false;
            _mouseDownLocation = e.Location;
            Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseUp"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _inDragDrop = false;
            _mouseUpLocation = e.Location;

            if (_mouseDownLocation == _mouseUpLocation) return;

            // Fragen!
            if(DialogResult.Yes == MessageBox.Show(this, "Raster übernehmen?", "Feldraster", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
            {
                // TODO: na ja, oder andersrum ...
                _topLeftItem = new Point(_mouseDownLocation.X * _rangeBitmap.Width / ClientSize.Width, _mouseDownLocation.Y * _rangeBitmap.Height / ClientSize.Height);
                _bottomRightItem = new Point(_mouseUpLocation.X * _rangeBitmap.Width / ClientSize.Width, _mouseUpLocation.Y * _rangeBitmap.Height / ClientSize.Height);

                _gridApplied = true;
                BuildMap();
            }
        }

        /// <summary>
        /// Obere linke Ecke des ersten Elementes
        /// </summary>
        private Point _topLeftItem;

        /// <summary>
        /// Untere, rechte Ecke des Elementes
        /// </summary>
        private Point _bottomRightItem;

        /*
        /// <summary>
        /// Die Steinkarte
        /// </summary>
        private readonly StoneColor[,] _stoneMap = new Stone[8,8];
        */

        /// <summary>
        /// Die Steinkarte (in Farbe)
        /// </summary>
        private Element[,] _colorMap = new Element[8, 8];

        /// <summary>
        /// Gibt an, ob das Gitter angewandt wurde
        /// </summary>
        private bool _gridApplied;

        /// <summary>
        /// Builds the map.
        /// </summary>
        private bool BuildMap()
        {
            if (_rawCapture == null) return false;

            int width = Math.Abs(_bottomRightItem.X - _topLeftItem.X);
            int height = Math.Abs(_bottomRightItem.Y - _topLeftItem.Y);

            _colorMap = GridGenerator.GenerateGrid(_rawCapture, _topLeftItem.X, _topLeftItem.Y, width, height, 8, 8);

            // Allet klar.)
            return true;
        }
    }
}
