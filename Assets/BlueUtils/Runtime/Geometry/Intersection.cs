using System;
using UnityEngine;

namespace BlueUtils.Geometry
{
	/// <summary>
	/// Helper class for handling intersections.
	/// </summary>
	public static class Intersection
	{
		/// <summary>
		/// Calculate instersection point between two lines.
		/// </summary>
		/// <param name="r1">Line1 represented by a ray.</param>
		/// <param name="r2">Line2 represented by a ray.</param>
		/// <returns>Point of intersection.</returns>
		/// <exception cref="Exception"></exception>
		public static Vector2 LineLine2D(Ray2D r1, Ray2D r2)
		{
			float u = RayRay2D(r1, r2);

			Vector2 intersectionPoint = r1.GetPoint(u);

			return intersectionPoint;
		}

		public static Vector2 LineLine2D(Vector2 origin1, Vector2 direction1, Vector2 origin2, Vector2 direction2)
		{
			float u = RayRay2D(origin1, direction1, origin2, direction2);
			Vector2 intersectionPoint = origin1 + u * direction1.normalized;
			return intersectionPoint;
		}

		public static float RayRay2D(Ray2D ray1, Ray2D ray2)
		{
			return RayRay2D(ray1.origin, ray1.direction, ray2.origin, ray2.direction);
		}

		public static float RayRay2D(Vector2 origin1, Vector2 direction1, Vector2 origin2, Vector2 direction2)
		{
			Vector2 dirPoint1 = origin2 + direction2;
			Vector2 dirPoint2 = origin1 + direction1;

			float denominator = (origin2.x - dirPoint1.x) * (origin1.y - dirPoint2.y)
				- (origin2.y - dirPoint1.y) * (origin1.x - dirPoint2.x);
			if (Mathf.Abs(denominator) <= 0.0001f)
				throw new InvalidOperationException();

			float numerator = (origin2.x - origin1.x) * (origin2.y - dirPoint1.y) - (origin2.y - origin1.y) * (origin2.x - dirPoint1.x);

			float u = numerator / denominator;

			return u;
		}

		public static Vector2? LineSegmentLineSegment(Vector2 l1start, Vector2 l1end, Vector2 l2start, Vector2 l2end)
		{
			float den = ((l1start.x - l1end.x) * (l2start.y - l2end.y) - (l1start.y - l1end.y) * (l2start.x - l2end.x));
			if (den == 0.0f) return null;

			float t = ((l1start.x - l2start.x) * (l2start.y - l2end.y) - (l1start.y - l2start.y) * (l2start.x - l2end.x)) / den;

			float u = ((l1start.x - l2start.x) * (l1start.y - l1end.y) - (l1start.y - l2start.y) * (l1start.x - l1end.x)) / den;

			if (t < 0.0f || t > 1.0f || u < 0.0f || u > 1.0f) return null;

			return l1start + (l1end - l1start) * t;
		}

		public static bool IsPointOnRay2D(Vector2 point, Ray2D ray, float maxDistance = 0.001f)
		{
			float d = Vector2.Dot(point, ray.direction) - Vector2.Dot(ray.origin, ray.direction);
			Vector2 difference = point - ray.origin - d * ray.direction;
			return difference.sqrMagnitude <= maxDistance * maxDistance;
		}

		/// <summary>
		/// Calculate the closest point on a line to a given point in 2D space.
		/// The point is clamped to the line segment.
		/// </summary>
		public static Vector2 ClosestPointOnLine2D(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
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
		/// <returns>Returns the closest points on the two lines as a tuple of Vector2. If the lines are parallel, returns null.</returns>
		public static (Vector2, Vector2)? ClosestPointBetweenLines2D(Vector2 line1start, Vector2 line1end, Vector2 line2start, Vector2 line2end)
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
			(Vector2, Vector2) candidate = (ClosestPointOnLine2D(line1start, line1end, line2start), line2start);
			if ((candidate.Item2 - candidate.Item1).sqrMagnitude < minSqrDistance)
			{
				minSqrDistance = (candidate.Item2 - candidate.Item1).sqrMagnitude;
				closestPoints = candidate;
			}

			// Line 2 end
			candidate = (ClosestPointOnLine2D(line1start, line1end, line2end), line2end);
			if ((candidate.Item2 - candidate.Item1).sqrMagnitude < minSqrDistance)
			{
				minSqrDistance = (candidate.Item2 - candidate.Item1).sqrMagnitude;
				closestPoints = candidate;
			}

			// Line 1 start
			candidate = (ClosestPointOnLine2D(line2start, line2end, line1start), line1start);
			if ((candidate.Item2 - candidate.Item1).sqrMagnitude < minSqrDistance)
			{
				minSqrDistance = (candidate.Item2 - candidate.Item1).sqrMagnitude;
				closestPoints = candidate;
			}

			// Line 1 end
			candidate = (ClosestPointOnLine2D(line2start, line2end, line1end), line1end);
			if ((candidate.Item2 - candidate.Item1).sqrMagnitude < minSqrDistance)
			{
				// minSqrDistance = (candidate.Item2 - candidate.Item1).sqrMagnitude;
				closestPoints = candidate;
			}

			return closestPoints;
		}
	}
}