using System;
using UnityEngine;

namespace BlueUtils.Geometry
{
	/// <summary>
	/// Helper class for handling intersections.
	/// </summary>
	public static partial class Intersection2D
	{
		/// <summary>
		/// Calculate instersection point between two lines.
		/// </summary>
		/// <param name="r1">Line1 represented by a ray.</param>
		/// <param name="r2">Line2 represented by a ray.</param>
		/// <returns>Point of intersection.</returns>
		/// <exception cref="Exception"></exception>
		public static Vector2? LineLine(Ray2D r1, Ray2D r2)
		{
			float? u = RayRay(r1, r2);
			if (!u.HasValue) return null;
			Vector2 intersectionPoint = r1.GetPoint(u.Value);
			return intersectionPoint;
		}

		/// <summary>
		/// The directions are expected to be normalised.
		/// </summary>
		/// <param name="origin1"></param>
		/// <param name="direction1">Expected to be normalised.</param>
		/// <param name="origin2"></param>
		/// <param name="direction2">Expected to be normalised.</param>
		/// <returns></returns>
		public static Vector2? LineLine(Vector2 origin1, Vector2 direction1, Vector2 origin2, Vector2 direction2)
		{
			float? u = RayRay(origin1, direction1, origin2, direction2);
			if (!u.HasValue) return null;
			Vector2 intersectionPoint = origin1 + u.Value * direction1;
			return intersectionPoint;
		}

		public static float? RayRay(Ray2D ray1, Ray2D ray2)
		{
			return RayRay(ray1.origin, ray1.direction, ray2.origin, ray2.direction);
		}

		/// <summary>
		/// The directions are expected to be normalised.
		/// </summary>
		/// <param name="origin1"></param>
		/// <param name="direction1">Expected to be normalised.</param>
		/// <param name="origin2"></param>
		/// <param name="direction2">Expected to be normalised.</param>
		/// <returns></returns>
		public static float? RayRay(Vector2 origin1, Vector2 direction1, Vector2 origin2, Vector2 direction2)
		{
			Vector2 dirPoint1 = origin2 + direction2;
			Vector2 dirPoint2 = origin1 + direction1;

			float denominator = (origin2.x - dirPoint1.x) * (origin1.y - dirPoint2.y)
				- (origin2.y - dirPoint1.y) * (origin1.x - dirPoint2.x);
			if (Mathf.Abs(denominator) <= 0.0001f)
				return null;

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

		public static float? RayLineSegment(Ray2D line, Vector2 p1, Vector2 p2)
		{
			Vector2 dirPoint = line.origin + line.direction;

			float denominator = (line.origin.x - dirPoint.x) * (p1.y - p2.y) - (line.origin.y - dirPoint.y) * (p1.x - p2.x);
			if (Mathf.Abs(denominator) <= 0.0001f)
				return null;

			float numerator = (line.origin.x - p1.x) * (line.origin.y - dirPoint.y) - (line.origin.y - p1.y) * (line.origin.x - dirPoint.x);

			float u = numerator / denominator;
			if (u < -0.01f || u > 1.01f)
				return null;

			Vector2 intersectionPoint = (p2 - p1) * u + p1;

			float dist = (intersectionPoint - line.origin).magnitude;
			if (Vector2.Dot(intersectionPoint - line.origin, line.direction) < 0.0f)
				dist *= -1;

			return dist;
		}

		public static bool IsPointOnRay(Vector2 point, Ray2D ray, float maxDistance = 0.001f)
		{
			float d = Vector2.Dot(point, ray.direction) - Vector2.Dot(ray.origin, ray.direction);
			Vector2 difference = point - ray.origin - d * ray.direction;
			return difference.sqrMagnitude <= maxDistance * maxDistance;
		}
	}
}