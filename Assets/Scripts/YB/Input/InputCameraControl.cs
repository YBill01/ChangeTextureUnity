using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace YBSlice.Input
{
	public class InputCameraControl : MonoBehaviour
	{
		public event Action<bool> OnMove;

		[SerializeField]
		private Vector2 rotationMultiplier = new Vector2(0.1f, 0.1f);
		[SerializeField]
		private float scrollMultiplier = 0.01f;

		[Space(10)]
		[SerializeField]
		private VCameraTarget cameraTarget;

		[SerializeField]
		private Bounds moveBounds;
		[SerializeField]
		private float moveMultiplierMin = 0.01f;
		[SerializeField]
		private float moveMultiplierMax = 0.1f;

		//[Space(10)]
		//[SerializeField]
		//private float zoomMin = 1.0f;
		//[SerializeField]
		//private float zoomMax = 5.0f;

		//private Camera _camera;
		[Space]
		public VCamera VCamera;

		private InputSystem_Actions _inputControls;

		private bool _isPointerLeftDown;

		private bool _isPointerMiddleDown;
		private bool _isPointerRightDown;
		private bool _isPointerDoubleLeftDown;

		private bool _isPointerOverGameObject;

		// UNITY EVENTS
		private void Awake()
		{
			_inputControls = new InputSystem_Actions();
			_inputControls.Enable();
		}

		private void Update()
		{
			_isPointerOverGameObject = EventSystem.current.IsPointerOverGameObject();
			//_camera.orthographicSize = zoomMin + (_vCamera.DistanceHelper * (zoomMax - zoomMin));
		}

		private void OnEnable()
		{
			_inputControls.Camera.PointerPositionDelta.performed += OnPointerPositionDelta;

			_inputControls.Camera.PointerLeftDown.performed += OnPointerLeftDown;
			_inputControls.Camera.PointerLeftDown.canceled += OnPointerLeftDown;
			_inputControls.Camera.PointerLeftDoubleClick.performed += OnPointerLeftDoubleClick;
			_inputControls.Camera.PointerMiddleDown.performed += OnPointerMiddleDown;
			_inputControls.Camera.PointerMiddleDown.canceled += OnPointerMiddleDown;
			_inputControls.Camera.PointerRightDown.performed += OnPointerRightDown;
			_inputControls.Camera.PointerRightDown.canceled += OnPointerRightDown;

			_inputControls.Camera.PointerScrollDelta.performed += OnPointerScroll;
		}
		private void OnDisable()
		{
			_inputControls.Camera.PointerPositionDelta.performed -= OnPointerPositionDelta;

			_inputControls.Camera.PointerLeftDown.performed -= OnPointerLeftDown;
			_inputControls.Camera.PointerLeftDown.canceled -= OnPointerLeftDown;
			_inputControls.Camera.PointerLeftDoubleClick.performed -= OnPointerLeftDoubleClick;
			_inputControls.Camera.PointerMiddleDown.performed -= OnPointerMiddleDown;
			_inputControls.Camera.PointerMiddleDown.canceled -= OnPointerMiddleDown;
			_inputControls.Camera.PointerRightDown.performed -= OnPointerRightDown;
			_inputControls.Camera.PointerRightDown.canceled -= OnPointerRightDown;

			_inputControls.Camera.PointerScrollDelta.performed -= OnPointerScroll;
		}

		private void OnPointerPositionDelta(InputAction.CallbackContext context)
		{
			Vector2 delta = context.ReadValue<Vector2>();

			if (_isPointerLeftDown)
			{
				VCamera.rotation.x -= delta.y * rotationMultiplier.y;
				VCamera.rotation.y += delta.x * rotationMultiplier.x;

				OnMove?.Invoke(true);
			}

			if (_isPointerMiddleDown || _isPointerRightDown)
			{
				Vector3 moveDirectionX = VCamera.transform.right * -delta.x;
				Vector3 moveDirectionY = VCamera.transform.up * -delta.y;

				cameraTarget.transform.position += (moveDirectionX + moveDirectionY) * (moveMultiplierMin + VCamera.distanceValue * (moveMultiplierMax - moveMultiplierMin));

				Vector3 position = cameraTarget.transform.position;
				cameraTarget.transform.position = new Vector3
				{
					x = Math.Clamp(position.x, moveBounds.min.x, moveBounds.max.x),
					y = Math.Clamp(position.y, moveBounds.min.y, moveBounds.max.x),
					z = Math.Clamp(position.z, moveBounds.min.z, moveBounds.max.z)
				};

				OnMove?.Invoke(true);
			}
		}
		private void OnPointerLeftDown(InputAction.CallbackContext context)
		{
			_isPointerLeftDown = false;

			if (_isPointerOverGameObject)
			{
				CheckOnMoveCancelInvoke();

				return;
			}

			_isPointerLeftDown = context.ReadValueAsButton();

			CheckOnMoveCancelInvoke();
		}
		private void OnPointerLeftDoubleClick(InputAction.CallbackContext context)
		{
			_isPointerDoubleLeftDown = false;

			if (_isPointerOverGameObject)
			{
				return;
			}

			_isPointerDoubleLeftDown = context.ReadValueAsButton();
		}
		private void OnPointerMiddleDown(InputAction.CallbackContext context)
		{
			_isPointerMiddleDown = false;

			if (_isPointerOverGameObject)
			{
				CheckOnMoveCancelInvoke();

				return;
			}

			_isPointerMiddleDown = context.ReadValueAsButton();

			CheckOnMoveCancelInvoke();
		}
		private void OnPointerRightDown(InputAction.CallbackContext context)
		{
			_isPointerRightDown = false;

			if (_isPointerOverGameObject)
			{
				CheckOnMoveCancelInvoke();

				return;
			}

			_isPointerRightDown = context.ReadValueAsButton();

			CheckOnMoveCancelInvoke();
		}
		private void OnPointerScroll(InputAction.CallbackContext context)
		{
			VCamera.distanceValue = Math.Clamp(VCamera.distanceValue - context.ReadValue<Vector2>().y * scrollMultiplier, 0, 1);
		}

		private void CheckOnMoveCancelInvoke()
		{
			if (!_isPointerLeftDown && !_isPointerMiddleDown && !_isPointerRightDown)
			{
				OnMove?.Invoke(false);
			}
		}

#if UNITY_EDITOR
		// UNITY GIZMOS
		private void OnDrawGizmosSelected()
		{
			Handles.color = new Color(0.75f, 0.25f, 0.125f, 1.0f);
			Handles.DrawWireCube(moveBounds.center, moveBounds.size);
			Handles.color = Color.white;
		}
#endif
	}
}