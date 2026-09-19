using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlueUtils.Polygons
{
	/// <summary>
	/// An object for easily matching and joining edges together. 
	/// An edge chain is a collection of points and normals that represent a connected series of edges. 
	/// It can be circular or open-ended.
	/// TODO: I made this as a helper class for a naive destruction algorithm, 
	/// so find out whether it is useful for other purposes, otherwise delete.
	/// </summary>	
	public class EdgeChain
	{
		#region Constants

		private const float Precision = 0.000001f;

		#endregion

		#region Properties

		public List<Vector3> Normals { get; private set; } = new();
		public List<Vector3> Points { get; private set; } = new();
		public bool Circular { get; private set; } = false;

		public int Count { get => Points.Count - 1; }

		public Vector3 FirstPoint => Points.First();
		public Vector3 LastPoint => Points.Last();
		public Vector3 FirstNormal => Normals.First();
		public Vector3 LastNormal => Normals.Last();

		public (Vector3, Vector3, Vector3) this[int index]
		{
			get
			{
				if (index >= Count || index < 0)
					throw new IndexOutOfRangeException();

				return (Points[index], Points[index + 1], Normals[index]);
			}
		}

		#endregion

		public EdgeChain(Vector3 start, Vector3 end, Vector3 normal)
		{
			Normals.Add(normal);

			Points.Add(start);
			Points.Add(end);
		}

		#region Public Methods

		public bool TryAddSingleEdge(Vector3 start, Vector3 end, Vector3 normal)
		{
			return this.TryJoin(new EdgeChain(start, end, normal));
		}

		public bool TryJoin(EdgeChain other)
		{
			if (Circular || other.Circular) return false;

			// If either of the other endpoints match the last point of this chain
			if (AtSamePoint(Points.Last(), other.Points.First()) || AtSamePoint(Points.Last(), other.Points.Last()))
			{
				// Check if we need to reverse the other chain
				bool addReversed = (Points.Last() - other.Points.Last()).sqrMagnitude < (Points.Last() - other.Points.First()).sqrMagnitude;

				Points.RemoveAt(Points.Count - 1);
				if (addReversed)
				{
					Points.AddRange(other.Points.Reverse<Vector3>());
					Normals.AddRange(other.Normals.Reverse<Vector3>());
				}
				else
				{
					Points.AddRange(other.Points);
					Normals.AddRange(other.Normals);
				}

				// Check if circular
				Circular = (AtSamePoint(Points.First(), Points.Last()));

				return true;
			}

			// If either of the other endpoints match the first point of this chain
			if (AtSamePoint(Points.First(), other.Points.Last()) || AtSamePoint(Points.First(), other.Points.First()))
			{
				// Check if we need to reverse the other chain
				bool addReversed = (Points.First() - other.Points.First()).sqrMagnitude < (Points.First() - other.Points.Last()).sqrMagnitude;

				Points.RemoveAt(0);
				if (addReversed)
				{
					Points = other.Points.Reverse<Vector3>().Concat(Points).ToList();
					Normals = other.Normals.Reverse<Vector3>().Concat(Normals).ToList();
				}
				else
				{
					Points = other.Points.Concat(Points).ToList();
					Normals = other.Normals.Concat(Normals).ToList();
				}

				// Check if circular
				Circular = (AtSamePoint(Points.First(), Points.Last()));

				Reverse();

				return true;
			}

			return false;
		}

		public void Reverse()
		{
			Normals.Reverse();
			Points.Reverse();
		}

		public bool Contains(Vector3 a, Vector3 b)
		{
			for (int i = 0; i < Count; i++)
			{
				if ((AtSamePoint(this[i].Item1, a) && AtSamePoint(this[i].Item2, b))
					|| (AtSamePoint(this[i].Item1, b) && AtSamePoint(this[i].Item2, a)))
					return true;
			}
			return false;
		}

		#endregion

		#region Private Methods

		private static bool AtSamePoint(Vector3 a, Vector3 b)
		{
			return (a - b).sqrMagnitude < Precision;
		}

		#endregion
	}
}
