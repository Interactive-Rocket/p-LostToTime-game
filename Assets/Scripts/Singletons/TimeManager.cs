using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    /* Currently this singleton only does anything in the case where time
    * reversal is consistent across the entire level, it might still be
    * worth having a reference to every object which has a time structure
    * contained. Or as we decided a reference to every object which is
    * being rewound.
    */
    private float _timeScale = 1.0f;

    public float TimeScale
    {
        get => _timeScale;
        set
        {
            _timeScale = value;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}
