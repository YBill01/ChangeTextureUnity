using UnityEngine;
using UnityEngine.UI;
using VContainer;
using YB.Helpers;
using YBSlice.Data;
using YBSlice.HFSM.Tools;

namespace YBSlice.UI
{
	public class UIMainScreen : UIScreen
	{
		[Space]
		[SerializeField]
		private UIToggleButton m_toolModeToggleButton;
		
		[Space]
		[SerializeField]
		private Button m_visibleTestCubeButton;

		private SlicerConfigData _config;
		private SlicerFlow _slicerFlow;
		
		[Inject]
		public void Construct(
			SlicerConfigData config,
			SlicerFlow slicerFlow)
		{
			_config = config;
			_slicerFlow = slicerFlow;
		}

		private void OnEnable()
		{
			//m_toolModeToggleButton.onValueChanged += ToolModeToggleButtonOnValueChanged;
			m_visibleTestCubeButton.onClick.AddListener(VisibleTestCubeButtonOnClick);
		}
		private void OnDisable()
		{
			//m_toolModeToggleButton.onValueChanged -= ToolModeToggleButtonOnValueChanged;
			m_visibleTestCubeButton.onClick.RemoveListener(VisibleTestCubeButtonOnClick);
		}

		protected override void OnPreShow()
		{
			m_toolModeToggleButton.Value = _config.toolMode == ToolMode.Normal;
			_slicerFlow.ToolMode = BindableProperty<ToolMode>.Bind(() => m_toolModeToggleButton.Value ? ToolMode.Normal : ToolMode.InsideOnly);
		}

		/*private void ToolModeToggleButtonOnValueChanged(bool value)
		{
			
		}*/
		private void VisibleTestCubeButtonOnClick()
		{
			_slicerFlow.VisibleTestCube();
		}
	}
}