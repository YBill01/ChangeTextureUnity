using System;
using UnityEngine;

namespace YBSlice
{
	public struct SlicePoint : IEquatable<SlicePoint>
	{
		public Vector3 position;
		public Vector3 normal;

		public bool inactive;

		public bool Equals(SlicePoint other)
		{
			return position.Equals(other.position) && normal.Equals(other.normal);
		}

		public override string ToString()
		{
			return $"position: {position}\nnormal: {normal}\ninactive: {inactive}";
		}
	}
}