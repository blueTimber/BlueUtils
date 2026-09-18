using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueUtils.Datastructures
{
	public class SortedList<K,T> : IEnumerable<T>
	{
		#region Variables

		private List<(K, T)> _list;
		private IComparer<K> _comparer;
		private readonly bool _uniqueKeys;

		#endregion

		#region Properties

		public T this[int index]
		{
			get
			{
				return _list[index].Item2;
			}
		}

		public int Count => _list.Count;

		#endregion

		public SortedList(bool uniqueKeys = true)
		{
			_list = new List<(K, T)>();
			_uniqueKeys = uniqueKeys;
		}

		public SortedList(IComparer<K> comparer, bool uniqueKeys = true)
		{
			_list = new List<(K, T)>();
			_comparer = comparer;
			_uniqueKeys = uniqueKeys;
		}

		public SortedList(int capacity, bool uniqueKeys = true)
		{
			_list = new List<(K, T)>(capacity);
			_uniqueKeys = uniqueKeys;
		}

		public SortedList(int capacity, IComparer<K> comparer, bool uniqueKeys = true)
		{
			_list = new List<(K, T)>(capacity);
			_comparer = comparer;
			_uniqueKeys = uniqueKeys;
		}

		#region IEnumerable Implementation

		public IEnumerator<T> GetEnumerator()
		{
			for (int i = 0; i < _list.Count; i++)
			{
				yield return _list[i].Item2;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Public Methods

		public void Add(K key, T value) 
		{
			IComparer<K> comparer = _comparer;
			comparer ??= Comparer<K>.Default;
			for (int i = 0; i < _list.Count; i++)
			{
				int comp = comparer.Compare(key, _list[i].Item1);
				if (comp < 0)
				{
					_list.Insert(i, (key, value));
					return;
				}
				else if (comp == 0)
				{
					if (_uniqueKeys)
					{
						Debug.LogWarning("Key has already been inserted!");
					}
					_list.Insert(i, (key, value));
					return;
				}
			}
			_list.Add((key, value));
		}

		public bool Remove(T value) 
		{ 
			for (int i = 0; i < _list.Count; i++)
			{
				if (EqualityComparer<T>.Default.Equals(_list[i].Item2, value))
				{
					_list.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		public void RemoveAll(Func<T,bool> predicate)
		{
			for (int i = 0; i < _list.Count; i++)
			{
				if (predicate(_list[i].Item2))
				{
					_list.RemoveAt(i);
					i--;
				}
			}
		}

		public void RemoveAt(int index)
		{
			_list.RemoveAt(index);
		}

		#endregion
	}
}
