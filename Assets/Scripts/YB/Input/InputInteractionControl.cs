using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace YBSlice.Input
{
	public class InputInteractionControl : MonoBehaviour
	{
		public event Action<IInteraction> OnTouch;
		public event Action<IInteraction> OnClick;

		private IInteraction _selectedObject;

		private InputSystem_Actions _inputActions;

		private bool _isPointerOverGameObject;

		private bool _isDown;
		public bool IsDown => _isDown;

		private bool _isPerformedCanceled;
		public bool IsPerformedCanceled => _isPerformedCanceled;

		private void Awake()
		{
			_inputActions = new InputSystem_Actions();
		}

		private void OnEnable()
		{
			_inputActions.Enable();

			_inputActions.UI.Click.performed += OnClickButton;
			_inputActions.UI.MiddleClick.performed += OnMiddleClickButton;
			_inputActions.UI.RightClick.performed += OnRightClickButton;

			_isDown = false;
			_isPerformedCanceled = false;
		}
		private void OnDisable()
		{
			_inputActions.UI.Click.performed -= OnClickButton;
			_inputActions.UI.MiddleClick.performed -= OnMiddleClickButton;
			_inputActions.UI.RightClick.performed -= OnRightClickButton;

			_inputActions.Disable();
		}

		private void Update()
		{
			_isPointerOverGameObject = EventSystem.current.IsPointerOverGameObject();

			if (_isDown || _isPerformedCanceled)
			{
				return;
			}

			if (TryGetInteractionObject(out IInteraction interactionObject, out Vector3 point, out Vector3 normal))
			{
				if (!ReferenceEquals(interactionObject, _selectedObject))
				{
					_selectedObject?.OnTouchInternal(false);

					if (interactionObject.InteractionCondition.touchable)
					{
						_selectedObject = interactionObject;
						_selectedObject.OnTouchInternal(true);

						OnTouch?.Invoke(_selectedObject);
					}
				}

				_selectedObject?.OnTouchPointInternal(point, normal);
			}
			else if (_selectedObject != null)
			{
				_selectedObject.OnTouchInternal(false);
				_selectedObject = null;
			}
		}

		public void PerformedCancel()
		{
			PerformedClear();

			_isPerformedCanceled = true;
			_isDown = false;
		}
		public void PerformedClear()
		{
			if (_selectedObject != null)
			{
				_selectedObject.OnTouchInternal(false);
				_selectedObject = null;
			}
		}

		private bool TryGetInteractionObject(out IInteraction interactionObject, out Vector3 point, out Vector3 normal)
		{
			interactionObject = null;
			point = Vector3.zero;
			normal = Vector3.zero;

			Ray rayOrigin = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

			if (Physics.Raycast(rayOrigin, out RaycastHit hitInfo))
			{
				GameObject go = hitInfo.collider.gameObject;
				if (go.TryGetComponent(out interactionObject) && !_isPointerOverGameObject)
				{
					point = hitInfo.point;
					normal = hitInfo.normal;

					return interactionObject.InteractionEnable;
				}
			}

			return false;
		}

		private void OnClickButton(InputAction.CallbackContext context)
		{
			_isDown = context.ReadValueAsButton();

			if (_isPointerOverGameObject || _isPerformedCanceled)
			{
				_isPerformedCanceled = false;

				return;
			}

			if (!_isDown)
			{
				if (TryGetInteractionObject(out IInteraction interactionObject, out Vector3 point, out Vector3 normal))
				{
					if (_selectedObject != null && ReferenceEquals(interactionObject, _selectedObject) && _selectedObject.InteractionCondition.clickable)
					{
						_selectedObject?.OnClickInternal(point, normal);

						OnClick?.Invoke(_selectedObject);
					}
				}
				else
				{
					OnClick?.Invoke(null);
				}
			}
		}

		private void OnMiddleClickButton(InputAction.CallbackContext context)
		{
			_isPerformedCanceled = false;
		}
		private void OnRightClickButton(InputAction.CallbackContext context)
		{
			_isPerformedCanceled = false;
		}
	}
}