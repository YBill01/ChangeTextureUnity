using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using YBSlice.Objects;

namespace YBSlice.HFSM.Tools
{
	public abstract class ToolBase
	{
		public bool Enable { get; set; } = true;

		private SlicerEditState _editState;
		private SlicerFlow _slicerFlow;

		public ToolBase(SlicerEditState editState, SlicerFlow slicerFlow)
		{
			_editState = editState;
			_slicerFlow = slicerFlow;
		}

		public void Update()
		{
			if (Enable && !EventSystem.current.IsPointerOverGameObject())
			{
				switch (_slicerFlow.ToolMode.Value)
				{
					case ToolMode.Normal:
						if (!TryUpdateNormalMode())
						{
							_editState.TryRemoveLastInactivePoint();
						}

						break;
					case ToolMode.InsideOnly:
						if (!TryUpdateInsideOnlyMode())
						{
							_editState.TryRemoveLastInactivePoint();
						}

						break;
				}
			}
			else
			{
				_editState.TryRemoveLastInactivePoint();
			}
		}

		protected abstract bool TryUpdateNormalMode();
		protected abstract bool TryUpdateInsideOnlyMode();

		protected bool TryGetPositionAtPointAxis(SlicePoint targetPoint, out Vector3 position)
		{
			position = Vector3.zero;

			Ray rayOrigin = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

			if (Vector3.Dot(rayOrigin.direction, targetPoint.normal) > 0)
			{
				return false;
			}

			Plane plane = new Plane(targetPoint.normal, targetPoint.position);
			if (plane.Raycast(rayOrigin, out float enter))
			{
				position = rayOrigin.GetPoint(enter);

				return true;
			}

			return false;
		}

		protected bool TryGetSlicerObject(SlicerObject slicerObject, Ray rayOrigin, out Vector3 position, out Vector3 normal)
		{
			position = Vector3.zero;
			normal = Vector3.zero;

			if (slicerObject.GetComponent<Collider>().Raycast(rayOrigin, out RaycastHit hitInfo, 100.0f))
			{
				position = hitInfo.point;
				normal = hitInfo.normal;

				return true;
			}

			return false;
		}
	}
}