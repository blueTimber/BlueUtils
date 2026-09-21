using System;
using System.Collections.Generic;

namespace BlueUtils.Datastructures
{
	public class BinaryHeap<P, V>
	{
		#region Variables

		private (P Priority, V Value)[] _items = new (P, V)[16];
		private readonly IComparer<P> _comparer;

		#endregion

		#region Properties

		public int Count { get; private set; } = 0;

		public bool Empty => Count == 0;

		#endregion

		public BinaryHeap(IComparer<P> comparer = null)
		{
			_comparer = comparer ?? Comparer<P>.Default;
		}

		#region Public Methods

		public void Add(P priority, V value)
		{
			if (Count == _items.Length) Grow();

			_items[Count] = (priority, value);
			SiftUp(Count);
			Count++;
		}

		public V Pop()
		{
			if (Empty) throw new InvalidOperationException("Heap is empty.");

			V result = _items[0].Value;
			Count--;

			if (Count > 0)
			{
				_items[0] = _items[Count];
				SiftDown(0);
			}

			// let the popped value be GC'd if it's a reference type
			_items[Count] = default; 
			return result;
		}

		public P PeekPriority()
		{
			if (Empty) throw new InvalidOperationException("Heap is empty.");
			return _items[0].Priority;
		}

		public void Clear()
		{
			Array.Clear(_items, 0, Count);
			Count = 0;
		}

		#endregion

		#region Private Methods

		private void Grow()
		{
			var bigger = new (P, V)[_items.Length * 2];
			Array.Copy(_items, bigger, _items.Length);
			_items = bigger;
		}

		private void SiftUp(int index)
		{
			while (index > 0)
			{
				int parent = (index - 1) / 2;
				if (_comparer.Compare(_items[index].Priority, _items[parent].Priority) >= 0) break;

				Swap(index, parent);
				index = parent;
			}
		}

		private void SiftDown(int index)
		{
			while (true)
			{
				int left = index * 2 + 1;
				int right = left + 1;
				int smallest = index;

				if (left < Count && _comparer.Compare(_items[left].Priority, _items[smallest].Priority) < 0)
					smallest = left;
				if (right < Count && _comparer.Compare(_items[right].Priority, _items[smallest].Priority) < 0)
					smallest = right;

				if (smallest == index) break;

				Swap(index, smallest);
				index = smallest;
			}
		}

		private void Swap(int a, int b)
		{
			(_items[a], _items[b]) = (_items[b], _items[a]);
		}

		#endregion
	}
}
