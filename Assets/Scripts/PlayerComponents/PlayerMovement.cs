﻿using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(PlayerSound))]
public class PlayerMovement : MonoBehaviour
{
	[Header("Player")]
	[Tooltip("Move speed of the character in m/s")]
	[SerializeField]
	public float MoveSpeed = 4.0f;
	[Tooltip("Sprint speed of the character in m/s")]
	public float SprintSpeed = 6.0f;
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

	[Header("Player Grounded")]
	[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
	public bool Grounded = true;
	[Tooltip("Useful for rough ground")]
	public float GroundedOffset = -0.14f;
	[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
	public float GroundedRadius = 0.5f;
	[Tooltip("What layers the character uses as ground")]
	public LayerMask GroundLayers;

	// player
	private float _speed;
	private float _verticalVelocity;
	private float _terminalVelocity = 53.0f;

	// timeout deltatime
	private float _jumpTimeoutDelta;
	private float _fallTimeoutDelta;

	private CharacterController _controller;
	private InputManager _input;
	private PlayerSound _playerSound;
	private MovingPlatform _movingPlatform;
	private Vector3 _positionRelativeToPlatform;
	private Vector3 _platformVelocity;
	private Vector3 _previousPlatformVelocity;
	private bool _landSoundPlayed = false;
	private int _platformLinger = 0;

	private void Awake()
	{
		_controller = GetComponent<CharacterController>();
		_input = GetComponent<InputManager>();
		_playerSound = GetComponent<PlayerSound>();
	}

	private void Start()
	{
		// reset our timeouts on start
		_jumpTimeoutDelta = JumpTimeout;
		_fallTimeoutDelta = FallTimeout;
	}

	private void Update()
	{
		JumpAndGravity();
		GroundedCheck();
		FollowPlatform();
		Move();
	}

	private void GroundedCheck()
	{
		// set sphere position, with offset
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
		Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
	}
	private void FollowPlatform(){
		_platformLinger--;
		if(_platformLinger==0){
			removeMovingPlatform(_movingPlatform);
		}
		if(_movingPlatform == null){
			_platformVelocity = Vector3.zero;
			return;
		}

        Vector3 angle_change = _movingPlatform._angle_change;
        Vector3 pos_diff = _movingPlatform._pos_diff;
		Vector3 angular_movement = Quaternion.Euler(angle_change) * _positionRelativeToPlatform - _positionRelativeToPlatform;
		Vector3 total_diff = pos_diff + angular_movement;
		if(_movingPlatform.gameObject.GetComponent<TimeEntity>().IsRewinding){
			_platformVelocity = total_diff / (Time.time - _movingPlatform._last_time);
		}
		else
		{
			_platformVelocity = Vector3.zero;
		}
		_positionRelativeToPlatform = transform.position - _movingPlatform._position;
		_movingPlatform.UpdatePosition();
		if(_platformLinger>0 && _movingPlatform.ManualColliderCheck(_controller)){
			//Debug.Log("platform regained");
			_platformLinger = 0;
		}
		else if (!_movingPlatform.ManualColliderCheck(_controller)){
			//Debug.Log("platform lost");
			removeMovingPlatform(_movingPlatform);
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
		float currentHorizontalSpeed = new Vector3(_controller.velocity.x- _previousPlatformVelocity.x, 0.0f, _controller.velocity.z- _previousPlatformVelocity.z).magnitude;
		
		_previousPlatformVelocity = _platformVelocity;
		float speedOffset = 0.1f;
		float inputMagnitude = _input.GetAnalogMovement() ? _input.GetMove().magnitude : 1f;

		// accelerate or decelerate to target speed
		if (currentHorizontalSpeed < targetSpeed - speedOffset)
		{
			// creates curved result rather than a linear one giving a more organic speed change
			// note T in Lerp is clamped, so we don't need to clamp our speed
			_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

			// round speed to 3 decimal places
			_speed = Mathf.Round(_speed * 1000f) / 1000f;
		}
		else 
		{
			if (currentHorizontalSpeed > targetSpeed + speedOffset)
				Debug.Log("capping speed");
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
		Vector3 finalMovement = (inputDirection.normalized * _speed + new Vector3(0, _verticalVelocity, 0) + _platformVelocity) * Time.deltaTime;
		_controller.Move(finalMovement);

		if (Grounded && inputDirection.magnitude > 0 && currentHorizontalSpeed / MoveSpeed >= 1)
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

			// Ensure the landing sound is played once when grounded again
			if (!_landSoundPlayed)
			{
				_playerSound.PlayLandSound();
				_landSoundPlayed = true;
			}

			// Jump
			if (_input.IsJumpping() && _jumpTimeoutDelta <= 0.0f)
			{
				// the square root of H * -2 * G = how much velocity needed to reach desired height
				_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
				_jumpTimeoutDelta = JumpTimeout;
				_playerSound.PlayJumpSound();
				// _landSoundPlayed = false; // Remove this line
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

			// _landSoundPlayed = false; // Remove this line
		}

		// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
		if (_verticalVelocity < _terminalVelocity)
		{
			_verticalVelocity += Gravity * Time.deltaTime;
		}

		// Reset _landSoundPlayed flag when falling below 0 velocity
		if (!Grounded && _verticalVelocity < 0.0f && _fallTimeoutDelta < 0)
		{
			_landSoundPlayed = false;
		}
	}

	public void setMovingPlatform(MovingPlatform movingPlatform){
		if (_movingPlatform == movingPlatform){
			_platformLinger = 0;
		}
		else if (_movingPlatform != null && _platformLinger>0){
			removeMovingPlatform(_movingPlatform);
		}

		if (_movingPlatform == null){
			_movingPlatform = movingPlatform;
			_movingPlatform.UpdatePosition();
			_movingPlatform.UpdatePosition();
			_positionRelativeToPlatform = transform.position - _movingPlatform._position;
		}
	}

	
	public void queueRemoveMovingPlatform(MovingPlatform movingPlatform){
		if (_movingPlatform == movingPlatform){
			_platformLinger = 4;
		}
	}
	
	public void removeMovingPlatform(MovingPlatform movingPlatform){
		if (_movingPlatform == movingPlatform){
			_platformLinger = 0;
			_movingPlatform = null;
			_platformVelocity = Vector3.zero;
			_positionRelativeToPlatform = Vector3.zero;
		}
	}

}