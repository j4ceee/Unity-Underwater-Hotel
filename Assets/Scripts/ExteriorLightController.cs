using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LightColourMode
{
    GreenBlue,
    PurpleRed
}

public class ExteriorLightController : MonoBehaviour
{
    public Light[] exteriorLights;

    public Color greenColour = new(0.294f, 0.906f, 0.345f);
    public Color blueColour = new(0.294f, 0.631f, 0.906f);

    public Color purpleColour = new(0.522f, 0.404f, 1f);
    public Color redColour = new(0.678f, 0.078f, 0.455f);

    public LightColourMode lightColourMode = LightColourMode.GreenBlue;

    private bool _isNight = false;

    [Tooltip("Time in seconds between color changes")]
    public float cycleTime = 10f;

    public void Start()
    {
        SwitchToNightSetting(false);
    }

    private Dictionary<int, Coroutine> _lightCoroutines = new Dictionary<int, Coroutine>();

    private IEnumerator LerpColour(int lightInt, Color targetColour, float duration)
    {
        Light lightInst = exteriorLights[lightInt];

        Color startColour = lightInst.color;
        float time = 0f;

        while (time < duration)
        {
            lightInst.color = Color.Lerp(startColour, targetColour, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        // end couroutine & remove from dictionary
        _lightCoroutines.Remove(lightInt);
    }

    public void SwitchToNightSetting(bool isNight)
    {
        _isNight = isNight;

        if (isNight)
        {
            foreach (var lightInst in exteriorLights)
            {
                lightInst.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (var lightInst in exteriorLights)
            {
                lightInst.gameObject.SetActive(false);
            }
        }
    }

    public void SetLightColourMode(LightColourMode mode)
    {
        lightColourMode = mode;
    }

    void Update()
    {
        if (_isNight && _lightCoroutines.Count < 4)
        {
            var lightInt = UnityEngine.Random.Range(0, exteriorLights.Length);

            Color targetColour;
            if (lightColourMode == LightColourMode.GreenBlue)
            {
                targetColour = UnityEngine.Random.value > 0.5f ? greenColour : blueColour;
            }
            else
            {
                targetColour = UnityEngine.Random.value > 0.5f ? purpleColour : redColour;
            }

            if (!_lightCoroutines.ContainsKey(lightInt))
            {
                _lightCoroutines[lightInt] = StartCoroutine(LerpColour(lightInt, targetColour, cycleTime));
            }
        }
    }
}
