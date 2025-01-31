using System;
using UnityEngine;
using VContainer.Unity;
using YB.Helpers;
using YBSlice.Data;
using YBSlice.HFSM;
using YBSlice.HFSM.Tools;
using YBSlice.Input;
using YBSlice.Objects;
using YBSlice.UI;

namespace YBSlice
{
	public class SlicerFlow : IInitializable, IStartable, IDisposable, ITickable
	{
		public event Action OnSelected;
		public event Action OnDeselected;
		
		public BindableProperty<ToolMode> ToolMode;

		private SlicerObject _selectedSlicerObject;
		public SlicerObject SelectedSlicerObject => _selectedSlicerObject;

		private SliceLine _sliceLine;
		public SliceLine SliceLine => _sliceLine;

		private SlicerStateMachine _stateMachine;

		private SlicerConfigData _config;
		private InputCameraControl _cameraController;
		private InputInteractionControl _inputInteractionControl;
		private InputUIControl _inputUIControl;
		private UICanvas _ui;
		private Transform _testCubeContainer;

		public SlicerFlow(
			SlicerConfigData config,
			InputCameraControl cameraController,
			InputInteractionControl inputInteractionControl,
			InputUIControl inputUIControl,
			UICanvas ui,
			Transform testCubeContainer)
		{
			_config = config;
			_cameraController = cameraController;
			_inputInteractionControl = inputInteractionControl;
			_inputUIControl = inputUIControl;
			_ui = ui;
			_testCubeContainer = testCubeContainer;
		}

		public void Initialize()
		{
			_sliceLine = new SliceLine();

			SlicerChoseState choseState = new SlicerChoseState(this, _inputInteractionControl, _cameraController, _ui);
			SlicerEditState editState = new SlicerEditState(this, _config, _inputInteractionControl, _inputUIControl, _cameraController, _ui);
			
			_stateMachine = new SlicerStateMachine(choseState, editState);

			OnSelected += choseState.AddEventTransition(editState);
			OnDeselected += editState.AddEventTransition(choseState);
			_ui.OnCloseEditorScreen += editState.AddEventTransition(choseState);
		}

		public void Start()
		{
			_sliceLine.Clear();
			_stateMachine.Init();
		}

		public void Dispose()
		{

		}

		public void Tick()
		{
			_stateMachine.Update();
		}

		public void Selected(SlicerObject slicerObject)
		{
			_selectedSlicerObject = slicerObject;

			_selectedSlicerObject?.Select();

			OnSelected?.Invoke();
		}
		public void Deselected()
		{
			_selectedSlicerObject?.Deselect();

			_selectedSlicerObject = null;

			OnDeselected?.Invoke();
		}

		public void EditorExit()
		{
			_ui.EditorScreenHide();
		}

		public void VisibleTestCube()
		{
			EditorExit();

			_testCubeContainer.gameObject.SetActive(!_testCubeContainer.gameObject.activeSelf);
		}
	}
}