using System;
using System.Linq;
using UnityEngine;

namespace BlueUtils.Polygons
{
	public static partial class Polygons
	{
		public static Vector2[] OrderLeftToRight(Vector2[] points, Vector2 up)
		{
			Vector2 left = Vector2.Perpendicular(up).normalized;
			Vector2[] ordered = points.OrderBy(points => -Vector2.Dot(points, left)).ToArray();
			return ordered;
		}

		public static void OrderLeftToRight(ref Vector2[] points, Vector2 up)
		{
			Vector2 left = Vector2.Perpendicular(up).normalized;
			Array.Sort(points, (a, b) => -Vector2.Dot(a, left).CompareTo(Vector2.Dot(b, left)));
		}
	}
}
