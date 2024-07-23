using System;
using Scripts;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

/**
 * Change aspects of the water volume:
 * [-] water level
 * [-] wave strength
 * [-] wave turbulence
 */
public class WaterController : MonoBehaviour
{
    [Tooltip("Ocean renderer of the ocean (likely 'Ocean' in the hierarchy)")]
    public GameObject oceanRenderer;

    [Tooltip("Wave controller of the ocean (likely 'Crest Waves' in the hierarchy)")]
    public Crest.ShapeFFT waves;

    [Tooltip("Material of the ocean (likely located inside Crest/Crest/Materials")]
    public Material oceanMaterial;

    [Tooltip("Underwater renderer of the ocean (should be on main camera)")]
    public Crest.UnderwaterRenderer underwaterRenderer;

    private float _waterLevel = 10f; // default = 10f
    private float _waveStrength = 0.313f; // default = 0.313f
    private float _waveTurbulence = 0.087f; // default = 0.087f

    private void Start()
    {
        Material newMaterial = Instantiate(oceanMaterial);
        oceanRenderer.GetComponent<Crest.OceanRenderer>().OceanMaterial = newMaterial;
        oceanMaterial = newMaterial;
    }

    /**
     * Set the water level of the ocean.
     */
    public void SetWaterLevel(float waterLevel)
    {
        _waterLevel = waterLevel;
        oceanRenderer.transform.position = new Vector3(oceanRenderer.transform.position.x, _waterLevel,
            oceanRenderer.transform.position.z);
    }

    /**
     * Set the strength of the waves.
     */
    public void SetWaveStrength(float waveStrength)
    {
        _waveStrength = waveStrength;
        waves._weight = _waveStrength;
    }

    /**
     * Set the turbulence of the waves.
     */
    public void SetWaveTurbulence(float waveTurbulence)
    {
        _waveTurbulence = waveTurbulence;
        waves._windTurbulence = _waveTurbulence;
    }

    private static readonly int ScatteringColor = Shader.PropertyToID("_ScatterColourBase");
    private static readonly int ScatteringColorShadow = Shader.PropertyToID("ScatterColourShadow");
    private static readonly int ScatteringColorShallow = Shader.PropertyToID("_ScatterColourShallow");
    private static readonly int SssColor = Shader.PropertyToID("_SSSTint");
    private static readonly int SssSunFalloff = Shader.PropertyToID("_SSSSunFalloff");
    private static readonly int NormalStrength = Shader.PropertyToID("_NormalsStrength");
    /**
     * Set Scattering & SSS colors
     */
    public void SetColors(Color scatteringColor, Color scatteringColorShadow, Color scatteringColorShallow, Color sssColor)
    {
        oceanMaterial.SetColor(ScatteringColor, scatteringColor);
        oceanMaterial.SetColor(ScatteringColorShadow, scatteringColorShadow);
        oceanMaterial.SetColor(ScatteringColorShallow, scatteringColorShallow);
        oceanMaterial.SetColor(SssColor, sssColor);
    }

    public void SetSunFalloff(float sssSunFalloff)
    {
        oceanMaterial.SetFloat(SssSunFalloff, sssSunFalloff);
    }

    public void SetNormalStrength(float normalStrength)
    {
        oceanMaterial.SetFloat(NormalStrength, normalStrength);
    }

    private static readonly int CausticsStrength = Shader.PropertyToID("_CausticsStrength");
    public float strongestCaustics = 3.2f;

