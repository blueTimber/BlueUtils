#nullable enable
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace BlueUtils.ObjectPooling
{
    public abstract class UnityObjectPool<T> : MonoBehaviour where T : MonoBehaviour
    {
		#region Editor Variables

		[SerializeField][AllowNull] private T _prefab;

		#endregion

		#region Variables

		private readonly Stack<T> _pool = new();

		#endregion

		#region Public Methods

		public T Pop(Transform? parent)
		{
			if (_pool.Count > 0) 
			{
				T obj = _pool.Pop();
				obj.transform.parent = parent;
				obj.enabled = true;
				return obj;
			}
			else
			{
				T obj = Instantiate(_prefab, parent);
				return obj;
			}
		}

		public void Push(T obj)
		{
			obj.transform.parent = null;
			obj.enabled = false;
			_pool.Push(obj);
		}

		#endregion
	}
}
