using System;
using UnityEngine;

namespace YBSlice.Objects
{
	[RequireComponent(typeof(Renderer), typeof(MeshFilter))]
	public class SlicerObject : InteractionBehaviour
	{
		public event Action<bool> OnTouch;
		public event Action<Vector3, Vector3> OnTouchPoint;
		public event Action<Vector3, Vector3> OnClickPoint;

		private const string DEFAULT_LAYER = "Default";
		private const string OUTLINE_LAYER = "Outline";
		private const string SELECTED_LAYER = "Selected";

		public Bounds Bounds => _renderer.bounds;
		public Vector3 Axis => transform.forward;

		public bool IsSelected { get; private set; }

		private int _defaultLayer;
		private int _outlineLayer;
		private int _selectedLayer;

		private Renderer _renderer;

		protected override void Awake()
		{
			base.Awake();

			_defaultLayer = LayerMask.NameToLayer(DEFAULT_LAYER);
			_outlineLayer = LayerMask.NameToLayer(OUTLINE_LAYER);
			_selectedLayer = LayerMask.NameToLayer(SELECTED_LAYER);

			_renderer = GetComponent<Renderer>();

		}

		public void Select()
		{
			gameObject.layer = _selectedLayer;

			IsSelected = true;
		}
		public void Deselect()
		{
			gameObject.layer = _defaultLayer;

			IsSelected = false;
		}

		public override void OnTouchInternal(bool enter)
		{
			if (!IsSelected)
			{
				if (enter)
				{
					gameObject.layer = _outlineLayer;
				}
				else
				{
					gameObject.layer = _defaultLayer;
				}
			}

			OnTouch?.Invoke(enter);
		}

		public void SetTexture(Texture texture)
		{
			_renderer.material.mainTexture = texture;
		}

		public override void OnTouchPointInternal(Vector3 position, Vector3 normal)
		{
			OnTouchPoint?.Invoke(position, normal);
		}
		public override void OnClickInternal(Vector3 position, Vector3 normal)
		{
			OnClickPoint?.Invoke(position, normal);
		}
	}
}