    /**
     * Update the strength of the caustics based on the progress of the day.
     */
    public void CausticsStrengthOverDay(float progress, float paddingLow, float paddingHigh)
    {
        // oceanMaterial.SetFloat("_CausticsStrength", XYZ);
        // at the highest point of the sun (progress = 0.5, so from .4 - .6), the caustics should be the strongest (3.2)
        // at the highest point of the moon (progress = 0 or 1), the caustics should be weaker (0.5)
        // between .15 - .35 and .65 - .85, the caustics should be off (0)
        // animate between these values

        float causticsStrength;

        const float weakest = 0.5f;

        // sun at highest point
        if (progress >= TimeOfDay.SunHigh - paddingHigh &&
            progress <= TimeOfDay.SunHigh + paddingHigh) // sun at highest point
        {
            causticsStrength = strongestCaustics;
        }
        // moon at highest point
        else if (progress >= TimeOfDay.MoonHighEnd - paddingHigh ||
                 progress <= TimeOfDay.MoonHighStart + paddingHigh) // moon at highest point
        {
            causticsStrength = weakest;
        }
        // sun & moon at horizon
        else if ((progress >= TimeOfDay.MorningLow - paddingLow && progress <= TimeOfDay.MorningLow + paddingLow) ||
                 (progress >= TimeOfDay.EveningLow - paddingLow && progress <= TimeOfDay.EveningLow + paddingLow))
        {
            causticsStrength = 0f;
        }
        // animate between all states
        // sun is rising
        else if (progress > TimeOfDay.MorningLow + paddingLow && progress < TimeOfDay.SunHigh - paddingHigh)
        {
            // animate from 0 to strongest
            causticsStrength = math.lerp(0f, strongestCaustics,
                (progress - TimeOfDay.MorningLow - paddingLow) /
                (TimeOfDay.SunHigh - paddingHigh - TimeOfDay.MorningLow - paddingLow));
        }
        // sun is setting
        else if (progress > TimeOfDay.SunHigh + paddingHigh && progress < TimeOfDay.EveningLow - paddingLow)
        {
            // animate from strongest to 0
            causticsStrength = math.lerp(strongestCaustics, 0f,
                (progress - TimeOfDay.SunHigh - paddingHigh) /
                (TimeOfDay.EveningLow - paddingLow - TimeOfDay.SunHigh - paddingHigh));
        }
        // moon is rising
        else if (progress > TimeOfDay.EveningLow + paddingLow && progress < TimeOfDay.MoonHighEnd - paddingHigh)
        {
            // animate from 0 to weakest
            causticsStrength = math.lerp(0f, weakest,
                (progress - TimeOfDay.EveningLow - paddingLow) /
                (TimeOfDay.MoonHighEnd - paddingHigh - TimeOfDay.EveningLow - paddingLow));
        }
        // moon is setting
        else if (progress > TimeOfDay.MoonHighStart + paddingHigh && progress < TimeOfDay.MorningLow - paddingLow)
        {
            // animate from weakest to 0
            causticsStrength = math.lerp(weakest, 0f,
                (progress - TimeOfDay.MoonHighStart - paddingHigh) /
                (TimeOfDay.MorningLow - paddingLow - TimeOfDay.MoonHighStart - paddingHigh));
        }
        else
        {
            causticsStrength = 0f;
        }

        UpdateCausticsStrength(causticsStrength);
    }

    public void UpdateCausticsStrength(float strength)
    {
        oceanMaterial.SetFloat(CausticsStrength, strength);
    }

    public float depthFogDefault = 0.243f;
    private const float DepthFogMax = 0.5f;

    public void OceanFogDensityOverDay(float progress, float paddingLow)
    {
        float depthFogDensityFactor = depthFogDefault;

        paddingLow = .5f*paddingLow;

        // when the sun is setting, the fog should get denser
        if (progress >= TimeOfDay.EveningLow && progress <= TimeOfDay.EveningLow + paddingLow)
        {
            // animate from default to max
            depthFogDensityFactor = math.lerp(depthFogDefault, DepthFogMax, (progress - TimeOfDay.EveningLow) / paddingLow);
        }
        // when the sun is rising, the fog should get less dense
        else if (progress >= TimeOfDay.MorningLow - paddingLow && progress <= TimeOfDay.MorningLow)
        {
            // animate from max to default
            depthFogDensityFactor = math.lerp(DepthFogMax, depthFogDefault, (progress - TimeOfDay.MorningLow + paddingLow) / paddingLow);
            underwaterRenderer._depthFogDensityFactor = depthFogDensityFactor;
        }
        else if (progress >= TimeOfDay.EveningLow + paddingLow || progress <= TimeOfDay.MorningLow - paddingLow)
        {
            depthFogDensityFactor = DepthFogMax;
        }

        UpdateOceanFogDensity(depthFogDensityFactor);
    }

    public void UpdateOceanFogDensity(float density)
    {
        underwaterRenderer._depthFogDensityFactor = density;
    }
}