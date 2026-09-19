using System.Collections.Generic;
using UnityEngine;

namespace BlueUtils.Geometry
{
	public static class UnitBox
	{
		public static (Vector2, Vector2)? BoundByBox(Vector2 p1, Vector2 p2)
		{
			List<Vector2> intersections = new(2);

			// Check if the line segment intersects with the box edges
			Vector2[] boxCorners = new Vector2[4]
			{
				new(-1, -1),
				new(1, -1),
				new(1, 1),
				new(-1, 1)
			};
			Vector2? inter1 = Intersection.LineSegmentLineSegment(p1, p2, boxCorners[0], boxCorners[1]);
			Vector2? inter2 = Intersection.LineSegmentLineSegment(p1, p2, boxCorners[1], boxCorners[2]);
			Vector2? inter3 = Intersection.LineSegmentLineSegment(p1, p2, boxCorners[2], boxCorners[3]);
			Vector2? inter4 = Intersection.LineSegmentLineSegment(p1, p2, boxCorners[3], boxCorners[0]);
			if (inter1.HasValue) intersections.Add(inter1.Value);
			if (inter2.HasValue) intersections.Add(inter2.Value);
			if (inter3.HasValue) intersections.Add(inter3.Value);
			if (inter4.HasValue) intersections.Add(inter4.Value);

			// Check if p1 is inside the box
			bool p1Inside = p1.x >= -1 && p1.x <= 1 && p1.y >= -1 && p1.y <= 1;

			if (intersections.Count == 0)
			{
				if (p1Inside)
				{
					return (p1, p2);
				}
				else
				{
					return null;
				}
			}
			else if (intersections.Count == 1)
			{
				if (p1Inside)
				{
					return (p1, intersections[0]);
				}
				else
				{
					return (intersections[0], p2);
				}
			}
			else
			{
				return (intersections[0], intersections[1]);
			}
		}
	}
}
