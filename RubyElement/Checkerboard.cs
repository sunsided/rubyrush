// ID $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace RubyElement
{
    /// <summary>
    /// Das Spielbrett
    /// </summary>
    public sealed class Checkerboard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Checkerboard"/> class.
        /// </summary>
        /// <param name="elementCountX">The X element count.</param>
        /// <param name="elementCountY">The Y element count.</param>
        public Checkerboard(int elementCountX, int elementCountY)
        {
            ElementCountX = elementCountX;
            ElementCountY = elementCountY;
            _grid = new Element[elementCountX, elementCountY];
            InitializeCountDictionary();
        }

        /// <summary>
        /// Objekt für Threadsynchronisierung
        /// </summary>
        private readonly object _lockObject = new object();

        /// <summary>
        /// Die Anzahl der Elemente in X-Richtung
        /// </summary>
        public readonly int ElementCountX;

        /// <summary>
        /// Die Anzahl der Elemente in Y-Richtung
        /// </summary>
        public readonly int ElementCountY;

        /// <summary>
        /// Die Elemente
        /// </summary>
        private readonly Element[,] _grid;

        /// <summary>
        /// Anzahl der unterschiedlichen Steinfarben, inklusive "unbekannt"
        /// </summary>
        private const int UniqueStoneColors = 8;

        /// <summary>
        /// "Dictionary" mit der Anzahl der Steine
        /// </summary>
        private readonly int[] _countDictionary = new int[UniqueStoneColors];

        /// <summary>
        /// Initialisiert das Anzahl-Array
        /// </summary>
        private void InitializeCountDictionary()
        {
            lock (_lockObject)
            {
                _countDictionary[(int) StoneColor.Unknown] = ElementCountX*ElementCountY;
                _countDictionary[(int) StoneColor.Red] = 0;
                _countDictionary[(int) StoneColor.Green] = 0;
                _countDictionary[(int) StoneColor.Blue] = 0;
                _countDictionary[(int) StoneColor.Yellow] = 0;
                _countDictionary[(int) StoneColor.Orange] = 0;
                _countDictionary[(int) StoneColor.Violet] = 0;
                _countDictionary[(int) StoneColor.White] = 0;
            }
        }

        /// <summary>
        /// Liefert die Anzahl der Steine der gegebenen Farbe auf dem Spielfeld
        /// </summary>
        /// <param name="color">Die Farbe</param>
        /// <returns>Die Anzahl der Steine</returns>
        [Pure]
        public int GetCount(StoneColor color)
        {
            lock (_lockObject) return _countDictionary[(int)color];
        }

        /// <summary>
        /// Gets or sets the <see cref="RubyElement.Element"/> with the specified x.
        /// </summary>
        /// <value></value>
        public Element this[int x, int y]
        {
            [Pure]
            get
            {
                if (x < 0 || x >= ElementCountX) throw new ArgumentOutOfRangeException("x", x, "X index out of range");
                if (y < 0 || y >= ElementCountX) throw new ArgumentOutOfRangeException("y", y, "Y index out of range");
                Contract.EndContractBlock();
                
                return _grid[x, y];
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                if (x < 0 || x >= ElementCountX) throw new ArgumentOutOfRangeException("x", x, "X index out of range");
                if (y < 0 || y >= ElementCountX) throw new ArgumentOutOfRangeException("y", y, "Y index out of range");
                Contract.EndContractBlock();

                Element oldValue = _grid[x, y];
                _grid[x, y] = value;

                // Werte anpassen
                lock (_lockObject)
                {
                    StoneColor oldColor = oldValue == null ? StoneColor.Unknown : oldValue.Color;
                    --_countDictionary[(int) oldColor];
                    ++_countDictionary[(int) value.Color];
                }
            }
        }
    }
}
