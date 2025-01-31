using UnityEngine;
using YBSlice.Data;

namespace YBSlice.HFSM.Tools
{
	public class AlignTool : ToolBase, ITool
	{
		private SlicerEditState _editState;
		private SlicerFlow _slicerFlow;
		private SlicerConfigData _config;

		public AlignTool(
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
			Debug.Log("AlignTool init");
		}
		public void Destroy()
		{
			Debug.Log("AlignTool destroy");
		}

		protected override bool TryUpdateNormalMode()
		{
			if (_editState.TryGetOrCreateLastInactivePoint(out SlicePoint point))
			{
				SlicePoint targetPoint = _slicerFlow.SliceLine[^2];
				if (TryGetPositionAtPointAxis(targetPoint, out Vector3 position))
				{
					AlignToNearestAxis(targetPoint, ref position);

					_editState.MovePoint(ref point, position, targetPoint.normal);
					_slicerFlow.SliceLine.InsertLast(point);

					return true;
				}
			}

			return false;
		}

		protected override bool TryUpdateInsideOnlyMode()
		{
			if (TryUpdateNormalMode())
			{
				if (_editState.TryGetOrCreateLastInactivePoint(out SlicePoint point))
				{
					if (TryGetSlicerObject(_slicerFlow.SelectedSlicerObject, new Ray(point.position + (0.01f * point.normal), -point.normal), out Vector3 position, out Vector3 normal))
					{
						_editState.MovePoint(ref point, position, normal);
						_slicerFlow.SliceLine.InsertLast(point);

						return true;
					}
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
		}

		private void AlignToNearestAxis(SlicePoint targetPoint, ref Vector3 position)
		{
			Vector3 tangent = Vector3.Cross(targetPoint.normal, Vector3.up).normalized;
			if (tangent.sqrMagnitude < 0.01f)
			{
				tangent = Vector3.Cross(targetPoint.normal, Vector3.right).normalized;
			}
			
			Vector3 bitangent = Vector3.Cross(targetPoint.normal, tangent).normalized;

			Vector3 localOffset = position - targetPoint.position;
			float projTangent = Vector3.Dot(localOffset, tangent);
			float projBitangent = Vector3.Dot(localOffset, bitangent);

			if (Mathf.Abs(projTangent) > Mathf.Abs(projBitangent))
			{
				position = targetPoint.position + projTangent * tangent;
			}
			else
			{
				position = targetPoint.position + projBitangent * bitangent;
			}
		}
	}
}