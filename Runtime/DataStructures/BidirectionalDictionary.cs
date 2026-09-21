using System.Collections.Generic;

namespace BlueUtils.Datastructures
{
	public class BidirectionalDictionary<T1, T2>
	{
		#region Variables

		private Dictionary<T1, T2> _forward = new();
		private Dictionary<T2, T1> _reverse = new();

		#endregion

		#region Properties

		public Indexer<T1, T2> Forward { get; private set; }
		public Indexer<T2, T1> Reverse { get; private set; }

		public int Count => _forward.Count;

		#endregion

		public BidirectionalDictionary()
		{
			this.Forward = new Indexer<T1, T2>(_forward);
			this.Reverse = new Indexer<T2, T1>(_reverse);
		}

		#region Public Methods

		/// <summary>
		/// Add a pair. Will fail if either of the values already exists.
		/// </summary>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		/// <returns></returns>
		public bool Add(T1 t1, T2 t2)
		{
			// Ensure pairs stay bidirectional
			if (_forward.ContainsKey(t1) || _reverse.ContainsKey(t2)) return false;

			_forward.Add(t1, t2);
			_reverse.Add(t2, t1);
			return true;
		}

		/// <summary>
		/// The same as Add, but it will delete any clashing pair.
		/// </summary>
		/// <param name="t1"></param>
		/// <param name="t2"></param>
		public void Set(T1 t1, T2 t2)
		{
			RemoveByFirst(t1);
			RemoveBySecond(t2);
			Add(t1, t2);
		}

		public bool RemoveByFirst(T1 t1)
		{
			if (_forward.TryGetValue(t1, out T2 t2))
			{
				_forward.Remove(t1);
				_reverse.Remove(t2);
				return true;
			}
			return false;
		}

		public bool RemoveBySecond(T2 t2)
		{
			if (_reverse.TryGetValue(t2, out T1 t1))
			{
				_reverse.Remove(t2);
				_forward.Remove(t1);
				return true;
			}
			return false;
		}

		public void Clear()
		{
			_forward.Clear();
			_reverse.Clear();
		}

		public T1[] FirstArray()
		{
			T1[] array = new T1[_forward.Count];
			_forward.Keys.CopyTo(array, 0);
			return array;
		}

		public T2[] SecondArray()
		{
			T2[] array = new T2[_reverse.Count];
			_reverse.Keys.CopyTo(array, 0);
			return array;
		}

		#endregion

		#region Nested 

		public class Indexer<T3, T4>
		{
			private Dictionary<T3, T4> _dictionary;
			public T4 this[T3 index]
			{
				get { return _dictionary[index]; }
			}

			public Indexer(Dictionary<T3, T4> dictionary)
			{
				_dictionary = dictionary;
			}
			
			public bool ContainsKey(T3 key)
			{
				return _dictionary.ContainsKey(key);
			}

			public bool TryGetValue(T3 key, out T4 value)
			{
				return _dictionary.TryGetValue(key, out value);
			}
		}

		#endregion
	}
}
