using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Serialization;

[System.Serializable]
public class WeatherType
{
    public string weatherName = "Sunny";

    [Header("Ocean Settings")]
    [Range(0f, 1f)]
    public float waveWeight = 0.313f;
    [Range(0f, 1f)]
    public float windTurbulence = 0.087f;

    // colours
    public Color scatteringColor = new(16, 63, 69);
    public Color scatteringColorShadow = new(2, 92, 168);
    public Color scatteringColorShallow = new(2, 92, 168);
    public Color sssColor = new(2, 92, 168);

    [Range(0.01f, 16f)]
    public float sssSunFalloff = 5.55f;
    [Range(0f, 1f)]
    public float normalStrength = 0.225f;

    [Range(0.01f, 1f)]
    public float depthFog = 0.243f;
    [Range(0f, 10f)]
    public float causticsStrength = 3.2f;

    [Header("Cloud Settings")]
    [Range(0f, 1f)]
    public float densityMultiplier = .4f;

    public AnimationCurve densityCurve;
    [Range(0f, 1f)]
    public float shapeFactor = .95f;
    public AnimationCurve ambientOcclusion;
    public float bottomAltitude = 3000f;
    public float altitudeRange = 1000f;
}

public class WeatherController : MonoBehaviour
{
    public WaterController oceanController;
    public Volume skyVolume;
    private VolumetricClouds _clouds;

    // array of weather types
    public List<WeatherType> weatherTypes = new List<WeatherType>();

    public int defaultWeather = 0;
    private int _currentWeather;
    private bool _isChangingWeather = false;

    // Start is called before the first frame update
    void Start()
    {
        skyVolume.profile.TryGet(out _clouds);
        ChangeWeather(defaultWeather);
        _currentWeather = defaultWeather;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Change current weather")]
    public void SetDefaultWeather()
    {
        skyVolume.profile.TryGet(out _clouds);
        ChangeWeather(defaultWeather);
    }

    private void ChangeWeather(int weatherIndex)
    {
        _currentWeather = weatherIndex;

        // change clouds
        _clouds.densityMultiplier.value = weatherTypes[weatherIndex].densityMultiplier;
        _clouds.densityCurve.value = weatherTypes[weatherIndex].densityCurve;
        _clouds.shapeFactor.value = weatherTypes[weatherIndex].shapeFactor;
        _clouds.ambientOcclusionCurve.value = weatherTypes[weatherIndex].ambientOcclusion;
        _clouds.bottomAltitude.value = weatherTypes[weatherIndex].bottomAltitude;
        _clouds.altitudeRange.value = weatherTypes[weatherIndex].altitudeRange;

        // change ocean
        oceanController.SetWaveStrength(weatherTypes[weatherIndex].waveWeight);
        oceanController.SetWaveTurbulence(weatherTypes[weatherIndex].windTurbulence);
        oceanController.SetColors(weatherTypes[weatherIndex].scatteringColor, weatherTypes[weatherIndex].scatteringColorShadow, weatherTypes[weatherIndex].scatteringColorShallow, weatherTypes[weatherIndex].sssColor);
        oceanController.SetSunFalloff(weatherTypes[weatherIndex].sssSunFalloff);
        oceanController.SetNormalStrength(weatherTypes[weatherIndex].normalStrength);

        oceanController.UpdateCausticsStrength(weatherTypes[weatherIndex].causticsStrength);
        oceanController.UpdateOceanFogDensity(weatherTypes[weatherIndex].depthFog);
        oceanController.depthFogDefault = weatherTypes[weatherIndex].depthFog;
        oceanController.strongestCaustics = weatherTypes[weatherIndex].causticsStrength;
    }

    private IEnumerator ChangeWeatherGradually(int newWeather)
    {
        _isChangingWeather = true;

        float duration = 10f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            oceanController.SetWaveStrength(Mathf.Lerp(weatherTypes[_currentWeather].waveWeight, weatherTypes[newWeather].waveWeight, t));
            oceanController.SetWaveTurbulence(Mathf.Lerp(weatherTypes[_currentWeather].windTurbulence, weatherTypes[newWeather].windTurbulence, t));
            oceanController.SetColors(Color.Lerp(weatherTypes[_currentWeather].scatteringColor, weatherTypes[newWeather].scatteringColor, t),
                Color.Lerp(weatherTypes[_currentWeather].scatteringColorShadow, weatherTypes[newWeather].scatteringColorShadow, t),
                Color.Lerp(weatherTypes[_currentWeather].scatteringColorShallow, weatherTypes[newWeather].scatteringColorShallow, t),
                Color.Lerp(weatherTypes[_currentWeather].sssColor, weatherTypes[newWeather].sssColor, t));
            oceanController.SetSunFalloff(Mathf.Lerp(weatherTypes[_currentWeather].sssSunFalloff, weatherTypes[newWeather].sssSunFalloff, t));
            oceanController.SetNormalStrength(Mathf.Lerp(weatherTypes[_currentWeather].normalStrength, weatherTypes[newWeather].normalStrength, t));

            oceanController.UpdateCausticsStrength(Mathf.Lerp(weatherTypes[_currentWeather].causticsStrength, weatherTypes[newWeather].causticsStrength, t));
            oceanController.UpdateOceanFogDensity(Mathf.Lerp(weatherTypes[_currentWeather].depthFog, weatherTypes[newWeather].depthFog, t));

            // transition through cloud values
            _clouds.densityMultiplier.value = Mathf.Lerp(weatherTypes[_currentWeather].densityMultiplier, weatherTypes[newWeather].densityMultiplier, t);
            _clouds.densityCurve.value = weatherTypes[newWeather].densityCurve;
            _clouds.shapeFactor.value = Mathf.Lerp(weatherTypes[_currentWeather].shapeFactor, weatherTypes[newWeather].shapeFactor, t);
            _clouds.ambientOcclusionCurve.value = weatherTypes[newWeather].ambientOcclusion;
            _clouds.bottomAltitude.value = Mathf.Lerp(weatherTypes[_currentWeather].bottomAltitude, weatherTypes[newWeather].bottomAltitude, t);
            _clouds.altitudeRange.value = Mathf.Lerp(weatherTypes[_currentWeather].altitudeRange, weatherTypes[newWeather].altitudeRange, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        ChangeWeather(newWeather);
        _isChangingWeather = false;
    }

    public void ChangeWeatherInGame(int newWeather)
    {
        if (_isChangingWeather)
        {
            return;
        }
        StartCoroutine(ChangeWeatherGradually(newWeather));
    }
}