using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace YBSlice.UI
{
	public class UILineCanvas : MonoBehaviour, IDisposable
	{
		[SerializeField]
		private Image m_pointRootPrefab;

		[SerializeField]
		private Image m_pointPrefab;

		[Space]
		[SerializeField]
		private UILineRenderer m_line;
		[SerializeField]
		private UILineRenderer m_lineLooped;

		private List<Image> _points;

		private bool IsLineValid => _points.Count >= 2;
		private bool IsLineLoopedValid => _points.Count >= 3;

		private SlicerFlow _slicerFlow;

		[Inject]
		public void Construct(
			SlicerFlow slicerFlow)
		{
			_slicerFlow = slicerFlow;
		}

		private void Awake()
		{
			_points = new List<Image>();
		}

		public void Init()
		{
			LineUpdate();

			_slicerFlow.SliceLine.OnChanged += SliceLineOnChanged; ;
			_slicerFlow.SliceLine.OnAdd += SliceLineOnAdd;
		}
		public void Dispose()
		{
			Clear();

			_slicerFlow.SliceLine.OnChanged -= SliceLineOnChanged; ;
			_slicerFlow.SliceLine.OnAdd -= SliceLineOnAdd;
		}

		private void LateUpdate()
		{
			Vector2[] points2d = _slicerFlow.SliceLine.WorldToScreen(Camera.main);

			for (int i = 0; i < points2d.Length; i++)
			{
				_points[i].rectTransform.anchoredPosition = points2d[i];
				_points[i].color = _slicerFlow.SliceLine[i].inactive ? Color.yellow : Color.white;
			}

			if (IsLineValid)
			{
				m_line.Points = points2d;
				m_line.SetAllDirty();
			}

			if (IsLineLoopedValid)
			{
				bool isHasIntersectingLines = _slicerFlow.SliceLine.HasIntersectingLines(points2d, false);
				bool isHasIntersectingLoopLines = _slicerFlow.SliceLine.HasIntersectingLines(points2d, true);

				Vector2[] points2dLooped = { points2d[0], points2d[^1] };
				m_lineLooped.Points = points2dLooped;
				m_lineLooped.color = isHasIntersectingLines ? Color.red : (isHasIntersectingLoopLines ? Color.yellow : Color.green);
				m_lineLooped.SetAllDirty();

				if (isHasIntersectingLines)
				{
					_points[^1].color = Color.red;
				}
				else if (_slicerFlow.SliceLine.First.Equals(_slicerFlow.SliceLine.Last))
				{
					_points[^1].color = Color.green;
				}
			}
		}

		public void LineUpdate()
		{
			Clear();

			bool root = true;
			foreach (SlicePoint slicePoint in _slicerFlow.SliceLine)
			{
				Image point = Instantiate(root ? m_pointRootPrefab : m_pointPrefab, transform);
				root = false;
				
				_points.Add(point);
			}

			if (IsLineValid)
			{
				m_line.gameObject.SetActive(true);
				if (IsLineLoopedValid)
				{
					m_lineLooped.gameObject.SetActive(true);
				}
				else
				{
					m_lineLooped.gameObject.SetActive(false);
				}
			}
			else
			{
				m_line.gameObject.SetActive(false);
				m_lineLooped.gameObject.SetActive(false);
			}
		}

		public void Clear()
		{
			for (int i = 0; i < _points.Count; i++)
			{
				Destroy(_points[i].gameObject);
			}

			_points.Clear();

			m_line.Points = Array.Empty<Vector2>();
		}

		private void SliceLineOnChanged(SliceLine sliceLine)
		{
			LineUpdate();
		}
		private void SliceLineOnAdd(int index)
		{
			LineUpdate();
		}
	}
}