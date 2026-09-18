using System;
using UnityEngine;

namespace BlueUtils.Serialisation
{
	/// <summary>
	/// A serialised value that can only be set once.
	/// </summary>
	/// <typeparam name="T">Needs to be serializable, else it won't work.</typeparam>
	[Serializable]
	public struct StaticValue<T>
	{
		#region Serialised Variables

		[SerializeField, HideInInspector] private T _value;

		[SerializeField, HideInInspector] private bool _isStored;

		#endregion

		#region Properties

		public T Value
		{
			readonly get => _value;
			set
			{
				if (_isStored)
				{
					return;
				}

				_value = value;
				_isStored = true;
			}
		}

		#endregion
	}
}
