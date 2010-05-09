// ID $Id$

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;

namespace RubyImageCapture
{
    /// <summary>
    /// Klasse, die kontinuierlich Screenshots bezieht
    /// </summary>
    public sealed class ContinuousScreenGrabber : BackgroundWorker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuousScreenGrabber"/> class.
        /// </summary>
        public ContinuousScreenGrabber()
        {
            WorkerSupportsCancellation = true;
            WorkerReportsProgress = false;
        }

        /// <summary>
        /// Die Aufnahmeposition
        /// </summary>
        private Rectangle _capturePosition;
        
        /// <summary>
        /// Die Aufnahmeposition
        /// </summary>
        public Rectangle CapturePosition
        {
            get { return _capturePosition; }
            set { _capturePosition = value; }
        }

        /// <summary>
        /// Das bezogene Bild
        /// </summary>
        public Bitmap CapturedImage { get; private set; }

        /// <summary>
        /// Holt die Aufnahme
        /// </summary>
        /// <remarks>Alias für <seealso cref="CapturedImage"/>.</remarks>
        /// <returns>Ein <see cref="Bitmap"/> mit dem Screenshot</returns>
        public Bitmap GetCapturedImage()
        {
            return CapturedImage;
        }

        /// <summary>
        /// Anzahl der Bilder pro Sekunde
        /// </summary>
        public float FramesPerSecond { get; private set; }

        /// <summary>
        /// Raises the <see cref="E:System.ComponentModel.BackgroundWorker.DoWork"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnDoWork(DoWorkEventArgs e)
        {
            Stopwatch fpsWatch = Stopwatch.StartNew();
            int imageCounter = 0;

            while(!CancellationPending)
            {
                // Frames per Second ermitteln
                long milliseconds = fpsWatch.ElapsedMilliseconds;
                if(fpsWatch.ElapsedMilliseconds > 1000)
                {
                    FramesPerSecond = imageCounter * 1000.0F / milliseconds;
                    imageCounter = 0;
                    fpsWatch.Restart();
                }

                // Aufnahmeposition kopieren
                Rectangle capturePosition = _capturePosition;

                // Testen, ob etwas aufzunehmen ist
                if (capturePosition.Width > 0 && capturePosition.Height > 0)
                {
                    // Bildzähler erhöhen
                    ++imageCounter;

                    // Bitmap holen
                    Bitmap bitmap = CaptureScreen.GetDesktopImage(capturePosition.Left, capturePosition.Top,
                                                                  capturePosition.Width, capturePosition.Height);

                    // Bitmap setzen
                    CapturedImage = bitmap;

                    // Pause machen
                    Thread.Sleep(50);
                }
                else
                {
                    // Wenn nichts aufzunehmen ist - ruhig eine längere Pause machen.
                    Thread.Sleep(250);
                }
            }
        }
    }
}
