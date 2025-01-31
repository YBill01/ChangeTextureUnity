using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace YBSlice.Input
{
	public class InputUIControl : MonoBehaviour
	{
		public event Action OnClick;
		public event Action OnSubmit;
		public event Action OnCancel;
		public event Action<bool> OnShift;
		
		private bool _isShift;
		public bool IsShift => _isShift;

		private InputSystem_Actions _inputActions;

		private void Awake()
		{
			_inputActions = new InputSystem_Actions();
		}

		private void OnEnable()
		{
			_inputActions.Enable();

			_inputActions.UI.Click.performed += OnClickButton;

			_inputActions.UI.Submit.performed += OnSubmitButton;
			_inputActions.UI.Submit.canceled += OnSubmitButton;

			_inputActions.UI.Cancel.performed += OnCancelButton;
			_inputActions.UI.Cancel.canceled += OnCancelButton;

			_inputActions.UI.LShift.performed += OnLShiftButton;
			_inputActions.UI.LShift.canceled += OnLShiftButton;

		}
		private void OnDisable()
		{
			_inputActions.UI.Click.performed -= OnClickButton;

			_inputActions.UI.Submit.performed -= OnSubmitButton;
			_inputActions.UI.Submit.canceled -= OnSubmitButton;

			_inputActions.UI.Cancel.performed -= OnCancelButton;
			_inputActions.UI.Cancel.canceled -= OnCancelButton;

			_inputActions.UI.LShift.performed -= OnLShiftButton;
			_inputActions.UI.LShift.canceled -= OnLShiftButton;

			_inputActions.Disable();
		}

		private void OnClickButton(InputAction.CallbackContext context)
		{
			bool value = context.ReadValueAsButton();

			if (!value)
			{
				OnClick?.Invoke();
			}
		}
		private void OnSubmitButton(InputAction.CallbackContext context)
		{
			bool value = context.ReadValueAsButton();

			if (value)
			{
				OnSubmit?.Invoke();
			}
		}
		private void OnCancelButton(InputAction.CallbackContext context)
		{
			bool value = context.ReadValueAsButton();

			if (!value)
			{
				OnCancel?.Invoke();
			}
		}
		private void OnLShiftButton(InputAction.CallbackContext context)
		{
			_isShift = context.ReadValueAsButton();

			OnShift?.Invoke(_isShift);
		}
	}
}