using BlueUtils.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlueUtils.Polygons
{
	/// <summary>
	/// TODO: Remove the EdgeChain dependency and improve memory efficiency.
	/// </summary>
	public static partial class PolygonTriangulation
	{
		public static (List<Vector3>, List<int>) SweepingLine(List<EdgeChain> chains, Vector3 p1, Vector3 p2, Vector3 p3)
		{
			foreach (EdgeChain edge in chains)
			{
				if (!edge.Circular)
				{
					throw new Exception("Edges not circular");
				}
			}

			// initialise return lists
			List<Vector3> vertices = new();
			List<int> triangles = new();

			// convert to 2D structure
			ProjectionPlane projector = new(p1, p2, p3);

			List<Vertex> vertexList = ConvertEdgeChain(chains, projector);


			// sweep top to bottom
			List<List<Vertex>> firstSweep = SweepLine(vertexList);

			// sweep bottom to top
			List<List<Vertex>> normalisedPolygons =
				SweepLine(
					firstSweep.SelectMany(x => x)
					.OrderBy(x => x.Pos.y).ToList()
				);



			// normalised triangulation
			List<Vector2> vertices2D = new();
			foreach (List<Vertex> polygon in normalisedPolygons)
			{
				(List<Vector2> v, List<int> t) = NormalisedTriangulation(polygon);

				triangles.AddRange(t.Select(x => x + vertices2D.Count).ToList());

				vertices2D.AddRange(v);
			}

			// convert to 3D
			vertices = vertices2D.Select(x => projector.ToVector3(x)).ToList();

			// return lists
			return (vertices, triangles);
		}

		private static List<Vertex> ConvertEdgeChain(List<EdgeChain> chains, ProjectionPlane projector)
		{
			List<Vertex> vertices = new();

			foreach (EdgeChain chain in chains)
			{
				vertices.Add(new Vertex(projector.ToVector2(chain[0].Item1)));

				Vertex first = vertices.Last();
				Vertex prev = vertices.Last();

				for (int i = 0; i < chain.Count - 1; i++)
				{
					Vertex v = new(projector.ToVector2(chain[i].Item2));

					Vector2 next = projector.ToVector2(chain[i + 1].Item2);
					if (Vector2.Angle(v.Pos - prev.Pos, next - v.Pos) < 0.0001f)
						continue;

					vertices.Add(v);

					new Edge(prev, v);

					prev = v;
				}

				new Edge(prev, first);
			}

			vertices = vertices.OrderByDescending(x => x.Pos.y).ToList();

			return vertices;
		}

		private static List<List<Vertex>> SweepLine(List<Vertex> vertexList)
		{
			List<List<Vertex>> resultChains = new();

			HashSet<Interval> intervals = new();

			HashSet<Vertex> processedVertices = new();

			foreach (Vertex vertex in vertexList)
			{
				processedVertices.Add(vertex);

				Vertex v1 = vertex.Connected[0][vertex];
				Vertex v2 = vertex.Connected[1][vertex];

				if (!processedVertices.Contains(v1) && !processedVertices.Contains(v2))
				{

					// Both edges going down.
					(Edge left, Edge right) = SortEdges(vertex);

					Interval i = InsideOfAnyInterval(intervals, vertex);
					if (i == null)
					{
						// start interval
						Interval interval = new(vertex);

						interval.SetLeft(left);
						interval.SetRight(right);

						intervals.Add(interval);

					}
					else
					{
						// split interval and create edge from last to current
						Interval interval = i.Split(vertex, left, right);
						intervals.Add(interval);
					}
				}
				else if (processedVertices.Contains(v1) && processedVertices.Contains(v2))
				{
					// Both edges going up.
					(Edge left, Edge right) = SortEdges(vertex);

					if (left.Interval == right.Interval)
					{
						// end interval
						resultChains.Add(left.Interval.Close(vertex));

						intervals.Remove(left.Interval);
					}
					else
					{
						// Merge intervals
						left.Interval.MergeRight(right.Interval, vertex);

						intervals.Remove(right.Interval);
					}
				}
				else
				{
					// One edge up, one edge down
					Edge up = vertex.Connected[0];
					Edge down = vertex.Connected[1];
					if (processedVertices.Contains(v2))
					{
						(up, down) = (down, up);
					}

					Interval interval = up.Interval;

					if (interval.Right == up)
					{
						interval.SetRight(down);
						interval.AddRight(vertex);
					}
					else
					{
						interval.SetLeft(down);
						interval.AddLeft(vertex);
					}
				}
			}

			return resultChains;
		}

		private static Interval InsideOfAnyInterval(HashSet<Interval> intervals, Vertex v)
		{
			Interval i = null;

			foreach (Interval interval in intervals)
			{
				if (interval.IsInside(v.Pos))
				{
					i = interval;
					break;
				}
			}

			return i;
		}

		private static (Edge, Edge) SortEdges(Vertex vertex)
		{
			Edge left = vertex.Connected[0];
			Edge right = vertex.Connected[1];
			if (Vector2.Dot(Vector2.left, (left[vertex].Pos - vertex.Pos).normalized)
				< Vector2.Dot(Vector2.left, (right[vertex].Pos - vertex.Pos).normalized))
			{
				(right, left) = (left, right);
			}

			return (left, right);
		}

		private static (List<Vector2>, List<int>) NormalisedTriangulation(List<Vertex> polygon)
		{
			polygon = polygon.OrderByDescending(x => x.Pos.y).ToList();

			List<Vector2> vertices = new();
			List<int> triangles = new();

			Dictionary<Vertex, int> vertexIndex = new();
			for (int i = 0; i < polygon.Count; i++)
			{
				vertices.Add(polygon[i].Pos);
				vertexIndex.Add(polygon[i], i);
			}

			Vertex top = polygon[0];
			(Edge leftEdge, Edge rightEdge) = SortEdges(top);
			(Vertex left, Vertex right) = (leftEdge[top], rightEdge[top]);
			Vertex bottom = polygon[^1];

			HashSet<Vertex> leftChain = new();
			HashSet<Vertex> rightChain = new();

			while (left != bottom)
			{
				leftChain.Add(left);
				if (left.Connected[0][left] == top || leftChain.Contains(left.Connected[0][left]))
				{
					left = left.Connected[1][left];
				}
				else
				{
					left = left.Connected[0][left];
				}
			}

			while (right != bottom)
			{
				rightChain.Add(right);
				if (right.Connected[0][right] == top || rightChain.Contains(right.Connected[0][right]))
				{
					right = right.Connected[1][right];
				}
				else
				{
					right = right.Connected[0][right];
				}
			}

			LinkedList<Vertex> Q = new();
			Q.AddLast(polygon[0]);
			Q.AddLast(polygon[1]);

			for (int i = 2; i < polygon.Count; i++)
			{
				if (leftChain.Contains(polygon[i]) && leftChain.Contains(Q.Last.Value)
					|| rightChain.Contains(polygon[i]) && rightChain.Contains(Q.Last.Value))
				{
					Q.AddLast(polygon[i]);

					while (Q.Count >= 3)
					{
						if (leftChain.Contains(polygon[i]))
						{
							Vector2 outDir = -Vector2.Perpendicular(Q.Last.Value.Pos - Q.ElementAt(Q.Count - 3).Pos);

							if (Vector2.Dot(outDir, Q.ElementAt(Q.Count - 2).Pos - Q.Last.Value.Pos) < 0)
								break;

							triangles.Add(vertexIndex[Q.ElementAt(Q.Count - 2)]);
							triangles.Add(vertexIndex[Q.ElementAt(Q.Count - 3)]);

							triangles.Add(vertexIndex[polygon[i]]);

							Q.Remove(Q.ElementAt(Q.Count - 2));
						}
						else
						{
							Vector2 outDir = Vector2.Perpendicular(Q.Last.Value.Pos - Q.ElementAt(Q.Count - 3).Pos);

							if (Vector2.Dot(outDir, Q.ElementAt(Q.Count - 2).Pos - Q.Last.Value.Pos) < 0)
								break;

							triangles.Add(vertexIndex[Q.ElementAt(Q.Count - 3)]);
							triangles.Add(vertexIndex[Q.ElementAt(Q.Count - 2)]);

							triangles.Add(vertexIndex[polygon[i]]);

							Q.Remove(Q.ElementAt(Q.Count - 2));
						}
					}
				}
				else
				{
					while (Q.Count >= 2)
					{
						if (leftChain.Contains(Q.ElementAt(1)))
						{
							triangles.Add(vertexIndex[Q.ElementAt(1)]);
							triangles.Add(vertexIndex[Q.First.Value]);
						}
						else
						{
							triangles.Add(vertexIndex[Q.First.Value]);
							triangles.Add(vertexIndex[Q.ElementAt(1)]);
						}
						triangles.Add(vertexIndex[polygon[i]]);

						Q.RemoveFirst();
					}

					Q.AddLast(polygon[i]);
				}
			}

			return (vertices, triangles);
		}

		private class Interval
		{
			public Vertex last;

			public Edge Left { get; private set; }
			public Edge Right { get; private set; }

			public List<Vertex> vertices = new();

			private Vertex leftVertex;
			private Vertex rightVertex;

			public Interval(Vertex vertex)
			{
				Vertex v = new(vertex.Pos);
				leftVertex = v;
				rightVertex = v;

				last = v;

				vertices.Add(v);
			}

			public Interval(List<Vertex> vertices, Vertex left, Vertex right, Vertex last)
			{
				this.vertices = vertices;
				leftVertex = left;
				rightVertex = right;

				this.last = last;
			}

			public void MergeRight(Interval other, Vertex vertex)
			{
				Vertex v = new(vertex.Pos);
				vertices.Add(v);

				new Edge(rightVertex, v);
				new Edge(other.leftVertex, v);

				vertices.AddRange(other.vertices);

				rightVertex = other.rightVertex;
				SetRight(other.Right);

				last = v;
			}

			public Interval Split(Vertex vertex, Edge left, Edge right)
			{
				if (last == leftVertex)
				{
					Interval interval = new(last);
					interval.AddRight(vertex);

					interval.SetLeft(Left);
					interval.SetRight(left);

					AddLeft(vertex);
					SetLeft(right);

					return interval;

				}
				else if (last == rightVertex)
				{
					Interval interval = new(last);
					interval.AddLeft(vertex);

					interval.SetRight(Right);
					interval.SetLeft(right);

					AddRight(vertex);
					SetRight(left);

					return interval;
				}
				else
				{
					List<Vertex> rightVertices = new()
				{
					rightVertex
				};
					Vertex nextVertex = rightVertex.Connected[0][rightVertex];
					while (nextVertex != last)
					{
						rightVertices.Add(nextVertex);

						if (nextVertex.Connected[0][nextVertex] == rightVertices[^2])
						{
							nextVertex = nextVertex.Connected[1][nextVertex];
						}
						else
						{
							nextVertex = nextVertex.Connected[0][nextVertex];
						}
					}

					// remove edge
					if (last.Connected[0][last] == rightVertices.Last())
					{
						last.Connected.RemoveAt(0);
					}
					else
					{
						last.Connected.RemoveAt(1);
					}

					if (rightVertices.Last().Connected[0][rightVertices.Last()] == last)
					{
						rightVertices.Last().Connected.RemoveAt(0);
					}
					else
					{
						rightVertices.Last().Connected.RemoveAt(1);
					}

					vertices.RemoveAll(x => rightVertices.Contains(x));

					// create new vertex on right
					Vertex newRight = new(last.Pos);

					new Edge(newRight, rightVertices.Last());
					rightVertices.Add(newRight);

					rightVertex = last;

					Interval interval = new(rightVertices, rightVertices.Last(), rightVertices.First(), rightVertices.Last());

					interval.AddLeft(vertex);
					AddRight(vertex);

					interval.SetLeft(right);
					interval.SetRight(Right);

					SetRight(left);

					return interval;

				}
			}

			public void SetRight(Edge e)
			{
				Right = e;
				e.Interval = this;
			}

			public void SetLeft(Edge e)
			{
				Left = e;
				e.Interval = this;
			}

			public bool IsInside(Vector2 pos)
			{
				float l = Left.GetXForY(pos.y);

				float r = Right.GetXForY(pos.y);

				return pos.x > l && pos.x < r;
			}

			public void AddLeft(Vertex vertex)
			{
				Vertex v = new(vertex.Pos);
				vertices.Add(v);

				new Edge(leftVertex, v);

				leftVertex = v;

				last = v;
			}

			public void AddRight(Vertex vertex)
			{
				Vertex v = new(vertex.Pos);
				vertices.Add(v);

				new Edge(rightVertex, v);

				rightVertex = v;

				last = v;
			}

			public List<Vertex> Close(Vertex vertex)
			{
				Vertex v = new(vertex.Pos);
				vertices.Add(v);

				new Edge(leftVertex, v);
				new Edge(rightVertex, v);

				return vertices;
			}
		}

		private class Vertex
		{
			public Vector2 Pos { get; set; }

			public List<Edge> Connected { get; set; }

			public Vertex(Vector2 position)
			{
				Pos = position;
				Connected = new(2);
			}
		}

		private class Edge
		{
			public (Vertex, Vertex) Connected { get; set; }

			public Interval Interval;

			public Edge(Vertex v1, Vertex v2)
			{
				v1.Connected.Add(this);
				v2.Connected.Add(this);

				Connected = (v1, v2);
			}

			public Vertex this[Vertex v]
			{
				get
				{
					if (Connected.Item1 == v)
						return Connected.Item2;

					if (Connected.Item2 == v)
						return Connected.Item1;

					throw new ArgumentOutOfRangeException();
				}
			}

			public float GetXForY(float y)
			{
				float i = (y - Connected.Item1.Pos.y) / (Connected.Item2.Pos.y - Connected.Item1.Pos.y);
				return i * (Connected.Item2.Pos.x - Connected.Item1.Pos.x) + Connected.Item1.Pos.x;
			}
		}
	}
}
