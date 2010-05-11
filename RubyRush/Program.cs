// ID $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using RubyImageCapture;
using RubyImageInterpretation;
using RubyLogic;
using RubyLogic.PatternDefinition;

namespace Ruby_Rush
{
    static class Program
    {
        /// <summary>
        /// Der verwendete Screengrabber
        /// </summary>
        internal static readonly ContinuousScreenGrabber Grabber = new ContinuousScreenGrabber();

        /// <summary>
        /// Der Evaluator
        /// </summary>
        internal static ContinuousBoardEvaluator Evaluator { get; private set; }

        /// <summary>
        /// Anzahl der Steine in X-Richtung
        /// </summary>
        internal const int ElementCountX = 8;

        /// <summary>
        /// Anzahl der Steine in Y-Richtung
        /// </summary>
        internal const int ElementCountY = 8;

        /// <summary>
        /// Der Default-Name der Musterdatei
        /// </summary>
        internal const string DefaultPatternFile = "PatternDefinitions.xml";

        /// <summary>
        /// Die Liste der Muster
        /// </summary>
        internal static IList<Pattern> Pattern { get; private set; }

            /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                // Muster laden
                try
                {
                    if (File.Exists(DefaultPatternFile))
                    {
                        Pattern = PatternImport.LoadFromXml(new FileInfo(DefaultPatternFile));
                    }
                }
                catch(Exception ex)
                {
                    // Fehler anzeigen und Prozess ggf. beenden
                    if(DialogResult.Cancel == MessageBox.Show(
                        "Ein Ausnahmefehler ist beim Laden der Musterdatei aufgetreten:" + Environment.NewLine + ex.Message, 
                        "Laden der Musterdatei", MessageBoxButtons.OKCancel, MessageBoxIcon.Error))
                    {
                        return;
                    }
                }

                // Interne Muster laden, falls XML-Datei nicht gefunden wurde
                if(Pattern == null || Pattern.Count == 0)
                {
                    Pattern = PatternDefinitionAttribute.GetPatternDefinitions();
                }

                // Hinweis ausgeben
                Trace.WriteLine(Pattern.Count + " Muster geladen.");
                Evaluator = new ContinuousBoardEvaluator(Pattern);

                // Worker starten
                Evaluator.RunWorkerAsync();
                Grabber.RunWorkerAsync();

                // Die Form anzeigen
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Main());
            }
            finally
            {
                // Die Worker abschießen
                Evaluator.CancelAsync();
                Grabber.CancelAsync();
            }
        }
    }
}
