using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerAim : MonoBehaviour
{
	[Header("Player and Cinemachine")]
	[Tooltip("Rotation speed of the character")]
	public float RotationSpeed = 1.0f;
	[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
	public GameObject CinemachineCameraTarget;
	[Tooltip("How far in degrees can you move the camera up")]
	public float TopClamp = 90.0f;
	[Tooltip("How far in degrees can you move the camera down")]
	public float BottomClamp = -90.0f;

	// cinemachine
	private float _cinemachineTargetPitch;
	private float _rotationVelocity;
	private PlayerInput _playerInput;
	private InputManager _input;
	private GameObject _mainCamera;

	private const float _threshold = 0f;

	private bool IsCurrentDeviceMouse
	{
		get
		{
			return _playerInput.currentControlScheme == "KeyboardMouse";
		}
	}

	private void Awake()
	{
		// get a reference to our main camera
		if (_mainCamera == null)
		{
			_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		}

		_input = GetComponent<InputManager>();
		_playerInput = GetComponent<PlayerInput>();
	}

	private void LateUpdate()
	{
		CameraRotation();
	}

	private void CameraRotation()
	{
		// if there is an input
		if (_input.GetLook().sqrMagnitude >= _threshold)
		{
			//Don't multiply mouse input by Time.deltaTime
			float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
			float sensitivityMultiplier = (PlayerManager.Instance != null) ? PlayerManager.Instance.sensitivity : 1.0f;

			_cinemachineTargetPitch += _input.GetLook().y * RotationSpeed * deltaTimeMultiplier * sensitivityMultiplier;
			_rotationVelocity = _input.GetLook().x * RotationSpeed * deltaTimeMultiplier * sensitivityMultiplier;

			// clamp our pitch rotation
			_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

			// Update Cinemachine camera target pitch
			CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

			// rotate the player left and right
			transform.Rotate(Vector3.up * _rotationVelocity);
		}
	}

	private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
	{
		if (lfAngle < -360f) lfAngle += 360f;
		if (lfAngle > 360f) lfAngle -= 360f;
		return Mathf.Clamp(lfAngle, lfMin, lfMax);
	}
}