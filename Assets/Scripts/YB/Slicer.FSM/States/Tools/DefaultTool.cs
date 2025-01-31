using UnityEngine;
using UnityEngine.InputSystem;
using YBSlice.Data;

namespace YBSlice.HFSM.Tools
{
	public class DefaultTool : ToolBase, ITool
	{
		private SlicerEditState _editState;
		private SlicerFlow _slicerFlow;
		private SlicerConfigData _config;

		public DefaultTool(
			SlicerEditState editState,
			SlicerFlow slicerFlow,
			SlicerConfigData config) : base(editState, slicerFlow)
		{
			_editState = editState;
			_slicerFlow = slicerFlow;
			_config = config;
		}

		public void Init()
		{
			Debug.Log("DefaultTool init");
		}
		public void Destroy()
		{
			Debug.Log("DefaultTool destroy");
		}

		protected override bool TryUpdateNormalMode()
		{
			if (TryUpdateInsideOnlyMode())
			{
				return true;
			}
			else if (!_slicerFlow.SliceLine.IsEmptyActive)
			{
				if (_editState.TryGetOrCreateLastInactivePoint(out SlicePoint point))
				{
					SlicePoint targetPoint = _slicerFlow.SliceLine[^2];
					if (TryGetPositionAtPointAxis(targetPoint, out Vector3 position))
					{
						_editState.MovePoint(ref point, position, targetPoint.normal);
						_slicerFlow.SliceLine.InsertLast(point);
						AutoLoopPoint(ref point);
						_slicerFlow.SliceLine.InsertLast(point);

						return true;
					}
				}
			}

			return false;
		}

		protected override bool TryUpdateInsideOnlyMode()
		{
			if (TryGetSlicerObject(_slicerFlow.SelectedSlicerObject, Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out Vector3 position, out Vector3 normal))
			{
				if (_editState.TryGetOrCreateLastInactivePoint(out SlicePoint point))
				{
					_editState.MovePoint(ref point, position, normal);
					_slicerFlow.SliceLine.InsertLast(point);
					AutoLoopPoint(ref point);
					_slicerFlow.SliceLine.InsertLast(point);

					return true;
				}
			}

			return false;
		}

		public void Click()
		{
			if (!Enable)
			{
				return;
			}

			if (_editState.TryInsertPoint(out bool isLooped))
			{

			}
			else if (isLooped && _editState.TryApplyLine())
			{

			}
		}

		private void AutoLoopPoint(ref SlicePoint point)
		{
			if (_slicerFlow.SliceLine.IsCuttlingValid)
			{
				Vector2[] points2d = _slicerFlow.SliceLine.WorldToScreen(Camera.main);
				if (Vector2.Distance(points2d[0], points2d[^1]) <= _config.autoCloseLineDistance)
				{
					point.position = _slicerFlow.SliceLine.First.position;
					point.normal = _slicerFlow.SliceLine.First.normal;
				}
			}
		}
	}
}