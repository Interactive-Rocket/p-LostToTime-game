using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class FirstPersonController : MonoBehaviour
{
	[Header("Player")]
	[Tooltip("Move speed of the character in m/s")]
	[SerializeField]
	public float MoveSpeed = 4.0f;
	[Tooltip("Sprint speed of the character in m/s")]
	public float SprintSpeed = 6.0f;
	[Tooltip("Rotation speed of the character")]
	public float RotationSpeed = 1.0f;
	[Tooltip("Acceleration and deceleration")]
	public float SpeedChangeRate = 10.0f;

	[Space(10)]
	[Tooltip("The height the player can jump")]
	public float JumpHeight = 1.2f;
	[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
	public float Gravity = -15.0f;

	[Space(10)]
	[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
	public float JumpTimeout = 0.1f;
	[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
	public float FallTimeout = 0.15f;
	[Tooltip("Time required to pass before being able to attack again. Set to 0f to instantly attack again")]
	public float AttackTimeout = 0.2f;

	[Header("Player Grounded")]
	[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
	public bool Grounded = true;
	[Tooltip("Useful for rough ground")]
	public float GroundedOffset = -0.14f;
	[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
	public float GroundedRadius = 0.5f;
	[Tooltip("What layers the character uses as ground")]
	public LayerMask GroundLayers;

	[Header("Cinemachine")]
	[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
	public GameObject CinemachineCameraTarget;
	[Tooltip("How far in degrees can you move the camera up")]
	public float TopClamp = 90.0f;
	[Tooltip("How far in degrees can you move the camera down")]
	public float BottomClamp = -90.0f;

	// cinemachine
	private float _cinemachineTargetPitch;

	// player
	private float _speed;
	private float _rotationVelocity;
	private float _verticalVelocity;
	private float _terminalVelocity = 53.0f;

	// timeout deltatime
	private float _jumpTimeoutDelta;
	private float _fallTimeoutDelta;
	private float _attackTimeoutDelta;

	// selecting time entity
	private Ray _rayTimeEntitySelector; //Ray that hits time-rewindable objects
	/*
	SnapshotsPathTracer entitySnapshotPathTracer; //The line of previous positions
	private TargetableFocus targetableFocus;
	*/
	private PlayerInput _playerInput;
	private CharacterController _controller;
	private InputManager _input;
	private GameObject _mainCamera;
	private PlayerInteract _interactManager;
	private PlayerSound _playerSound;
	//private GrabManager _grabManager;
	private TimeEntity currentRewindingEntity;


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
	}

	private void Start()
	{
		_controller = GetComponent<CharacterController>();
		_input = GetComponent<InputManager>();
		_playerInput = GetComponent<PlayerInput>();
		_interactManager = GetComponent<PlayerInteract>();
		_playerSound = GetComponent<PlayerSound>();
		/*
		_grabManager = GetComponent<GrabManager>();
		entitySnapshotPathTracer = GetComponent<SnapshotsPathTracer>();
		targetableFocus = GetComponent<TargetableFocus>();
		*/

		// reset our timeouts on start
		_jumpTimeoutDelta = JumpTimeout;
		_fallTimeoutDelta = FallTimeout;
		_attackTimeoutDelta = AttackTimeout;

	}
	float timer = 0.0f;
	private void Update()
	{
		JumpAndGravity();
		GroundedCheck();
		Move();
		Attacking();
		Interacting();
		Grabbing();
		//Debug.Log("Timer: "+timer+" | Time.time: "+Time.time);
		// PreviewingTime();
	}

	private void LateUpdate()
	{
		CameraRotation();
	}

	private void GroundedCheck()
	{
		// set sphere position, with offset
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
		Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
	}

	private void CameraRotation()
	{
		// if there is an input
		if (_input.GetLook().sqrMagnitude >= _threshold)
		{
			//Don't multiply mouse input by Time.deltaTime
			float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

			_cinemachineTargetPitch += _input.GetLook().y * RotationSpeed * deltaTimeMultiplier;
			_rotationVelocity = _input.GetLook().x * RotationSpeed * deltaTimeMultiplier;

			// clamp our pitch rotation
			_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

			// Update Cinemachine camera target pitch
			CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

			// rotate the player left and right
			transform.Rotate(Vector3.up * _rotationVelocity);
		}
	}

	private void Move()
	{
		// set target speed based on move speed, sprint speed and if sprint is pressed
		float targetSpeed = _input.IsSprinting() ? SprintSpeed : MoveSpeed;

		// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

		// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is no input, set the target speed to 0
		if (_input.GetMove() == Vector2.zero) targetSpeed = 0.0f;

		// a reference to the players current horizontal velocity
		float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

		float speedOffset = 0.1f;
		float inputMagnitude = _input.GetAnalogMovement() ? _input.GetMove().magnitude : 1f;

		// accelerate or decelerate to target speed
		if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
		{
			// creates curved result rather than a linear one giving a more organic speed change
			// note T in Lerp is clamped, so we don't need to clamp our speed
			_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

			// round speed to 3 decimal places
			_speed = Mathf.Round(_speed * 1000f) / 1000f;
		}
		else
		{
			_speed = targetSpeed;
		}

		// normalise input direction
		Vector3 inputDirection = new Vector3(_input.GetMove().x, 0.0f, _input.GetMove().y).normalized;

		// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is a move input rotate player when the player is moving
		if (_input.GetMove() != Vector2.zero)
		{
			// move
			inputDirection = transform.right * _input.GetMove().x + transform.forward * _input.GetMove().y;
		}

		// move the player
		_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

		if (Grounded && inputDirection.magnitude > 0 && _speed / MoveSpeed >= 1)
		{
			_playerSound.PlayFootstep(_speed / MoveSpeed);
		}
		else
			_playerSound.StopFootsteps();

	}

	private void JumpAndGravity()
	{
		if (Grounded)
		{
			// reset the fall timeout timer
			_fallTimeoutDelta = FallTimeout;

			// stop our velocity dropping infinitely when grounded
			if (_verticalVelocity < 0.0f)
			{
				_verticalVelocity = -2f;
			}

			// Jump
			if (_input.IsJumpping() && _jumpTimeoutDelta <= 0.0f)
			{
				// the square root of H * -2 * G = how much velocity needed to reach desired height
				_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
			}

			// jump timeout
			if (_jumpTimeoutDelta >= 0.0f)
			{
				_jumpTimeoutDelta -= Time.deltaTime;
			}
		}
		else
		{
			// reset the jump timeout timer
			_jumpTimeoutDelta = JumpTimeout;

			// fall timeout
			if (_fallTimeoutDelta >= 0.0f)
			{
				_fallTimeoutDelta -= Time.deltaTime;
			}

			// if we are not grounded, do not jump
			_input.SetJump(false);
		}

		// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
		if (_verticalVelocity < _terminalVelocity)
		{
			_verticalVelocity += Gravity * Time.deltaTime;
		}
	}

	private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
	{
		if (lfAngle < -360f) lfAngle += 360f;
		if (lfAngle > 360f) lfAngle -= 360f;
		return Mathf.Clamp(lfAngle, lfMin, lfMax);
	}

	private void OnDrawGizmosSelected()
	{
		Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
		Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

		if (Grounded) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;

		// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
		Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
	}

	// The actual logic for attacking should be moved to a different class
	// This should only contain a reference to the attack action, otherwise 
	// This script will become entirely unwieldy
	private void Attacking()
	{
		if (!_input.IsAttackingOne() && currentRewindingEntity != null)
		{
			currentRewindingEntity.Rewind(false);
			currentRewindingEntity = null;
		}

		else if (_input.IsAttackingOne() && currentRewindingEntity != null)
		{
			currentRewindingEntity.Rewind(true);
		}

		else if (_input.IsAttackingOne())
		{
			//Debug.Log("Do Attack One!");
			// reset the attack timeout timer
			_attackTimeoutDelta = AttackTimeout;
			Vector3 screenCenterPoint = new Vector3(Screen.width / 2, Screen.height / 2, 0); //The ray is shoot from the center of the screen
			_rayTimeEntitySelector = Camera.main.ScreenPointToRay(screenCenterPoint);
			Debug.DrawRay(_rayTimeEntitySelector.origin, _rayTimeEntitySelector.direction * 10, Color.yellow);

			if (Physics.Raycast(_rayTimeEntitySelector, out RaycastHit hit))
			{
				GameObject hitObject = hit.collider.gameObject;
				//Debug.Log(hitObject.name);
				TimeEntity timeEntity = hitObject.GetComponentInParent<TimeEntity>();

				// Change to be more safe in case the hit object does not have a TimeEntity component
				if (timeEntity != null)
				{
					currentRewindingEntity = timeEntity;
					var snapshots = timeEntity.GetSnapshots();
					if (snapshots != null) //if has recorded data, draw previous positions
					{
						_playerSound.PlayAbilitySound();
						//entitySnapshotPathTracer.SetEntitySnapshots(snapshots);
					}

					timeEntity.Rewind(true);
					// Do we want this to also work for the old not TimeEntity?
					// These really shouldn't be separate classes.
				}
				else
				{

					//Debug.Log("No TimeEntity component found.");
				}
			}

			else
			{
				//Debug.Log("Not hit");
			}

		}

		if (_input.IsAttackingTwo())
		{
			//Debug.Log("Do Attack Two!");
		}
		else
		{
			//TimeManager.Instance.TimeScale = 1.0f;
		}

		if (_attackTimeoutDelta >= 0.0f)
		{
			_attackTimeoutDelta -= Time.deltaTime;
		}

	}
	private float nextLog = 0.0f;

	void Interacting()
	{
		if (_input.IsInteracting())
		{
			_interactManager.SendInteract();
		}
	}

	void Grabbing()
	{
		//_grabManager.UpdateGrabStatus(_input.IsGrabbing());
	}


}