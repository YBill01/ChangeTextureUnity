using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace YBSlice
{
	public class SliceLine : IEnumerable<SlicePoint>
	{
		public event Action<SliceLine> OnChanged = delegate { };
		public event Action<int> OnAdd = delegate { };

		public int Count => _points.Count;
		public int CountActive => _points.Count(x => !x.inactive);

		public bool IsEmpty => Count == 0;
		public bool IsEmptyActive => CountActive == 0;

		public bool IsCuttlingValid => CountActive >= 2;

		public SlicePoint First => _points[0];
		public SlicePoint Last => _points[^1];

		public int LastIndex => _points.Count - 1;

		public Vector3[] Points => _points
			.Where(x => !x.inactive)
			.ToList()
			.ConvertAll(x => x.position)
			.ToArray();

		private List<SlicePoint> _points;

		public SliceLine()
		{
			_points = new List<SlicePoint>();
		}

		public SlicePoint this[int index]
		{
			get => _points[index];
			set => _points[index] = value;
		}

		public bool TryAdd(SlicePoint point)
		{
			if (_points.Contains(point))
			{
				return false;
			}

			_points.Add(point);

			OnAdd?.Invoke(LastIndex);

			return true;
		}
		public void Insert(SlicePoint point, int index)
		{
			_points[index] = point;
		}
		public void InsertLast(SlicePoint point)
		{
			_points[^1] = point;
		}
		public void Remove(int index)
		{
			_points.RemoveAt(index);

			OnChanged?.Invoke(this);
		}
		public void RemoveLast()
		{
			_points.RemoveAt(LastIndex);

			OnChanged?.Invoke(this);
		}
		public bool TryRemoveLastInactive()
		{
			if (Last.inactive)
			{
				RemoveLast();

				return true;
			}

			return false;
		}

		public void Clear()
		{
			_points.Clear();

			OnChanged(this);
		}

		public IEnumerator<SlicePoint> GetEnumerator() => _points.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}