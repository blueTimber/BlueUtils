#nullable enable
using System.Collections.Generic;

namespace BlueUtils.ObjectPooling
{
	public abstract class ObjectPool<T> where T : class
	{
		#region Variables

		private readonly Stack<T> _pool = new();

		#endregion

		#region Public Methods

		public virtual T Pop()
		{
			if (_pool.Count > 0)
			{
				return _pool.Pop();
			}
			else
			{
				return ConstructObj();
			}
		}

		public virtual void Push(T obj)
		{
			_pool.Push(obj);
		}

		#endregion

		#region Protected Methods

		protected abstract T ConstructObj();

		#endregion
	}
}
