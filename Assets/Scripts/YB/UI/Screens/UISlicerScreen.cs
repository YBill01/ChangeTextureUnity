using UnityEngine;
using VContainer;
using YBSlice.Input;

namespace YBSlice.UI
{
	public class UISlicerScreen : UIScreen
	{

		[Space]
		[SerializeField]
		private UILineCanvas m_lineCanvas;


		private IObjectResolver _resolver;

		//private SlicerFlow _slicerFlow;
		//private InputUIControl _inputUIControl;

		[Inject]
		public void Construct(
			IObjectResolver resolver//,
			//SlicerFlow slicerFlow,
			/*InputUIControl inputUIControl,*/
			/*InputInteractionControl inputInteractionControl*/)
		{
			_resolver = resolver;

			//_slicerFlow = slicerFlow;
			//_inputUIControl = inputUIControl;
			//_inputInteractionControl = inputInteractionControl;
			_resolver.Inject(m_lineCanvas);
		}

		protected override void OnShow()
		{
			m_lineCanvas.Init();
		}
		protected override void OnHide()
		{
			m_lineCanvas.Dispose();
		}

	}
}