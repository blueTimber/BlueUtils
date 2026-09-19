using UnityEngine;

namespace BlueUtils.Geometry
{
	public static partial class Intersection3D
	{
		/// <summary>
		/// Intersection between two planes.
		/// </summary>
		/// <param name="p1">Point instide first plane.</param>
		/// <param name="normal1">Normal of first plane.</param>
		/// <param name="p2">Point inside second plane.</param>
		/// <param name="normal2">Normal of second plane.</param>
		/// <returns>A ray of the intersection line, or null if planes are parallel.</returns>
		public static Ray? PlanePlane(Vector3 p1, Vector3 normal1, Vector3 p2, Vector3 normal2)
		{
			Vector3 resultP;
			Vector3 resultDir;

			resultDir = Vector3.Cross(normal1, normal2);

			Vector3 ldir = Vector3.Cross(normal2, resultDir);

			float denominator = Vector3.Dot(normal1, ldir);

			if (Mathf.Abs(denominator) <= 0.0001f)
				return null;

			Vector3 plane1ToPlane2 = p1 - p2;
			float t = Vector3.Dot(normal1, plane1ToPlane2) / denominator;
			resultP = p2 + t * ldir;

			return new Ray(resultP, resultDir);
		}

		public static float? PlaneRay(Plane plane, Ray ray)
		{
			float denominator = Vector3.Dot(ray.direction, plane.normal);

			if (Mathf.Abs(denominator) <= 0.0001f)
			{
				return null;
			}

			return ((-plane.distance) - Vector3.Dot(ray.origin, plane.normal)) / denominator;
		}
	}
}
