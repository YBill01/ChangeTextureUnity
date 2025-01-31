using UnityEngine;
using VContainer;
using YBSlice.Input;
using UnityEditor;

namespace YBSlice.Debugger
{
	public class SlicerDebug : MonoBehaviour
	{
		private IObjectResolver _resolver;

		private SlicerFlow _slicerFlow;
		private InputUIControl _inputUIControl;
		private InputInteractionControl _inputInteractionControl;

		[Inject]
		public void Construct(
			IObjectResolver resolver,
			SlicerFlow slicerFlow,
			InputUIControl inputUIControl,
			InputInteractionControl inputInteractionControl)
		{
			_resolver = resolver;
			_slicerFlow = _resolver.Resolve<SlicerFlow>();
			_slicerFlow = slicerFlow;
			_inputUIControl = inputUIControl;
			_inputInteractionControl = inputInteractionControl;
		}

#if UNITY_EDITOR
		// UNITY GIZMOS
		private void OnDrawGizmos()
		{
			if (Application.isPlaying)
			{
				if (_slicerFlow.SelectedSlicerObject)
				{
					Handles.color = Color.blue;
					Handles.DrawWireCube(_slicerFlow.SelectedSlicerObject.Bounds.center, _slicerFlow.SelectedSlicerObject.Bounds.size);
				}

				for (int i = 0; i < _slicerFlow.SliceLine.Count; i++)
				{
					Handles.color = Color.yellow;

					Handles.Label(_slicerFlow.SliceLine[i].position, $"p: {i}");

					if (i == 0)
					{
						Handles.DrawWireDisc(_slicerFlow.SliceLine[i].position, _slicerFlow.SliceLine[i].normal, 0.1f, 2.0f);
					}
					else
					{
						Handles.DrawWireDisc(_slicerFlow.SliceLine[i].position, _slicerFlow.SliceLine[i].normal, 0.05f, 1.0f);
					}

					if (i < _slicerFlow.SliceLine.Count - 1)
					{
						Handles.DrawLine(_slicerFlow.SliceLine[i].position, _slicerFlow.SliceLine[i + 1].position, 1.0f);
					}

					Handles.color = Color.blue;
					Handles.DrawLine(_slicerFlow.SliceLine[i].position, _slicerFlow.SliceLine[i].position + (_slicerFlow.SliceLine[i].normal * 0.25f)); // Z

					Vector3 tangent = Vector3.Cross(_slicerFlow.SliceLine[i].normal, Vector3.up).normalized;
					if (tangent.sqrMagnitude < 0.01f)
					{
						tangent = Vector3.Cross(_slicerFlow.SliceLine[i].normal, Vector3.right).normalized;
					}
					Vector3 bitangent = Vector3.Cross(_slicerFlow.SliceLine[i].normal, tangent).normalized;

					Handles.color = Color.red;
					Handles.DrawLine(_slicerFlow.SliceLine[i].position, _slicerFlow.SliceLine[i].position + (tangent * 0.25f)); // X

					Handles.color = Color.green;
					Handles.DrawLine(_slicerFlow.SliceLine[i].position, _slicerFlow.SliceLine[i].position + (bitangent * 0.25f)); // Y
				}
			}
		}
#endif
	}
}