// ID $Id$

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using RubyElement;

namespace RubyLogic
{
    /// <summary>
    /// Klasse, die ein gegebenes Spielfeld kontinuierlich auswertet
    /// </summary>
    public class ContinuousBoardEvaluator : BackgroundWorker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuousBoardEvaluator"/> class.
        /// </summary>
        /// <param name="patternList">The pattern list.</param>
        public ContinuousBoardEvaluator(IList<Pattern> patternList)
        {
            PatternList = patternList;
            WorkerSupportsCancellation = true;
        }

        /// <summary>
        /// Setzt oder liefert das Spielbrett
        /// </summary>
        public Checkerboard Board { [Pure] get; set; }

        /// <summary>
        /// Die Empfehlungen zum aktuellen Spielfeld
        /// </summary>
        public IList<Recommendation> Recommendations { [Pure] get; private set; }

        /// <summary>
        /// Die zu verwendenden Muster
        /// </summary>
        public IList<Pattern> PatternList { get; private set; }

        /// <summary>
        /// Die Anzahl der unbekannten Steine, ab denen auf eine Auswertung verzichtet wird
        /// </summary>
        public const int UnknownThreshold = 4;


        /// <summary>
        /// Event, das gerufen wird, wenn ein Spielbrett ausgewertet wurde
        /// </summary>
        public event EventHandler BoardEvaluated;

        /// <summary>
        /// Invokes the frame evaluated event
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void InvokeBoardEvaluated(EventArgs e)
        {
            EventHandler handler = BoardEvaluated;
            if (handler != null) handler.Invoke(this, e);
        }

        /// <summary>
        /// Wertet das Spielfeld aus
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDoWork(DoWorkEventArgs e)
        {
            while(!CancellationPending)
            {
                // Board holen, damit es während der Auswertung nicht modifiziert wird
                Checkerboard board = Board;
                if (board == null || board.GetCount(StoneColor.Unknown) >= UnknownThreshold)
                {
                    Thread.Sleep(100);
                    continue;
                }

                // Die Gesamtzahl der Empfehlungen
                Collection<Recommendation> totalRecommendations = new Collection<Recommendation>();

                // Alle Empfehlungen durchlaufen
                for (int p = 0; p < PatternList.Count; ++p)
                {
                    Pattern pattern = PatternList[p];
                    List<Recommendation> recommendations = new List<Recommendation>();

                    for (int y = 0; y < board.ElementCountY; ++y)
                    {
                        for (int x = 0; x < board.ElementCountX; ++x)
                        {
                            Element element = board[x, y];
                            IEnumerable<Recommendation> recs = pattern.Evaluate(element);
                            recommendations.AddRange(recs);
                        }
                    }

                    // Empfehlungen in die Liste einreihen
                    for (int i = 0; i < recommendations.Count; ++i)
                    {
                        Recommendation rec = recommendations[i];

                        // Wenn ein ähnliches Element bereits in der Liste ist, nur das Element mit höherer Punktzahl behalten
                        int index = totalRecommendations.IndexOf(rec);
                        if (index >= 0)
                        {
                            if (totalRecommendations[index].Rank < rec.Rank)
                            {
                                totalRecommendations[index] = rec;
                            }
                        }
                        else
                        {
                            totalRecommendations.Add(rec);
                        }
                    }
                }

                // Empfehlungen ausdünnen
                // TODO: Empfehlungen, die mehrere Steine betreffen zusammenfassen und die Ränge addieren
                List<Recommendation> unoptimizedRecommendations = totalRecommendations.ToList();

                // Identische Elemente zusammenfassen)
                for (int i = 0; i < unoptimizedRecommendations.Count - 1; ++i)
                {
                    Recommendation r = unoptimizedRecommendations[i];
                    for (int j = i + 1; j < unoptimizedRecommendations.Count; ++j)
                    {
                        Recommendation r2 = unoptimizedRecommendations[j];
                        if(r2.IsSameMove(r))
                        {
                            // Rang addieren
                            r.SetRank(r.Rank + r2.Rank);

                            // Zweites Element entfernen
                            unoptimizedRecommendations.RemoveAt(j);

                            // Zähler reduzieren
                            --i;
                            break;
                        }
                    }
                }

                // Nach Punktzahl sortieren
                unoptimizedRecommendations = unoptimizedRecommendations.OrderBy(a => a.Rank).Reverse().ToList();

                // Elemente entfernen, die auf denselben Stein verweisen (der Erste - und damit auch der mit der höchsten Punktzahl - gewinnt)
                for (int i = 0; i < unoptimizedRecommendations.Count - 1; ++i)
                {
                    Recommendation r = unoptimizedRecommendations[i];
                    for (int j = i + 1; j < unoptimizedRecommendations.Count; ++j)
                    {
                        Recommendation r2 = unoptimizedRecommendations[j];
                        if (r2.Element.ParentXIndex == r.Element.ParentXIndex && r2.Element.ParentYIndex == r.Element.ParentYIndex)
                        {
                            // Zweites Element entfernen
                            unoptimizedRecommendations.RemoveAt(j);

                            // Zähler reduzieren
                            --i;
                            break;
                        }
                    }
                }

                // Nach Punktzahl sortieren
                Recommendations = unoptimizedRecommendations.ToList();

                InvokeBoardEvaluated(EventArgs.Empty);
                Thread.Sleep(50);
            }
        }
    }
}
