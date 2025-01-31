using UnityEngine;
using YB.HFSM;
using YBSlice.Input;
using YBSlice.Objects;
using YBSlice.UI;

namespace YBSlice.HFSM
{
	public class SlicerChoseState : State
	{
		private SlicerFlow _slicerFlow;
		private InputInteractionControl _inputInteractionControl;
		private InputCameraControl _cameraController;
		private UICanvas _ui;

		public SlicerChoseState(
			SlicerFlow slicerFlow,
			InputInteractionControl inputInteractionControl,
			InputCameraControl cameraController,
			UICanvas ui)
		{
			_slicerFlow = slicerFlow;
			_inputInteractionControl = inputInteractionControl;
			_cameraController = cameraController;
			_ui = ui;
		}

		protected override void OnEnter()
		{
			_inputInteractionControl.OnClick += InputInteractionControlOnClick;
			_cameraController.OnMove += CameraControllerOnMove;

			_inputInteractionControl.gameObject.SetActive(true);

			Debug.Log("Chose enter");
		}
		protected override void OnExit()
		{
			_inputInteractionControl.OnClick -= InputInteractionControlOnClick;
			_cameraController.OnMove -= CameraControllerOnMove;

			Debug.Log("Chose exit");
		}

		private void InputInteractionControlOnClick(IInteraction interaction)
		{
			if (interaction != null && interaction.SourceGameObject.TryGetComponent(out SlicerObject slicerObject))
			{
				_slicerFlow.SelectedSlicerObject?.Deselect();
				_slicerFlow.Selected(slicerObject);
			}
		}

		private void CameraControllerOnMove(bool value)
		{
			if (value)
			{
				_inputInteractionControl.PerformedCancel();
			}
		}
	}
}