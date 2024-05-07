
using UnityEngine;
using UnityEngine.UI;

public class UISliders : MonoBehaviour
{
    [Tooltip("GameObject that contains the sensitivity slider.")]
    [SerializeField] private GameObject sensitivitySlider;
    [Tooltip("GameObject that contains the volume slider.)")]
    [SerializeField] private GameObject volumeSlider;
    [Tooltip("GameObject that contains the image for the volume, so it can be animated and cool :)")]
    [SerializeField] private GameObject volumeImageContainer;
    [Tooltip("GameObject that contains the image for the volume, so it can be animated and cool :)")]
    [SerializeField] private Sprite[] volumeSprites;

    private Slider _sensitivitySlider;
    private Slider _volumeSlider;
    private Image _volumeImage;

    void Awake()
    {
        if (sensitivitySlider != null) _sensitivitySlider = sensitivitySlider.GetComponent<Slider>();
        if (volumeSlider != null) _volumeSlider = volumeSlider.GetComponent<Slider>();
        if (volumeImageContainer != null) _volumeImage = volumeImageContainer.GetComponent<Image>();
    }

    void Start()
    {
        float newSensitivity = 1.0f;
        float newVolume = 1.0f;

        if (PlayerPrefs.HasKey("Sensitivity")) newSensitivity = PlayerPrefs.GetFloat("Sensitivity"); 
        if (PlayerPrefs.HasKey("Volume")) newVolume = PlayerPrefs.GetFloat("Volume");

        if (_sensitivitySlider != null) _sensitivitySlider.value = newSensitivity;
        if (_volumeSlider != null) _volumeSlider.value = newVolume;

        UpdateVolumeImage(newVolume);
    }

    public void UpdateSensitivity(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);
        if (PlayerManager.Instance != null) PlayerManager.Instance.sensitivity = value;
    }

    public void UpdateVolume(float value)
    {
        PlayerPrefs.SetFloat("Volume", value);
        if (PlayerManager.Instance != null) PlayerManager.Instance.volume = value;

        UpdateVolumeImage(value);
    }

    private void UpdateVolumeImage(float value)
    {
        if (_volumeImage == null || volumeSprites.Length == 0) return;
        if (value < 0f || value > 1.0f) return;

        int index = (int)(value * (volumeSprites.Length - 1));
        _volumeImage.sprite = volumeSprites[index];
    }

    public void SavePlayerPrefs()
    {
        PlayerPrefs.Save();
    }
}
