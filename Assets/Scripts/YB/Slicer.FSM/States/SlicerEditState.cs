using UnityEngine;
using YB.HFSM;
using YBSlice.Data;
using YBSlice.HFSM.Tools;
using YBSlice.Input;
using YBSlice.Objects;
using YBSlice.UI;

namespace YBSlice.HFSM
{
	public class SlicerEditState : State
	{
		private SlicerFlow _slicerFlow;
		private SlicerConfigData _config;
		private InputInteractionControl _inputInteractionControl;
		private InputUIControl _inputUIControl;
		private InputCameraControl _cameraController;
		private UICanvas _ui;

		private ITool _tool;
		private DefaultTool _defaultTool;
		private AlignTool _alignTool;
		private Slicer _slicer;

		public SlicerEditState(
			SlicerFlow slicerFlow,
			SlicerConfigData config,
			InputInteractionControl inputInteractionControl,
			InputUIControl inputUIControl,
			InputCameraControl cameraController,
			UICanvas ui)
		{
			_slicerFlow = slicerFlow;
			_config = config;
			_inputInteractionControl = inputInteractionControl;
			_inputUIControl = inputUIControl;
			_cameraController = cameraController;
			_ui = ui;

			_defaultTool = new DefaultTool(this, _slicerFlow, _config);
			_alignTool = new AlignTool(this, _slicerFlow, _config);
			_tool = _defaultTool;
			_slicer = new Slicer();
		}

		protected override void OnEnter()
		{
			_ui.EditorScreenShow();

			_inputInteractionControl.OnClick += InputInteractionControlOnClick;
			_ui.OnTextureItemSelected += UIOnTextureItemSelected;
			_cameraController.OnMove += CameraControllerOnMove;
			_inputUIControl.OnClick += InputUIControlOnClick;
			_inputUIControl.OnSubmit += InputUIControlOnSubmit;
			_inputUIControl.OnCancel += InputUIControlOnCancel;
			_inputUIControl.OnShift += InputUIControlOnShift;

			Selected(_slicerFlow.SelectedSlicerObject);

			ChangeTool(_defaultTool);

			Debug.Log("Edit enter");
		}
		protected override void OnExit()
		{
			_inputInteractionControl.OnClick -= InputInteractionControlOnClick;
			_ui.OnTextureItemSelected -= UIOnTextureItemSelected;
			_cameraController.OnMove -= CameraControllerOnMove;
			_inputUIControl.OnClick -= InputUIControlOnClick;
			_inputUIControl.OnSubmit -= InputUIControlOnSubmit;
			_inputUIControl.OnCancel -= InputUIControlOnCancel;
			_inputUIControl.OnShift -= InputUIControlOnShift;

			Deselected();

			_tool.Destroy();

			_inputInteractionControl.PerformedClear();
			_inputInteractionControl.gameObject.SetActive(true);

			Debug.Log("Edit exit");
		}
		protected override void OnUpdate()
		{
			_tool.Update();
		}

		private void Selected(SlicerObject slicerObject)
		{
			_slicerFlow.SliceLine.Clear();
			_slicerFlow.Selected(slicerObject);
		}
		private void Deselected()
		{
			_slicerFlow.SliceLine.Clear();
			_slicerFlow.SelectedSlicerObject.Deselect();
		}

		private void ChangeTool(ITool tool)
		{
			_tool.Destroy();
			_tool = tool;
			_tool.Init();
		}


		private void InputInteractionControlOnClick(IInteraction interaction)
		{
			if (!_slicerFlow.SliceLine.IsEmpty)
			{
				return;
			}

			if (interaction is null)
			{
				_slicerFlow.EditorExit();

				return;
			}

			if (interaction.SourceGameObject.TryGetComponent(out SlicerObject slicerObject))
			{
				if (!ReferenceEquals(slicerObject, _slicerFlow.SelectedSlicerObject))
				{
					Deselected();
					Selected(slicerObject);
				}
			}
		}

