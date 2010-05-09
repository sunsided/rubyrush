// ID $Id$

using System;
using System.Windows.Forms;
using RubyImageCapture;

namespace Ruby_Rush
{
    static class Program
    {
        /// <summary>
        /// Der verwendete Screengrabber
        /// </summary>
        internal static ContinuousScreenGrabber Grabber = new ContinuousScreenGrabber();

        /// <summary>
        /// Anzahl der Steine in X-Richtung
        /// </summary>
        internal const int ElementCountX = 8;

        /// <summary>
        /// Anzahl der Steine in Y-Richtung
        /// </summary>
        internal const int ElementCountY = 8;


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                // Den Worker starten
                Grabber.RunWorkerAsync();

                // Die Form anzeigen
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Main());
            }
            finally
            {
                // Den Grabber abschießen
                Grabber.CancelAsync();
            }
        }
    }
}
