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

		// reset our timeouts on start
		_attackTimeoutDelta = AttackTimeout;

	}

	private void Update()
	{
		Attacking();
	}

	private void Attacking()
	{
		if (!_input.IsAttackingOne())
		{
			RewindManager.Instance.RewindObjects(false);
		}
		else if (_input.IsAttackingOne())
		{
			_attackTimeoutDelta = AttackTimeout;
			RewindManager.Instance.RewindObjects(true);
			// RewindManager.Instance.DeselectAll();
		}

		if (_input.IsAttackingTwo())
		{

			Vector3 screenCenterPoint = new Vector3(Screen.width / 2, Screen.height / 2, 0); //The ray is shoot from the center of the screen
			_rayTimeEntitySelector = Camera.main.ScreenPointToRay(screenCenterPoint);

			if (Physics.Raycast(_rayTimeEntitySelector, out RaycastHit hit))
			{
				GameObject hitObject = hit.collider.gameObject;
				Debug.Log(hitObject.name);
				TimeEntity timeEntity = hitObject.GetComponentInParent<TimeEntity>();
				if (timeEntity != null)
				{
					// RewindManager.Instance.SelectObject(hit.collider.gameObject);
					if (RewindManager.Instance != null)
					{
						if (RewindManager.Instance.ObjectIsSelected(hit.collider.gameObject))
						{
							RewindManager.Instance.DeselectObject(hit.collider.gameObject);
						}
						else
						{
							RewindManager.Instance.SelectObject(hit.collider.gameObject);
						}
					}
					else
					{
						Debug.LogError("RewindManager.Instance is null");
					}

				}
			}

			_input.AttackTwoInput(false);
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
}