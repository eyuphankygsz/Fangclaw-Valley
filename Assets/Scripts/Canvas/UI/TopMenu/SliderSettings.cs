using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Zenject;

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
    public void SetSFX(Slider slider)
    {
        _mixer.SetFloat("SFX", Mathf.Log10(slider.value) * 20);
    }
    public void SetMusic(Slider slider)
    {
        _mixer.SetFloat("Music", Mathf.Log10(slider.value) * 20);
    }
}
