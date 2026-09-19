using UnityEngine;

namespace BlueUtils.Geometry
{
	/// <summary>
	/// A projection plane is a 2D plane in 3D space defined by an origin point and two orthogonal vectors.
	/// It allows for easy conversion between 3D points and their 2D projections onto the plane.
	/// </summary>
	public readonly struct ProjectionPlane
	{
		#region Properties

		public Vector3 Origin { get; }

		public Vector3 Xvector { get; }
		public Vector3 Yvector { get; }

		#endregion

		/// <summary>
		/// Create a projection plane from three points in 3D space.
		/// Ensure the three points are not on the same line, otherwise the plane will be nonsensical.
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="p3"></param>
		public ProjectionPlane(Vector3 p1, Vector3 p2, Vector3 p3)
		{
			Origin = p1;

			Xvector = (p2 - p1).normalized;

			Vector3 normal = Vector3.Cross(Xvector, (p3 - p1));
			Yvector = Vector3.Cross(Xvector, normal).normalized;
		}

		#region Public Methods

		public Vector2 ToVector2(Vector3 point)
		{
			Vector2 result = new()
			{
				x = Vector3.Dot(point - Origin, Xvector),
				y = Vector3.Dot(point - Origin, Yvector)
			};

			return result;
		}

		public Vector3 ToVector3(Vector2 point)
		{
			return Origin + point.x * Xvector + point.y * Yvector;
		}

		#endregion
	}
}
