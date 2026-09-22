using UnityEngine;

namespace BlueUtils.Geometry
{
	public static partial class Intersection3D
	{
		/// <summary>
		/// Calculate the closest point on a line to a given point in 3D space.
		/// The point is clamped to the line segment.
		/// </summary>
		public static Vector3 ClosestPointOnLineSegment(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
		{
			Vector3 line = lineEnd - lineStart;
			float lineSqrMag = line.sqrMagnitude;
			if (lineSqrMag < Mathf.Epsilon)
				return lineStart; 

			float t = Vector3.Dot(point - lineStart, line) / lineSqrMag;
			t = Mathf.Clamp01(t);
			return lineStart + t * line;
		}
	}
}
