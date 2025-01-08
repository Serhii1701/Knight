using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour, IDataPersistence
{
    readonly private float defaultValue = 0.5f;
    public float volumeEffectsValue;

    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI valueText;
    [SerializeField] Toggle defaultToggle;

    [SerializeField] Slider sliderEffects;
    [SerializeField] TextMeshProUGUI valueEffectsText;
    [SerializeField] Toggle defaultEffectsToggle;

    AudioSource audioSource;

    private void Awake()
    {
        audioSource = Camera.main.GetComponent<AudioSource>();
        audioSource.volume = slider.value;
    }

    public void LoadData(GameData data)
    {
        if (data != null)
        {
            audioSource.volume = data.audioVolumeValue;
            slider.value = data.audioVolumeValue;
            valueText.text = data.audioVolumeValue.ToString("0.0");

            volumeEffectsValue = data.audioVolumeEffectsValue;
            sliderEffects.value = data.audioVolumeEffectsValue;
            valueEffectsText.text = data.audioVolumeEffectsValue.ToString("0.0");
        }
    }

    public void SaveData(GameData data)
    {
        data.audioVolumeValue = audioSource.volume;
        data.audioVolumeEffectsValue = sliderEffects.value;
    }

    private void SetVolume(Slider slider, Toggle defaultToggle, AudioSource audioSource, TextMeshProUGUI valueText)
    {
        valueText.text = slider.value.ToString("0.0");
        audioSource.volume = slider.value;

        if (slider.value != defaultValue)
        {
            defaultToggle.isOn = false;
        }
    }

    private void SetVolume(Slider slider, Toggle defaultToggle, TextMeshProUGUI valueText)
    {
        valueText.text = slider.value.ToString("0.0");
        volumeEffectsValue = slider.value;

        if (slider.value != defaultValue)
        {
            defaultToggle.isOn = false;
        }
    }

    private void SetDefaultValue(Slider slider, Toggle defaultToggle, AudioSource audioSource, TextMeshProUGUI valueText)
    {
        if (defaultToggle.isOn)
        {
            slider.value = defaultValue;
            audioSource.volume = defaultValue;
            valueText.text = defaultValue.ToString("0.0");
        } 
    }

    private void SetDefaultValue(Slider slider, Toggle defaultToggle, TextMeshProUGUI valueText)
    {
        if (defaultToggle.isOn)
        {
            slider.value = defaultValue;
            volumeEffectsValue = defaultValue;
            valueText.text = defaultValue.ToString("0.0");
        }
    }

    public void OnMusicSliderValueChanged()
    {
        SetVolume(slider, defaultToggle, audioSource, valueText);
    }

    public void OnEffectsSliderValueChanged()
    {
        SetVolume(sliderEffects, defaultEffectsToggle, valueEffectsText);
    }

    public void SetMusicDefaultValue()
    {
        SetDefaultValue(slider, defaultToggle, audioSource, valueText);
    }

    public void SetEffectsDefaultValue()
    {
        SetDefaultValue(sliderEffects, defaultEffectsToggle, valueEffectsText);
    }
}
