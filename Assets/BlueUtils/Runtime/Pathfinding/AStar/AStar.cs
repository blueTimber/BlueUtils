using BlueUtils.Datastructures;
using System;
using System.Collections.Generic;

namespace BlueUtils.Pathfinding
{
	/// <summary>
	/// A fast and memory efficient implementation of A*.
	/// Not thread safe.
	/// </summary>
	/// <typeparam name="Node"></typeparam>
	public class AStar<Node> where Node : class, IAStarNode<Node>
	{
		#region Variables

		private readonly Func<Node, Node, float> _expectedDist;

		private readonly BinaryHeap<float, int> _queue = new();
		private readonly Dictionary<Node, int> _index = new();
		private readonly List<SearchedNode> _nodes = new();

		#endregion

		public AStar(Func<Node, Node, float> expectedDist)
		{
			_expectedDist = expectedDist;
		}

		#region Public Methods

		public Node[] CalculatePath(Node start, Node end)
		{
			if (start == null || end == null) return null;

			if (start == end) return new Node[] { start };
			
			_queue.Clear();
			_nodes.Clear();
			_index.Clear();

			_queue.Add(0f, 0);
			_nodes.Add(new SearchedNode(start, 0f));
			_index[start] = 0;

			try
			{
				while (!_queue.Empty)
				{
					int currentNodeIndex = _queue.Pop();
					SearchedNode current = _nodes[currentNodeIndex];
					Node currentNode = current.Node;

					if (current.Closed) continue; // stale entry

					// Check if we reached the end
					if (currentNode == end) return current.Collect(_nodes, currentNodeIndex);

					current.Closed = true;
					_nodes[currentNodeIndex] = current;

					// Go over neighbours
					int neighbourCount = currentNode.Neighbours.Count;
					for (int i = 0; i < neighbourCount; i++)
					{
						(Node neighbour, float neighbourDist) = currentNode.Neighbours[i];
						float newDist = current.DistToNode + neighbourDist;

						if (_index.TryGetValue(neighbour, out int existingIndex))
						{
							SearchedNode existing = _nodes[existingIndex];
							if (newDist >= existing.DistToNode) continue;
							existing.DistToNode = newDist;
							existing.ParentIndex = currentNodeIndex;
							existing.Closed = false;
							_nodes[existingIndex] = existing;
							_queue.Add(newDist + _expectedDist(neighbour, end), existingIndex);
						}
						else
						{
							float heuristic = _expectedDist(neighbour, end);
							_nodes.Add(new SearchedNode(
								neighbour,
								newDist,
								currentNodeIndex
							));
							int index = _nodes.Count - 1;
							_index[neighbour] = index;
							_queue.Add(newDist + heuristic, index);
						}
					}
				}

				// End not found, no path possible
				return null;
			}
			finally
			{
				_queue.Clear();
				_nodes.Clear();
				_index.Clear();
			}
		}

		#endregion

		#region Nested 

		private struct SearchedNode
		{
			public Node Node;
			public float DistToNode;
			public int ParentIndex;
			public bool Closed;

			public SearchedNode(Node node, float distToNode, int parentIndex = -1)
			{
				ParentIndex = parentIndex;
				Node = node;
				DistToNode = distToNode;
				Closed = false;
			}

			public readonly Node[] Collect(IReadOnlyList<SearchedNode> nodes, int endIndex)
			{
				int count = 0;
				for (int n = endIndex; n != -1; n = nodes[n].ParentIndex) count++;
				Node[] result = new Node[count];
				for (int n = endIndex; n != -1; n = nodes[n].ParentIndex) result[--count] = nodes[n].Node;
				return result;
			}
		}

		#endregion
	}
}
