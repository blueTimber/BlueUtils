using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueUtils.Datastructures
{
	[Serializable]
	public class Tup<K, T>
	{
		public K Item1;
		public T Item2;
		public Tup(K item1, T item2)
		{
			Item1 = item1;
			Item2 = item2;
		}
	}

	/// <summary>
	/// A sorted list that doesn't act like a dictionary like the built-in SortedList, 
	/// but instead acts like a list that is sorted by key. It allows duplicate keys if specified.
	/// Insertion and removal are O(n) and retrieval is O(1) by index. It is serializable and can be used in Unity.
	/// It also allows for iteration over the values in the list, but not the keys. 
	/// If you need to iterate over the keys, you can use a foreach loop over the Pairs property and access the key through the Item1 property of each Tup.
	/// </summary>
	/// <typeparam name="K"></typeparam>
	/// <typeparam name="V"></typeparam>
	[Serializable]
	public class SortedList<K,V> : IEnumerable<V>, ISerializationCallbackReceiver
	{
		#region Editor Variables

		[SerializeField] private bool _uniqueKeys;
		[SerializeField] private List<Tup<K, V>> _list;

		#endregion

		#region Variables

		private IComparer<K> _comparer;

		#endregion

		#region Properties

		public V this[int index]
		{
			get
			{
				return _list[index].Item2;
			}
		}

		public int Count => _list.Count;

		public List<Tup<K, V>> Pairs => _list;

		#endregion

		public SortedList(bool uniqueKeys = true)
		{
			_list = new List<Tup<K, V>>();
			_comparer = Comparer<K>.Default;
			_uniqueKeys = uniqueKeys;
		}

		public SortedList(IComparer<K> comparer, bool uniqueKeys = true)
		{
			_list = new List<Tup<K, V>>();
			_comparer = comparer;
			_uniqueKeys = uniqueKeys;
		}

		public SortedList(int capacity, bool uniqueKeys = true)
		{
			_list = new List<Tup<K, V>>(capacity);
			_comparer = Comparer<K>.Default;
			_uniqueKeys = uniqueKeys;
		}

		public SortedList(int capacity, IComparer<K> comparer, bool uniqueKeys = true)
		{
			_list = new List<Tup<K, V>>(capacity);
			_comparer = comparer;
			_uniqueKeys = uniqueKeys;
		}

		#region IEnumerable Implementation

		public IEnumerator<V> GetEnumerator()
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

		/// <summary>
		/// Add a value by key. Fails if no duplate keys is true and the key already exists.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool Add(K key, V value, bool overrideDuplicate = false) 
		{
			for (int i = 0; i < _list.Count; i++)
			{
				int comp = _comparer.Compare(key, _list[i].Item1);
				if (comp < 0)
				{
					_list.Insert(i, new(key, value));
					return true;
				}
				else if (comp == 0)
				{
					if (_uniqueKeys) 
					{ 
						if (overrideDuplicate)
						{
							_list[i] = new(key, value);
						}
						return false;
					} 
					
					_list.Insert(i, new(key, value));
					return true;
				}
			}
			_list.Add(new(key, value));
			return true;
		}

		public bool Remove(V value) 
		{ 
			for (int i = 0; i < _list.Count; i++)
			{
				if (EqualityComparer<V>.Default.Equals(_list[i].Item2, value))
				{
					_list.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		public void RemoveAll(Func<V,bool> predicate)
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

		public V Pop()
		{
			if (_list.Count == 0) throw new ArgumentOutOfRangeException();

			V result = _list[0].Item2;
			RemoveAt(0);

			return result;
		}

		public void Clear()
		{
			_list.Clear();
		}

		#endregion

		#region Private Methods

		private void VerifyIntegrity()
		{
			// Sort the list
			_list.Sort((a, b) =>
			{
				return _comparer.Compare(a.Item1, b.Item1);
			});

			// Check for duplicate keys if uniqueKeys is true
			if (_uniqueKeys)
			{
				for (int i = 1; i < _list.Count; i++)
				{
					if (_comparer.Compare(_list[i - 1].Item1, _list[i].Item1) == 0)
					{
						Debug.LogWarning($"Duplicate key found: {_list[i].Item1}.");
					}
				}
			}
		}

		#endregion

		#region Editor

		private void OnValidate()
		{
			if (_comparer == null)
			{
				_comparer = Comparer<K>.Default;
			}

			VerifyIntegrity();
		}

		public void OnBeforeSerialize() => OnValidate();

		public void OnAfterDeserialize() { }

		#endregion
	}
}
