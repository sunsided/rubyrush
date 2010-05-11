// ID $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using RubyElement;
using RubyImageCapture;
using RubyImageInterpretation;
using RubyLogic;

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
            TopMost = true;

            _rubyRange.Move += RubyRange_Move;
            _rubyRange.SizeChanged += RubyRange_SizeChanged;
            _rubyRange.Shown += RubyRangeShown;

            refreshTimer.Start();

            Program.Grabber.FrameGrabbed += Grabber_FrameGrabbed;
            Program.Evaluator.BoardEvaluated += Evaluator_BoardEvaluated;
        }

        private volatile int oldTopValue = 0;

        private static Random Randomizer = new Random();

        /// <summary>
        /// Handles the BoardEvaluated event of the Evaluator control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Evaluator_BoardEvaluated(object sender, EventArgs e)
        {
            _boardReady = true;
            _movesForDisplay = ((ContinuousBoardEvaluator) sender).Recommendations;
            Interlocked.Exchange(ref _moves, _movesForDisplay);

            if (_stopwatch.ElapsedMilliseconds < 125) return;
            _stopwatch.Restart();

            if (_gridApplied && _moves != null && _moves.Count > 0)
            {
                newMovesAvailable = true;
                ThreadPool.QueueUserWorkItem(delegate
                                                 {
                                                     IList<Recommendation> localMoves = Interlocked.Exchange(ref _moves, null);
                                                     if (localMoves == null || localMoves.Count == 0) return;
                                                     if (moveStillRunning) return;
                                                     Thread.Sleep(15);

                                                     newMovesAvailable = false;
                                                     moveStillRunning = true;
                                                     try
                                                     {
                                                         oldTopValue = localMoves[0].Rank;

                                                         int count = Math.Min(2, localMoves.Count);

                                                         for (int i = 0; i < count; ++i)
                                                         {
                                                             oldTopValue = localMoves[i].Rank;
                                                             Trace.WriteLine("Playing Move " + localMoves[i]);
                                                             PlayMove(localMoves[i]);

                                                             Thread.Sleep(155 + (oldTopValue > 4 ? 10 : 0));
                                                         }
                                                     }
                                                     finally
                                                     {
                                                         Thread.Sleep(15);
                                                         moveStillRunning = false;
                                                     }
                                                 });
            }
        }

        private volatile bool newMovesAvailable = false;

        /// <summary>
        /// Gibt an, dass noch ein Zug gespielt wird
        /// </summary>
        private volatile bool moveStillRunning = false;

        /// <summary>
        /// Die Liste der Bewegungsempfehlungen
        /// </summary>
        private IList<Recommendation> _moves;

        /// <summary>
        /// Die Liste der Bewegungsempfehlungen
        /// </summary>
        private IList<Recommendation> _movesForDisplay;

        private volatile Stopwatch _stopwatch = Stopwatch.StartNew();

        /// <summary>
        /// Handles the FrameGrabbed event of the Grabber control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void Grabber_FrameGrabbed(object sender, EventArgs e)
        {
            ContinuousScreenGrabber grabber = (ContinuousScreenGrabber) sender;

            Bitmap capturedImage = grabber.GetCapturedImage();
            _captureSize = capturedImage.Size;

            // if (moveStillRunning && (Randomizer.Next(0, 1000) < 600)) return;

            if (!BuildMap(capturedImage)) return;
            Program.Evaluator.Board = _board;
        }

        /// <summary>
        /// Die Größe des aufgenommenen Bildes
        /// </summary>
        private Size _captureSize = Size.Empty;

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
            _rawCaptureForScreen = Program.Grabber.GetCapturedImageForScreen();
            if (_rawCapture == null) return false;

            /*
            _rangeBitmap = ImageFilter.FilterBitmap(_rawCapture);
            return _rangeBitmap != null;
             * */
            return true;
        }

        /// <summary>
        /// Das Originalbild
        /// </summary>
        private Bitmap _rawCapture;

        /// <summary>
        /// Das Originalbild
        /// </summary>
        private Bitmap _rawCaptureForScreen;

        /*
        /// <summary>
        /// Das gefilterte Bild
        /// </summary>
        private Bitmap _rangeBitmap;
        */

        /// <summary>
        /// Wird aufgerufen, wenn die Form gezeichnet wird
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Bitmap source = _rawCaptureForScreen;
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

                    const int elementsX = Program.ElementCountX - 1;
                    const int elementsY = Program.ElementCountY - 1;

                    // Nebengitter: Horizontale Linien
                    int width = Math.Abs(_mouseDownLocation.X - _currentMouseLocation.X) / elementsX;
                    for (int i = 0; i < elementsX; ++i)
                    {
                        int x = _mouseDownLocation.X + width*i;
                        gr.DrawLine(_gridPenSmall, x, _mouseDownLocation.Y, x, _currentMouseLocation.Y);
                    }

                    // Nebengitter: Vertikale Linien
                    int height = Math.Abs(_mouseDownLocation.Y - _currentMouseLocation.Y) / elementsY;
                    for (int i = 0; i < elementsY; ++i)
                    {
                        int y = _mouseDownLocation.Y + height*i;
                        gr.DrawLine(_gridPenSmall, _mouseDownLocation.X, y, _currentMouseLocation.X, y);
                    }
                }
                else if (_gridApplied && _boardReady)
                {
                    float top = _topLeftItem.Y*(float) Height/source.Height;
                    float left = _topLeftItem.X*(float) Width/source.Width;

                    float width = Math.Abs(_bottomRightItem.X - _topLeftItem.X) * (float)Width / (source.Width);
                    float height = Math.Abs(_bottomRightItem.Y - _topLeftItem.Y) * (float)Height / (source.Height);

                    float widthSteps = width/7.25F;
                    float heightSteps = height/7.75F;

                    Graphics gr = e.Graphics;
                    gr.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Gray)), ClientRectangle);

                    const int size = 30;
                    const int halfSize = size/2;


                    for (int y = 0; y < Program.ElementCountX; y++)
                    {
                        float ypos = heightSteps*y + top;

                        for (int x = 0; x < Program.ElementCountY; x++)
                        {
                            float xpos = widthSteps*x + left;

                            Color color = _board[x, y].StereotypeColor;
                            Brush brush = new SolidBrush(color);

                            RectangleF rect = new RectangleF(xpos - halfSize, ypos - halfSize, size, size);
                            gr.DrawEllipse(new Pen(Color.White, 2.0F), rect);
                            gr.FillEllipse(brush, rect);
                        }
                    }

                    const int maxSuggestions = 1;

                    // Bewegungsempfehlungen am Start? Zeichnen!
                    IList<Recommendation> moves = _movesForDisplay;
                    if (moves != null && moves.Count > maxSuggestions - 1)
                    {
                        gr.FillRectangle(new SolidBrush(Color.FromArgb(192, Color.Black)), ClientRectangle);

                        for (int i = 0; i < Math.Min(moves.Count, maxSuggestions); ++i)
                        {

                            Recommendation rec1 = moves[i];
                            int xIndex = rec1.Element.ParentXIndex;
                            int yIndex = rec1.Element.ParentYIndex;

                            float ypos = heightSteps * yIndex + top;
                            float xpos = widthSteps * xIndex + left;
                            
                            GraphicsPath path = new GraphicsPath(FillMode.Winding);
                            path.AddEllipse(xpos - 25, ypos - 25, 50, 50);

                            if (rec1.Move == Direction.Right)
                            {
                                path.AddRectangle(new RectangleF(xpos, ypos - 10, 60, 20));
                            }
                            else if (rec1.Move == Direction.Left)
                            {
                                path.AddRectangle(new RectangleF(xpos - 60, ypos - 10, 60, 20));
                            }
                            else if (rec1.Move == Direction.Up)
                            {
                                path.AddRectangle(new RectangleF(xpos - 10, ypos - 60, 20, 60));
                            }
                            else if (rec1.Move == Direction.Down)
                            {
                                path.AddRectangle(new RectangleF(xpos - 10, ypos, 20, 60));
                            }

                            Brush fillBrush = new SolidBrush(Color.FromArgb(192, i == 0 ? Color.White : Color.Red));
                            path.CloseAllFigures();
                            gr.FillPath(fillBrush, path);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Spielt einen Zug nach
        /// </summary>
        /// <param name="move"></param>
        private void PlayMove(Recommendation move)
        {
            User32Point pt = MoveMouse(move, true);
            Thread.Sleep(1);
            PlatformInvokeUser32.mouse_event(PlatformInvokeUser32.MOUSEEVENTF_LEFTDOWN, pt.X, pt.Y, 0, IntPtr.Zero);
            Thread.Sleep(5);

            int deltaX = 0;
            int deltaY = 0;

            if (move.Move == Direction.Right) deltaX = 3;
            else if (move.Move == Direction.Left) deltaX = -3;
            else if (move.Move == Direction.Up) deltaY = -3;
            else if (move.Move == Direction.Down) deltaY = 3;

            const int steps = 10;
            for (int i = 0; i < steps; ++i)
            {
                int x = pt.X + i*deltaX;
                int y = pt.Y + i * deltaY;
                PlatformInvokeUser32.SetCursorPos(x, y);
                PlatformInvokeUser32.mouse_event(PlatformInvokeUser32.MOUSEEVENTF_LEFTDOWN, x, y, 0, IntPtr.Zero);
                Thread.Sleep(3 + ((x^y) % 3 == 0 ? -1 : 0));
            }

            PlatformInvokeUser32.mouse_event(PlatformInvokeUser32.MOUSEEVENTF_LEFTDOWN | PlatformInvokeUser32.MOUSEEVENTF_LEFTUP, pt.X + steps * deltaX, pt.Y + steps * deltaY, 0, IntPtr.Zero);
            Thread.Sleep(5);
        }

        /// <summary>
        /// Bewegt die Maus an eine gegebene Koordinate
        /// </summary>
        /// <param name="move"></param>
        private User32Point MoveMouse(Recommendation move, bool simulationOnly = false)
        {
            float top = _topLeftItem.Y * (float)Height / _captureSize.Height;
            float left = _topLeftItem.X * (float)Width / _captureSize.Width;

            float width = Math.Abs(_bottomRightItem.X - _topLeftItem.X) * (float)Width / (_captureSize.Width);
            float height = Math.Abs(_bottomRightItem.Y - _topLeftItem.Y) * (float)Height / (_captureSize.Height);

            float widthSteps = width / 7.25F;
            float heightSteps = height / 7.75F;

            int xIndex = move.Element.ParentXIndex;
            int yIndex = move.Element.ParentYIndex;

            float localX = widthSteps * xIndex + left;
            float localY = heightSteps * yIndex + top;

            float selectionX = localX*_rubyRange.Width/Width;
            float selectionY = localY * _rubyRange.Height/Height;

            User32Point pt = new User32Point();
            try
            {
                _rubyRange.Invoke((MethodInvoker) delegate
                                                      {
                                                          pt.X = (int) selectionX;
                                                          pt.Y = (int) selectionY;
                                                          PlatformInvokeUser32.ClientToScreen(_rubyRange.Handle, ref pt);
                                                      });
            
                if(!simulationOnly) PlatformInvokeUser32.SetCursorPos(pt.X, pt.Y);
                return pt;
            }
            catch
            {
                return new User32Point() {X = 0, Y = 0};
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
            if(WindowState == FormWindowState.Minimized)
            {
                _gridApplied = false;
                moveStillRunning = false;
            }

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
            if (_rawCaptureForScreen == null) return;

            // Position merken
            _currentMouseLocation = e.Location;
            if (_inDragDrop) Invalidate();

            // Mausposition auf Bildgröße skalieren
            int scaledX = e.X * _captureSize.Width / ClientSize.Width;
            int scaledY = e.Y * _captureSize.Height / ClientSize.Height;

            // Idiotentest
            if (scaledX < 0 || scaledX >= _captureSize.Width) return;
            if (scaledY < 0 || scaledY >= _captureSize.Height) return;

            // Farbe ermitteln
            Color color = _rawCaptureForScreen.GetPixel(scaledX, scaledY);
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
            if (e.Button == MouseButtons.Left)
            {
                _inDragDrop = true;
                _gridApplied = false;
                moveStillRunning = false;
                _mouseDownLocation = e.Location;
                Invalidate();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseClick"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if(e.Button == MouseButtons.Right && _gridApplied)
            {
                Point p = GetIndexFromImageCoordinates(e.Location);

                Element element = _board[p.X, p.Y];

                StringBuilder builder = new StringBuilder();
                builder.AppendLine("Element: " + element ?? "(keines!?)");
                builder.AppendLine();
                builder.AppendLine("Left neighbour: " + element.LeftNeighbour ?? "-");
                builder.AppendLine("Top neighbour: " + element.TopNeighbour ?? "-");
                builder.AppendLine("Right neighbour: " + element.RightNeighbour ?? "-");
                builder.AppendLine("Bottom neighbour: " + element.BottomNeighbour ?? "-");

                MessageBox.Show(this, builder.ToString(), "Element");
            }
        }

        /// <summary>
        /// Ermittelt den Index anhand der Mausposition
        /// </summary>
        /// <param name="imageCoordinates">Die Position im Bild</param>
        /// <returns></returns>
        private Point GetIndexFromImageCoordinates(Point imageCoordinates)
        {
            // Bildschirmkoordinaten zu Bildkoordinaten skalieren
            float scaledX = (float)(imageCoordinates.X - _topLeftItem.X) * _captureSize.Width / (ClientSize.Width - _topLeftItem.X);
            float scaledY = (float)(imageCoordinates.Y - _topLeftItem.Y) * _captureSize.Height / (ClientSize.Height - _topLeftItem.Y);

            // Bildkoordinaten zu Indizes skalieren
            int x = (int)Math.Round(scaledX * (Program.ElementCountX) / _captureSize.Width);
            int y = (int)Math.Round(scaledY * (Program.ElementCountY) / _captureSize.Height);

            // Wertebereich eingrenzen
            x = Math.Max(Math.Min(Program.ElementCountX, x), 0);
            y = Math.Max(Math.Min(Program.ElementCountY, y), 0);
            return new Point(x, y);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseUp"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _mouseUpLocation = e.Location;

            if (_mouseDownLocation == _mouseUpLocation) return;
            if (!_inDragDrop) return;
            _inDragDrop = false;

            // Fragen!
            if(DialogResult.Yes == MessageBox.Show(this, "Raster übernehmen?", "Feldraster", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
            {
                // TODO: na ja, oder andersrum ...
                _topLeftItem = new Point(_mouseDownLocation.X * _captureSize.Width / ClientSize.Width, _mouseDownLocation.Y * _captureSize.Height / ClientSize.Height);
                _bottomRightItem = new Point(_mouseUpLocation.X * _captureSize.Width / ClientSize.Width, _mouseUpLocation.Y * _captureSize.Height / ClientSize.Height);

                _gridApplied = true;
                moveStillRunning = false;
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
        private Checkerboard _board = new Checkerboard(Program.ElementCountX, Program.ElementCountY);

        /// <summary>
        /// Gibt an, ob das Gitter angewandt wurde
        /// </summary>
        private bool _gridApplied;

        /// <summary>
        /// Gibt an, ob das Gitter angewandt wurde
        /// </summary>
        private bool _boardReady;

        /// <summary>
        /// Builds the map.
        /// </summary>
        private bool BuildMap(Bitmap bitmap)
        {
            if (bitmap == null || !_gridApplied) return false;

            int width = Math.Abs(_bottomRightItem.X - _topLeftItem.X);
            int height = Math.Abs(_bottomRightItem.Y - _topLeftItem.Y);

            _board = GridGenerator.GenerateGrid(bitmap, _topLeftItem.X, _topLeftItem.Y, width, height, Program.ElementCountX, Program.ElementCountY);

            // Allet klar.)
            return true;
        }
    }
}
