using System;
using UnityEngine;

namespace BlueUtils.Intersection
{
	public struct OriginCenteredLineSegment
	{
		#region Variables

		private float _start;
		private float _end;

		private Ray2D _line;

		#endregion

		#region Properties

		public readonly Ray2D Line => _line;

		public readonly Vector2 StartPoint => _line.GetPoint(_start);
		public readonly Vector2 EndPoint => _line.GetPoint(_end);

		#endregion

		public OriginCenteredLineSegment(Vector2 start, Vector2 end)
		{
			if (start.x < -1.0f || start.y < -1.0f || start.x > 1.0f || start.y > 1.0f)
				throw new ArgumentException("Start is outside of allowed area: " + start);

			_start = 0f;
			_end = (end - start).magnitude;
			_line = new Ray2D(start, (end - start).normalized);

			RestrictToScreen();
		}

		/// <summary>
		/// Create the segment bounded by the origin centered box for -1 to 1 on each axis.
		/// </summary>
		/// <param name="line">The origin of the line must be within the range -1 to 1 on each axis.</param>
		public OriginCenteredLineSegment(Ray2D line, bool forwardsOnly = false)
		{
			if (line.origin.x < -1.0f || line.origin.y < -1.0f || line.origin.x > 1.0f || line.origin.y > 1.0f)
				throw new ArgumentException("Centre is outside of allowed area");

			_start = forwardsOnly ? 0f : float.MinValue; 
			_end = float.MaxValue;
			_line = line;

			RestrictToScreen();
		}

		public OriginCenteredLineSegment(Ray2D line, float start, float end)
		{
			if (line.origin.x < -1.0f || line.origin.y < -1.0f || line.origin.x > 1.0f || line.origin.y > 1.0f)
				throw new ArgumentException("Centre is outside of allowed area");

			_line = line;
			_start = start;
			_end = end;

			RestrictToScreen();
		}

		#region Public Methods

		public OriginCenteredLineSegment Copy()
		{
			return new OriginCenteredLineSegment(_line, _start, _end);
		}

		public void FlipDirection()
		{
			(_start, _end) = (_end, _start);
			_line.direction *= -1;
		}

		/// <summary>
		/// Restrict the line segment by a ray.
		/// </summary>
		/// <param name="line"></param>
		/// <returns>True if the line was changed.</returns>
		public bool Restrict(Ray2D line)
		{
			float intersectPoint;
			try
			{
				intersectPoint = IntersectionUtils.RayRay2D(_line, line);
			}
			catch
			{
				return false;
			}

			if (intersectPoint >= 0.0f && intersectPoint < _end)
			{
				_end = intersectPoint;
				return true;
			}
			else if (intersectPoint < 0.0f && intersectPoint > _start)
			{
				_start = intersectPoint;
				return true;
			}

			return false;
		}

		public bool Restrict(Vector2 start, Vector2 end)
		{
			Vector2? intersectPoint = IntersectionUtils.LineSegmentLineSegment(StartPoint, EndPoint, start, end);
			if (!intersectPoint.HasValue) return false;

			float projectedPoint = Comparison.ProjectLength(intersectPoint.Value - _line.origin, _line.direction); 

			if (projectedPoint >= 0.0f && projectedPoint < _end)
			{
				_end = projectedPoint;
				return true;
			}
			else if (projectedPoint < 0.0f && projectedPoint > _start)
			{
				_start = projectedPoint;
				return true;
			}

			return false;
		}

		public readonly OriginCenteredLineSegment? Split(Ray2D line)
		{
			float intersectPoint;
			try
			{
				intersectPoint = IntersectionUtils.RayRay2D(_line, line);
			}
			catch
			{
				return null;
			}

			return null;
		}

		public readonly Vector2? Intersect(OriginCenteredLineSegment segment)
		{
			return IntersectionUtils.LineSegmentLineSegment(
				_line.GetPoint(_start), _line.GetPoint(_end),
				segment._line.GetPoint(segment._start), segment._line.GetPoint(segment._end)
				);
		}

		public readonly Vector2? Intersect(Ray2D ray)
		{
			float intersectPoint;
			try
			{
				intersectPoint = IntersectionUtils.RayRay2D(_line, ray);
			}
			catch
			{
				return null;
			}

			if (intersectPoint >= _start && intersectPoint <= _end)
			{
				return _line.GetPoint(intersectPoint);
			}
			else
			{
				return null;
			}
		}

		#endregion

		#region Private Methods

		private void RestrictToScreen()
		{
			// Top
			Restrict(new Ray2D(new Vector2(1.0f, 1.0f), Vector2.left));
			// Right
			Restrict(new Ray2D(new Vector2(1.0f, 1.0f), Vector2.down));
			// Bottom
			Restrict(new Ray2D(new Vector2(1.0f, -1.0f), Vector2.left));
			// Left
			Restrict(new Ray2D(new Vector2(-1.0f, 1.0f), Vector2.down));
		}

		#endregion
	}
}
