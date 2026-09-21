using System.Collections;
using System.Collections.Generic;

namespace BlueUtils.Datastructures
{
	/// <summary>
	/// A priority queue build with a doubly-linked list. 
	/// If all you want is pushing and popping, and no iteration, try BinaryHeap.
	/// </summary>
	/// <typeparam name="K"></typeparam>
	/// <typeparam name="V"></typeparam>
	public class PriorityQueue<K, V> : IEnumerable<V>
	{
		#region Variables

		private Link _head = null;
		private Link _tail = null;

		private Link _actualTail = null;

		private readonly IComparer<K> _comparer;

		private int _capacity = 0;

		#endregion

		#region Properties

		public int Count { get; private set; } = 0;

		public bool Empty => Count == 0;

		public V Head
		{
			get
			{
				if (_head == null) throw new System.ArgumentOutOfRangeException();
				return _head.Value;
			}
		}

		public V Tail
		{
			get
			{
				if (_tail == null) throw new System.ArgumentOutOfRangeException();
				return _tail.Value;
			}
		}

		#endregion

		public PriorityQueue(IComparer<K> comparer = null)
		{
			_comparer = comparer ?? Comparer<K>.Default;
		}

		#region IEnumerable Implementation

		public IEnumerator<V> GetEnumerator()
		{
			Link current = _head;
			int i = 0;
			while (i < Count)
			{
				yield return current.Value;
				current = current.Next;
				i++;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Public Methods

		public void Add(K key, V value)
		{
			Link previous = null;
			Link current = _head;
			int i = 0;
			while (i < Count)
			{
				int comp = _comparer.Compare(key, current.Key);
				if (comp < 0)
				{
					AddLink(key, value, previous, current);
					return;
				}
				else if (comp == 0)
				{
					// Override current with new value
					current.Value = value;
					return;
				}
				previous = current;
				current = current.Next;
				i++;
			}

			// Add new link to end
			AddLink(key, value, previous, current);
		}

		public V Pop()
		{
			if (Empty) throw new System.IndexOutOfRangeException();

			V result = _head.Value;
			RemoveLink(_head);
			return result;
		}

		public void Clear()
		{
			_head = null;
			_tail = null;
			Count = 0;
		}

		public void Shrink()
		{
			if (_actualTail == null || Count == _capacity) return;

			// Let the unused tail be garbage collected
			if (_tail != null)
			{
				_tail.Next.Previous = null;
				_tail.Next = null;
			}
			_actualTail = _tail;
			_capacity = Count;
		}

		#endregion

		#region Private Methods

		private Link AddLink(K key, V value, Link previous = null, Link next = null)
		{
			Link result;
			Count++;
			// Get new link
			if (Count < _capacity)
			{
				result = _actualTail;
				result.Key = key;
				result.Value = value;
				_actualTail = result.Previous;
			}
			else
			{
				_capacity++;
				result = new Link(key, value);
			}

			// Set connections
			result.Previous = previous;
			result.Next = next;

			// Update connected links
			if (previous != null)
			{
				previous.Next = result;
			}
			if (next != null)
			{
				next.Previous = result;
			}

			// Update head and tail
			if (previous == _tail)
			{
				_tail = result;
			}
			if (next == _head)
			{
				_head = result;
			}
			if (previous == _actualTail)
			{
				_actualTail = result;
			}

			return result;
		}

		private void RemoveLink(Link link)
		{
			if (link == null) return;
			Count--;
			link.Key = default(K);
			link.Value = default(V);

			// Update head and tails
			if (link == _tail)
			{
				_tail = link.Previous;
			}
			if (link == _head)
			{
				_head = link.Next;
			}

			// Update previous and next
			if (link.Previous != null)
			{
				link.Previous.Next = link.Next;
			}
			if (link.Next != null)
			{
				link.Next.Previous = link.Previous;
			}

			// Update actual tail
			link.Next = null;
			link.Previous = _actualTail;
			if (_actualTail != null)
			{
				_actualTail.Next = link;
			}
			_actualTail = link;
		}

		#endregion

		#region Nested

		private class Link
		{
			public K Key;
			public V Value;
			public Link Previous;
			public Link Next;

			public Link(K key, V value, Link previous = null, Link next = null)
			{
				Key = key;
				Value = value;
				Previous = previous;
				Next = next;
			}
		}

		#endregion
	}
}
