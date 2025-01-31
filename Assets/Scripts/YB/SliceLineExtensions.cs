using UnityEngine;

namespace YBSlice
{
	public static class SliceLineExtensions
	{
		public static Vector2[] WorldToScreen(this SliceLine l, Camera camera, bool useInactive = true)
		{
			Vector2[] result = useInactive ? new Vector2[l.Count] : new Vector2[l.CountActive];

			int i = 0;
			foreach (SlicePoint point in l)
			{
				if (!useInactive && point.inactive)
				{
					continue;
				}

				result[i++] = Camera.main.WorldToScreenPoint(point.position);
			}

			return result;
		}

		public static bool HasIntersectingLines(this SliceLine l, Vector2[] points, bool isClosed)
		{
			if (points.Length < 4) return false;

			int lineCount = isClosed ? points.Length : points.Length - 1;

			for (int i = 0; i < lineCount; i++)
			{
				Vector2 p1 = points[i];
				Vector2 q1 = points[(i + 1) % points.Length];

				for (int j = i + 2; j < (isClosed ? lineCount : points.Length - 1); j++)
				{
					if (j == i + 1 || (isClosed && j == points.Length - 1 && i == 0))
						continue;

					Vector2 p2 = points[j];
					Vector2 q2 = points[(j + 1) % points.Length];

					if (q2.Equals(p1)) continue;

					if (DoSegmentsIntersect(p1, q1, p2, q2))
					{
						return true;
					}
				}
			}

			return false;
		}

		private static bool DoSegmentsIntersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
		{
			float orientation(Vector2 a, Vector2 b, Vector2 c)
			{
				return (b.y - a.y) * (c.x - b.x) - (b.x - a.x) * (c.y - b.y);
			}

			bool onSegment(Vector2 a, Vector2 b, Vector2 c)
			{
				return Mathf.Min(a.x, b.x) <= c.x && c.x <= Mathf.Max(a.x, b.x) &&
					Mathf.Min(a.y, b.y) <= c.y && c.y <= Mathf.Max(a.y, b.y);
			}

			float o1 = orientation(p1, q1, p2);
			float o2 = orientation(p1, q1, q2);
			float o3 = orientation(p2, q2, p1);
			float o4 = orientation(p2, q2, q1);

			if (o1 * o2 < 0 && o3 * o4 < 0) return true;

			if (Mathf.Approximately(o1, 0) && onSegment(p1, q1, p2)) return true;
			if (Mathf.Approximately(o2, 0) && onSegment(p1, q1, q2)) return true;
			if (Mathf.Approximately(o3, 0) && onSegment(p2, q2, p1)) return true;
			if (Mathf.Approximately(o4, 0) && onSegment(p2, q2, q1)) return true;

			return false;
		}
	}
}