		public bool TryGetOrCreateLastInactivePoint(out SlicePoint point)
		{
			if (!_slicerFlow.SliceLine.IsEmpty && _slicerFlow.SliceLine.Last.inactive)
			{
				point = _slicerFlow.SliceLine.Last;

				return true;
			}

			point = new SlicePoint
			{
				inactive = true
			};

			return _slicerFlow.SliceLine.TryAdd(point);
		}

		public bool TryRemoveLastInactivePoint()
		{
			if (_slicerFlow.SliceLine.IsEmpty)
			{
				return false;
			}

			return _slicerFlow.SliceLine.TryRemoveLastInactive();
		}
		
		public void MovePoint(ref SlicePoint point, Vector3 position, Vector3 normal)
		{
			point.position = position;
			point.normal = normal;
		}
		public bool TryInsertPoint(out bool isLooped)
		{
			isLooped = false;

			if (_slicerFlow.SliceLine.IsEmpty)
			{
				return false;
			}

			SlicePoint point = _slicerFlow.SliceLine.Last;

			if (!point.inactive)
			{
				return false;
			}

			if (_slicerFlow.SliceLine.Count >= 2 && point.Equals(_slicerFlow.SliceLine.First))
			{
				isLooped = true;

				return false;
			}

			if ((_slicerFlow.SliceLine.IsCuttlingValid && point.Equals(_slicerFlow.SliceLine[^2])))
			{
				return false;
			}

			if (!_slicerFlow.SliceLine.HasIntersectingLines(_slicerFlow.SliceLine.WorldToScreen(Camera.main), false))
			{
				SlicePoint newPoint = new SlicePoint
				{
					position = point.position,
					normal = point.normal,
					inactive = false
				};

				_slicerFlow.SliceLine.InsertLast(newPoint);

				CheckInteractionState();

				return true;
			}

			return false;
		}
		
		public bool TryApplyLine()
		{
			if (_slicerFlow.SliceLine.IsCuttlingValid)
			{
				if (!_slicerFlow.SliceLine.HasIntersectingLines(_slicerFlow.SliceLine.WorldToScreen(Camera.main, false), true))
				{
					Vector3[] points = _slicerFlow.SliceLine.Points;

					if (_slicer.IsPointsVisible(points))
					{
						if (_slicer.TrySlice(_slicerFlow.SelectedSlicerObject, points, out SlicerObject partObject1, out SlicerObject partObject2))
						{
							CheckInteractionState();

							Deselected();
							Selected(partObject1);

							_ui.SendMessageScreenShow("Cut complete!");

							return true;
						}
						else
						{
							_ui.SendMessageScreenShow("Cut attempt failed!");
						}
					}
					else
					{
						_ui.SendMessageScreenShow("Some points are outside the camera's field of view!");
					}
				}
				else
				{
					_ui.SendMessageScreenShow("Some lines intersect!");
				}
			}

			return false;
		}

		private void CheckInteractionState()
		{
			_inputInteractionControl.PerformedClear();
			_inputInteractionControl.gameObject.SetActive(_slicerFlow.SliceLine.IsEmptyActive);
		}

		private void CameraControllerOnMove(bool value)
		{
			if (value)
			{
				_inputInteractionControl.PerformedCancel();
				_tool.Enable = false;
			}
			else
			{
				_tool.Enable = true;
			}
		}

		private void InputUIControlOnClick()
		{
			_tool.Click();
		}
		
		private void InputUIControlOnSubmit()
		{
			TryApplyLine();
		}
		private void InputUIControlOnCancel()
		{
			if (!_slicerFlow.SliceLine.IsEmptyActive)
			{
				_slicerFlow.SliceLine.Clear();
			}
			else
			{
				_slicerFlow.EditorExit();
			}

			CheckInteractionState();
		}
		private void InputUIControlOnShift(bool value)
		{
			if (!_slicerFlow.SliceLine.IsEmptyActive)
			{
				if (value)
				{
					ChangeTool(_alignTool);
				}
				else
				{
					ChangeTool(_defaultTool);
				}
			}
		}

		private void UIOnTextureItemSelected(int index)
		{
			_slicerFlow.SelectedSlicerObject.SetTexture(_config.textureItems[index].texture);
		}
	}
}