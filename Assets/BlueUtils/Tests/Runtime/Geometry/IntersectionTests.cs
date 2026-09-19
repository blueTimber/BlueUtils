using BlueUtils.Geometry;
using NUnit.Framework;

namespace BlueUtils.Tests.Geometry
{
	public class IntersectionTests
	{
		[Test]
		public void LineLine2DPerpendicular()
		{
			Assert.AreSame(
				Intersection2D.LineLine(
					new UnityEngine.Ray2D(UnityEngine.Vector2.zero, UnityEngine.Vector2.right), 
					new UnityEngine.Ray2D(UnityEngine.Vector2.zero, UnityEngine.Vector2.left)
				).HasValue, 
				false
			);
		}
	}
}
