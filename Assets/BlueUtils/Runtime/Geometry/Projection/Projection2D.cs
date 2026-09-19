using System.Linq;
using UnityEngine;

namespace BlueUtils.Geometry
{
	public static partial class Projection2D
	{
		/// <summary>
		/// Projects a vector onto another vector.
		/// </summary>
		/// <param name="vector">Vector to project.</param>
		/// <param name="onto">Vector to project onto.</param>
		/// <returns>The projection of the vector onto the other vector.</returns>
		/// <remarks>This is equivalent to the dot product of both vectors multiplied by the unit vector of the onto vector.</remarks>
		public static Vector2 Project(Vector2 vector, Vector2 onto)
		{
			float dot = Vector2.Dot(vector, onto);
			return onto * (dot / onto.sqrMagnitude);
		}

		/// <summary>
		/// Calculates the length of the projection of a vector onto another vector.
		/// </summary>
		/// <param name="vector">Vector to project.</param>
		/// <param name="onto">Vector to project onto.</param>
		/// <returns>The length of the projection.</returns>
		/// <remarks>If the onto vector is normalised, this is equivalent to the dot product of both vectors.</remarks>
		public static float ProjectLength(Vector2 vector, Vector2 onto)
		{
			float dot = Vector2.Dot(vector, onto);
			return dot / onto.magnitude;
		}
	}
}
