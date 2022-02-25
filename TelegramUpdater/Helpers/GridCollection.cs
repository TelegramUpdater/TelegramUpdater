using System.Collections;

namespace TelegramUpdater.Helpers
{
    /// <summary>
    /// A grid like collection. useful for building telegram keyboards.
    /// </summary>
    /// <typeparam name="T">Type of element in the grid collection.</typeparam>
    public sealed class GridCollection<T> : ICollection<T>
    {
        private readonly List<List<T>> _grid;

        /// <summary>
        /// Create a new grid collection.
        /// </summary>
        /// <param name="rowCapacity">
        /// Specify the maximum allowed items count for a row.
        /// <para>Leave <see langword="null"/> for no limit.</para>
        /// </param>
        public GridCollection(int? rowCapacity = default)
        {
            RowCapacity = rowCapacity;
            _grid = new() { rowCapacity == null? new(): new(rowCapacity.Value) };
        }

        /// <summary>
        /// Returns the count of all elements in the grid.
        /// </summary>
        public int Count => _grid.Sum(x=> x.Count);

        /// <summary>
        /// Count the rows.
        /// </summary>
        public int RowsCount => _grid.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <summary>
        /// Get maximum item count allowed for a row.
        /// </summary>
        /// <remarks>Returns <see langword="null"/> if there's no limit.</remarks>
        public int? RowCapacity { get; }

        /// <summary>
        /// Get ready to use items of this grid.
        /// </summary>
        public List<List<T>> Items => _grid;

        /// <summary>
        /// Add an item to the end of the last row of the grid.
        /// </summary>
        /// <param name="item">Item to add.</param>
        public void Add(T item)
        {
            var lastRow = _grid[^1];
            // If the last row is full and autoExtend enabled, then we can add a row.
            if (RowCapacity.HasValue)
            {
                if (lastRow.Count >= RowCapacity.Value)
                {
                    AddRow();
                }
            }

            _grid[^1].Add(item);
        }

        /// <summary>
        /// Same as <see cref="Add(T)"/>. but returns this ( <see cref="GridCollection{T}"/> ).
        /// </summary>
        /// <param name="item">Item to add.</param>
        public GridCollection<T> AddItem(T item)
        {
            Add(item);
            return this;
        }

        /// <summary>
        /// Add a new empty row to the grid.
        /// </summary>
        public GridCollection<T> AddRow()
        {
            _grid.Add(RowCapacity == null ? new() : new(RowCapacity.Value));
            return this;
        }

        /// <summary>
        /// Adds a new row and adds an item to that row.
        /// </summary>
        /// <param name="item">Item to be added.</param>
        public GridCollection<T> AddToRow(T item)
        {
            _grid.Add(new() { item });
            return this;
        }

        /// <summary>
        /// Adds an item to the specified row.
        /// </summary>
        /// <remarks><see cref="AutoExtend"/> dose not work on this.</remarks>
        /// <param name="rowIndex">Index of target row.</param>
        /// <param name="item">Item to add.</param>
        public GridCollection<T> AddToRow(int rowIndex, T item)
        {
            _grid[rowIndex].Add(item);
            return this;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _grid.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(T item)
        {
            return _grid.Any(x => x.Any(y => y != null && y.Equals(item)));
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return _grid.SelectMany(x=> x).GetEnumerator();
        }

        /// <summary>
        /// Enumerate over rows.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IEnumerable<T>> EnumerateRows()
        {
            foreach (var l in _grid)
            {
                yield return l;
            }
        }

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            foreach (var l in _grid)
            {
                if (l.Contains(item))
                {
                    return l.Remove(item);
                }
            }

            return false;
        }

        /// <summary>
        /// Remove a row by index.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <returns></returns>
        public void RemoveRow(int rowIndex)
        {
            _grid.RemoveAt(rowIndex);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _grid.SelectMany(x => x).GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        public static implicit operator List<List<T>>(GridCollection<T> grid) => grid.Items;
    }
}
