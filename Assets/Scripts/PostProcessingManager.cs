using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingManager : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;
    public float maxLuminanceValue;
    private void Awake()
    {
        if (postProcessVolume.profile.TryGetSettings(out AutoExposure exposure))
        {
            exposure.maxLuminance.value = maxLuminanceValue;
            exposure.minLuminance.value = 0;
            exposure.keyValue.value = 0;
        }
    }

    public void SetExposure(float exposureValue)
    {
        postProcessVolume.profile.TryGetSettings(out AutoExposure exposure);
        exposure.keyValue.value = exposureValue;
        //Debug.Log("Exposure is " + exposure.keyValue.value);
    }
}
