using System;
using UnityEngine;
using VContainer;
using YBSlice.Input;

namespace YBSlice.UI
{
	public class UICanvas : MonoBehaviour
	{
		public event Action OnCloseEditorScreen;
		public event Action<int> OnTextureItemSelected;

		[SerializeField]
		private UIScreenController m_uiController;

		private UIEditorMenuScreen _editorScreen;
		private UISlicerScreen _slicerScreen;

		private IObjectResolver _resolver;

		private InputUIControl _inputUIControl;
		
		[Inject]
		public void Construct(
			IObjectResolver resolver,
			InputUIControl inputUIControl,
			InputInteractionControl inputInteractionControl)
		{
			_resolver = resolver;
			_inputUIControl = inputUIControl;
		}

		private void OnEnable()
		{
			//_inputUIControl.OnCancel += InputUIControlOnCancel;
			
			m_uiController.OnShow.AddListener<UIMainScreen>(UIMainScreenOnShow);

			m_uiController.OnShow.AddListener<UIEditorMenuScreen>(UIEditorScreenOnShow);
			m_uiController.OnHide.AddListener<UIEditorMenuScreen>(UIEditorScreenOnHide);

			m_uiController.OnShow.AddListener<UISlicerScreen>(UISlicerScreenOnShow);
			m_uiController.OnHide.AddListener<UISlicerScreen>(UISlicerScreenOnHide);
		}
		private void OnDisable()
		{
			//_inputUIControl.OnCancel -= InputUIControlOnCancel;
			
			m_uiController.OnShow.RemoveListener<UIEditorMenuScreen>();
			m_uiController.OnHide.RemoveListener<UIEditorMenuScreen>();

			m_uiController.OnShow.RemoveListener<UISlicerScreen>();
			m_uiController.OnHide.RemoveListener<UISlicerScreen>();
		}

		public void EditorScreenShow()
		{
			m_uiController.Show<UIEditorMenuScreen>();
			m_uiController.Show<UISlicerScreen>();
		}
		public void EditorScreenHide()
		{
			m_uiController.Hide<UIEditorMenuScreen>();
			m_uiController.Hide<UISlicerScreen>();
		}

		public void SendMessageScreenShow(string message)
		{
			m_uiController.Show<UIMessageScreen>()
				.SetMessage(message);
		}

		private void UIMainScreenOnShow(UIScreen screen)
		{
			_resolver.Inject(screen);
		}

		private void UIEditorScreenOnShow(UIScreen screen)
		{
			UIEditorMenuScreen editorScreen = screen as UIEditorMenuScreen;

			_resolver.Inject(editorScreen);

			editorScreen.OnItemClick += OnTextureItemSelected;

			_editorScreen = editorScreen;
		}
		private void UIEditorScreenOnHide(UIScreen screen)
		{
			UIEditorMenuScreen editorScreen = screen as UIEditorMenuScreen;

			editorScreen.OnItemClick -= OnTextureItemSelected;

			_editorScreen = null;

			OnCloseEditorScreen?.Invoke();
		}
		
		private void UISlicerScreenOnShow(UIScreen screen)
		{
			UISlicerScreen slicerScreen = screen as UISlicerScreen;

			_resolver.Inject(slicerScreen);

			_slicerScreen = slicerScreen;
		}
		private void UISlicerScreenOnHide(UIScreen screen)
		{
			UISlicerScreen slicerScreen = screen as UISlicerScreen;

			_slicerScreen = null;
		}
	}
}