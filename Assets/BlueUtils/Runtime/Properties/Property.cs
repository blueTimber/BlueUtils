using BlueUtils.Datastructures;
using System;
using System.Linq.Expressions;

namespace DeliverTheMountain.Properties
{
	public class Property<T>
	{
		#region Variables

		private readonly SortedList<int, PropertyOverride> _overrides;

		private T _currentVal;

		private readonly bool _canAdd;
		private readonly Func<T, T, T> _add;
		private readonly bool _canMultiply;
		private readonly Func<T, T, T> _multiply;
		private readonly bool _canCompare;
		private readonly Func<T, T, bool> _greaterThan;

		#endregion

		#region Properties

		public T Base { get; }
		public T Value => _currentVal;

		#endregion

		public Property(T value) 
		{ 
			Base = value;
			_currentVal = Base;
			_overrides = new SortedList<int, PropertyOverride>();

			// It's difficult to check whether addition and multiplication is possible in this version of C#,
			// but I don't want to make multiple version of property
			// Check if adding is possible
			try
			{
				ParameterExpression paramA = Expression.Parameter(typeof(T), "a");
				ParameterExpression paramB = Expression.Parameter(typeof(T), "b");
				BinaryExpression body = Expression.Add(paramA, paramB);
				// Compile a native function pointer
				_add = Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
				_canAdd = true;
			}
			catch
			{
				// If adding failed it's not possible
				_canAdd = false;
			}
			// Check if multiplication is possible
			try
			{
				ParameterExpression paramA = Expression.Parameter(typeof(T), "a");
				ParameterExpression paramB = Expression.Parameter(typeof(T), "b");
				BinaryExpression body = Expression.Multiply(paramA, paramB);
				// Compile a native function pointer
				_multiply = Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
				_canMultiply = true;
			}
			catch
			{
				// If adding failed it's not possible
				_canMultiply = false;
			}
			// Check if comparison is possible
			try
			{
				ParameterExpression paramA = Expression.Parameter(typeof(T), "a");
				ParameterExpression paramB = Expression.Parameter(typeof(T), "b");
				BinaryExpression body = Expression.GreaterThan(paramA, paramB);
				// Compile a native function pointer
				_greaterThan = Expression.Lambda<Func<T, T, bool>>(body, paramA, paramB).Compile();
				_canCompare = true;
			}
			catch
			{
				// If adding failed it's not possible
				_canCompare = false;
			}
		}

		#region Public Methods

		public void Override(OverrideType type, T value, int order, IPropertyOverrider source)
		{
			switch (type)
			{
				case OverrideType.Add:
					if (!_canAdd) return;
					break;
				case OverrideType.Multiply:
					if (!_canMultiply) return;
					break;
				case OverrideType.Min:
				case OverrideType.Max:
					if (!_canCompare) return;
					break;
			}

			_overrides.Add(order, new PropertyOverride()
			{
				Value = value,
				Type = type,
				Order = order,
				Source = source,
			});
			CalculateCurrentValue();
		}

		public void RemoveOverride(IPropertyOverrider source)
		{
			for (int i = 0; i < _overrides.Count; i++)
			{
				if (_overrides[i].Source == source)
				{
					_overrides.RemoveAt(i);
					i--;
				}
			}
			CalculateCurrentValue();
		}

		public void UpdateOverride(IPropertyOverrider source, T value)
		{
			for (int i = 0; i < _overrides.Count; i++)
			{
				if (_overrides[i].Source == source)
				{
					_overrides[i].Value = value;
					return;
				}
			}
			CalculateCurrentValue();
		}

		#endregion

		#region Private Methods

		private void CalculateCurrentValue()
		{
			T result = Base;
			for (int i = 0; i < _overrides.Count; i++) 
			{
				PropertyOverride propertyOverride = _overrides[i];
				switch (propertyOverride.Type)
				{
					case OverrideType.Set:
						result = propertyOverride.Value; 
						break;
					case OverrideType.Add:
						result = _add(result, propertyOverride.Value);
						break;
					case OverrideType.Multiply:
						result = _multiply(result, propertyOverride.Value);
						break;
					case OverrideType.Max:
						if (_greaterThan(propertyOverride.Value, result))
						{
							result = propertyOverride.Value;
						}
						break;
					case OverrideType.Min:
						if (!_greaterThan(propertyOverride.Value, result))
						{
							result = propertyOverride.Value;
						}
						break;
				}
			}
			_currentVal = result;
		}

		#endregion

		#region Nested

		public enum OverrideType
		{
			Set = 1,
			Add = 2,
			Multiply = 3,
			Max = 4,
			Min = 5,
		}

		private class PropertyOverride
		{
			public T Value;
			public OverrideType Type;
			public int Order;
			public IPropertyOverrider Source;
		}

		#endregion
	}
}
