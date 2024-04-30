using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(PlayerSound))]
public class PlayerAttack : MonoBehaviour
{
	[Tooltip("Time required to pass before being able to attack again. Set to 0f to instantly attack again")]
	public float AttackTimeout = 0.2f;
	private float _attackTimeoutDelta;

	// selecting time entity
	private Ray _rayTimeEntitySelector; //Ray that hits time-rewindable objects
	/*
	SnapshotsPathTracer entitySnapshotPathTracer; //The line of previous positions
	private TargetableFocus targetableFocus;
	*/
	private InputManager _input;
	private GameObject _mainCamera;
	private PlayerSound _playerSound;
	//private GrabManager _grabManager;
	private TimeEntity currentRewindingEntity;

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
		_input = GetComponent<InputManager>();
		_playerSound = GetComponent<PlayerSound>();

		// reset our timeouts on start
		_attackTimeoutDelta = AttackTimeout;

	}
	
	private void Update()
	{
		Attacking();
	}

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
}