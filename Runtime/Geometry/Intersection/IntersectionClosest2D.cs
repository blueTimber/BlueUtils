using UnityEngine;

namespace BlueUtils.Geometry
{
	public static partial class Intersection2D
	{
		/// <summary>
		/// Calculate the closest point on a line to a given point in 2D space.
		/// The point is clamped to the line segment.
		/// </summary>
		public static Vector2 ClosestPointOnLine(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
		{
			Vector2 lineDir = (lineEnd - lineStart).normalized;
			float t = Vector2.Dot(point - lineStart, lineDir);
			if (t < 0)
				return lineStart;
			else if (t * t > (lineEnd - lineStart).sqrMagnitude)
				return lineEnd;
			return lineStart + t * lineDir;
		}

		/// <summary>
		/// Calculate the closest points on two lines in 2D space.
		/// Clamped to the line segments.
		/// </summary>
		/// <param name="line1start"></param>
		/// <param name="line1end"></param>
		/// <param name="line2start"></param>
		/// <param name="line2end"></param>
		/// <returns>Returns the closest points on the two lines as a tuple of Vector2. 
		/// If the lines are parallel, returns null.
		/// Item1 is on line1 and Item2 is on line2.</returns>
		public static (Vector2, Vector2)? ClosestPointBetweenLines(Vector2 line1start, Vector2 line1end, Vector2 line2start, Vector2 line2end)
		{
			// Calculate the direction vectors of the lines
			Vector2 line1Dir = (line1end - line1start).normalized;
			Vector2 line2Dir = (line2end - line2start).normalized;
			// Return null if the lines are almost parallel
			float denom = Vector2.Dot(line1Dir, line2Dir);
			if (Mathf.Abs(denom) >= 0.9999f)
				return null;

			// Check if the lines intersect
			Vector2? intersection = LineSegmentLineSegment(line1start, line1end, line2start, line2end);
			if (intersection.HasValue)
				return (intersection.Value, intersection.Value);

			// Calculate the closest points on the lines
			float minSqrDistance = float.MaxValue;
			(Vector2, Vector2) closestPoints = (Vector2.zero, Vector2.zero);

			// Line 2 start
			(Vector2, Vector2) candidate = (ClosestPointOnLine(line1start, line1end, line2start), line2start);
			if ((candidate.Item2 - candidate.Item1).sqrMagnitude < minSqrDistance)
			{
				minSqrDistance = (candidate.Item2 - candidate.Item1).sqrMagnitude;
				closestPoints = candidate;
			}

			// Line 2 end
			candidate = (ClosestPointOnLine(line1start, line1end, line2end), line2end);
			if ((candidate.Item2 - candidate.Item1).sqrMagnitude < minSqrDistance)
			{
				minSqrDistance = (candidate.Item2 - candidate.Item1).sqrMagnitude;
				closestPoints = candidate;
			}

			// Line 1 start
			candidate = (ClosestPointOnLine(line2start, line2end, line1start), line1start);
			if ((candidate.Item2 - candidate.Item1).sqrMagnitude < minSqrDistance)
			{
				minSqrDistance = (candidate.Item2 - candidate.Item1).sqrMagnitude;
				closestPoints = candidate;
			}

			// Line 1 end
			candidate = (ClosestPointOnLine(line2start, line2end, line1end), line1end);
			if ((candidate.Item2 - candidate.Item1).sqrMagnitude < minSqrDistance)
			{
				// minSqrDistance = (candidate.Item2 - candidate.Item1).sqrMagnitude;
				closestPoints = candidate;
			}

			return closestPoints;
		}
	}
}
