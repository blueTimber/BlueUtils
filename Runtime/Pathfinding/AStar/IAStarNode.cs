using System.Collections.Generic;

namespace BlueUtils.Pathfinding
{
	public interface IAStarNode<T> where T : IAStarNode<T>
	{
		#region Properties

		public IReadOnlyList<(T, float)> Neighbours { get; }

		#endregion
	}
}
