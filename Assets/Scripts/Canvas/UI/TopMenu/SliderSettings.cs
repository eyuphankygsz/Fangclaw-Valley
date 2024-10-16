using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SliderSettings : MonoBehaviour
{
    [SerializeField]
    private PlayerCamera _camera;
    [SerializeField]
    private AudioMixer _mixer;

    public void SetMouseSensitivity(Slider slider)
    {
        _camera.SetSensitivity(slider.value);
    }
    
    public void SetMusic(Slider slider)
    {
        _mixer.SetFloat("Music", Mathf.Log10(slider.value) * 20);
    }
}
