using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.PostProcessing;

public class GraphicsManager : MonoBehaviour, IDataPersistence
{
    [Header("Graphics Settings")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private TMP_Text brightnessTextValue;
    [SerializeField] private float brightnessValue = 0.5f;
    PostProcessingManager postProcessingManager;

    [Header("Resolution Dropdown")]
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    [Header("Quality Settings")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullScreenToggle;

    [Header("Default Settings")]
    [SerializeField] private Toggle defaultToggle;

    private void Awake()
    {
        postProcessingManager = GameObject.Find("PostProcessing").GetComponent<PostProcessingManager>();

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void LoadData(GameData data)
    {
        if (data != null)
        {
            resolutionDropdown.value = data.resolutionIndex;
            SetResolution();

            qualityDropdown.value = data.qualityLevel;
            QualitySettings.SetQualityLevel(data.qualityLevel);

            fullScreenToggle.isOn = data.fullScreen;
            SetFullScreen();

            brightnessSlider.value = data.brightness;
            SetBrightness();

            defaultToggle.isOn = data.isDefaultToggle;
        }
        else
        {
            defaultToggle.isOn = true;
            SetDefaultValue();
        }
    }

    public void SaveData(GameData data)
    {
        data.resolutionIndex = resolutionDropdown.value;
        data.qualityLevel = qualityDropdown.value;
        data.fullScreen = fullScreenToggle.isOn;
        data.brightness = brightnessSlider.value;
        data.isDefaultToggle = defaultToggle.isOn;
    }

    public void SetResolution()
    {
        int resolutionIndex = resolutionDropdown.value;
        Resolution resolution = resolutions[resolutionIndex];
        /*Debug.Log(resolutions[resolutionIndex].width + " x " + resolutions[resolutionIndex].height +
            "\n" + resolutionDropdown.value);*/
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        /*Debug.Log(Screen.currentResolution.width + " x " + Screen.currentResolution.height + 
            "\n" + resolutionDropdown.value + Screen.fullScreen);*/

        if (resolutionIndex != 0)
        {
            defaultToggle.isOn = false;
        }
    }

    public void SetBrightness()
    {
        postProcessingManager.SetExposure(brightnessSlider.value * postProcessingManager.maxLuminanceValue);
        brightnessTextValue.text = brightnessSlider.value.ToString("0.0");

        if (brightnessSlider.value != brightnessValue)
        {
            defaultToggle.isOn = false;
        }
    }

    public void SetFullScreen()
    {
        if (fullScreenToggle.isOn)
        {
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreen = false;
            defaultToggle.isOn = false;
        }
    }

    public void SetQuality()
    {
        int qualityLevel = qualityDropdown.value;
        QualitySettings.SetQualityLevel(qualityLevel, true);
        //Debug.Log(QualitySettings.GetQualityLevel());

        if (qualityLevel != 0)
        {
            defaultToggle.isOn = false;
        }
    }

    public void SetDefaultValue()
    {
        if (defaultToggle.isOn)
        {
            qualityDropdown.value = 0;
            QualitySettings.SetQualityLevel(0);

            fullScreenToggle.isOn = true;
            Screen.fullScreen = true;

            resolutionDropdown.value = 0;
            Screen.SetResolution(resolutions[0].width, resolutions[0].height, Screen.fullScreen);

            brightnessSlider.value = brightnessValue;
            brightnessTextValue.text = brightnessValue.ToString("0.0");
        }
    }
}
