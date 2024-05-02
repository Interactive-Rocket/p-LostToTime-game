using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
	private bool canControl = true;

	[Header("Character Input Values")]
	[SerializeField]
	private Vector2 move;
	[SerializeField]
	private Vector2 look;
	[SerializeField]
	private bool jump;
	[SerializeField]
	private bool sprint;
	[SerializeField]
	private bool attackOne;
	[SerializeField]
	private bool attackOneHeld;
	[SerializeField]
	private bool attackTwo;
	[SerializeField]
	private bool attackTwoHeld;
	[SerializeField]
	private bool interact;
	[SerializeField]
	private bool grab;
	[SerializeField]
	private bool reloadScene = false;
	[SerializeField]
	private bool pause = false;

	[SerializeField]
	private bool deselect;

	[Header("Movement Settings")]
	[SerializeField]
	private bool analogMovement;

	[Header("Mouse Cursor Settings")]
	[SerializeField]
	private bool cursorLocked = true;
	[SerializeField]
	private bool cursorInputForLook = true;


	public void OnMove(InputAction.CallbackContext value)
	{
		MoveInput(canControl ? value.ReadValue<Vector2>() : Vector2.zero);
	}

	public void OnLook(InputAction.CallbackContext value)
	{
		if (cursorInputForLook)
		{
			LookInput(canControl ? value.ReadValue<Vector2>() : Vector2.zero);
		}
	}

	public void OnJump(InputAction.CallbackContext value)
	{
		JumpInput(canControl && value.action.triggered);
	}

	public void OnSprint(InputAction.CallbackContext value)
	{
		SprintInput(canControl && value.ReadValue<float>() == 1);
	}

	public void OnAttackOne(InputAction.CallbackContext value)
	{
		AttackOneInput(canControl && value.action.triggered);

		if (value.performed)
		{
			attackOneHeld = true;
		}

		if (value.canceled)
		{
			attackOneHeld = false;
		}
	}

	public void OnAttackTwo(InputAction.CallbackContext value)
	{
		AttackTwoInput(canControl && value.action.triggered);

		if (value.performed)
		{
			attackTwoHeld = true;
		}

		if (value.canceled)
		{
			attackTwoHeld = false;
		}
	}

	public void OnInteract(InputAction.CallbackContext value)
	{
		InteractInput(canControl && value.action.triggered);
	}

	public void OnGrab(InputAction.CallbackContext value)
	{
		GrabInput(canControl && value.action.triggered);
	}

	public void OnDeselect(InputAction.CallbackContext value)
	{
		DeselectInput(canControl && value.action.triggered);
	}

	public void OnReloadScene(InputAction.CallbackContext value)
	{
		// Always reload scene even if we can't control
		ReloadSceneInput(value.action.triggered);
	}

	public void OnPause(InputAction.CallbackContext value)
	{
		// Always allow opening the menu even if we can't control
		PauseInput(value.action.triggered);
	}

	// Getters
	public Vector2 GetMove()
	{
		return move;
	}
	public Vector2 GetLook()
	{
		return look;
	}

	public bool IsJumpping()
	{
		return jump;
	}
	//TODO: Improve jump
	public void SetJump(bool value) //Quick fix
	{
		jump = value;
	}
	public bool IsSprinting()
	{
		return sprint;
	}
	public bool IsAttackingOne()
	{
		return attackOne;
	}
	public bool IsHoldingAttackOne()
	{
		return attackOneHeld;
	}
	public bool IsAttackingTwo()
	{
		return attackTwo;
	}
	public bool IsHoldingAttackTwo()
	{
		return attackTwoHeld;
	}
	public bool IsInteracting()
	{
		return interact;
	}
	public bool IsGrabbing()
	{
		return grab;
	}

	public bool IsDeselecting()
	{
		return deselect;
	}

	public bool IsReloadingScene()
	{
		return reloadScene;
	}
	public bool IsPausing()
	{
		return pause;
	}

	public bool GetAnalogMovement()
	{
		return analogMovement;
	}

	//Event handlers
	public void MoveInput(Vector2 newMoveDirection)
	{
		move = newMoveDirection;
	}

	public void LookInput(Vector2 newLookDirection)
	{
		look = newLookDirection;
	}

	public void JumpInput(bool newJumpState)
	{
		jump = newJumpState;
	}

	public void SprintInput(bool newSprintState)
	{
		sprint = newSprintState;
	}

	public void AttackOneInput(bool newAttackOneState)
	{
		attackOne = newAttackOneState;
	}
	public void AttackTwoInput(bool newAttackTwoState)
	{
		attackTwo = newAttackTwoState;
	}

	public void InteractInput(bool newInteractState)
	{
		interact = newInteractState;
	}

	public void GrabInput(bool newGrabState)
	{
		grab = newGrabState;
	}

	public void DeselectInput(bool newDeselectState)
	{
		deselect = newDeselectState;
	}

	public void ReloadSceneInput(bool newSceneReloadState)
	{
		reloadScene = newSceneReloadState;
	}

	public void PauseInput(bool newPauseState)
	{
		pause = newPauseState;
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		SetCursorState(cursorLocked);
	}

	private void SetCursorState(bool newState)
	{
		Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
	}

	void Update()
	{
		if (PlayerManager.Instance != null) canControl = PlayerManager.Instance.controlEnabled;
	}
}