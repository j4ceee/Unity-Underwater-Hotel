using Scripts;
using Unity.Mathematics;
using UnityEngine;

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

    private static readonly int CausticsStrength = Shader.PropertyToID("_CausticsStrength");
    /**
     * Update the strength of the caustics based on the progress of the day.
     */
    public void UpdateCausticsStrength(float progress, float paddingLow, float paddingHigh)
    {
        // oceanMaterial.SetFloat("_CausticsStrength", XYZ);
        // at the highest point of the sun (progress = 0.5, so from .4 - .6), the caustics should be the strongest (3.2)
        // at the highest point of the moon (progress = 0 or 1), the caustics should be weaker (0.5)
        // between .15 - .35 and .65 - .85, the caustics should be off (0)
        // animate between these values

        float causticsStrength;

        const float strongest = 3.2f;
        const float weakest = 0.5f;

        // sun at highest point
        if (progress >= TimeOfDay.SunHigh - paddingHigh &&
            progress <= TimeOfDay.SunHigh + paddingHigh) // sun at highest point
        {
            causticsStrength = strongest;
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
            causticsStrength = math.lerp(0f, strongest,
                (progress - TimeOfDay.MorningLow - paddingLow) /
                (TimeOfDay.SunHigh - paddingHigh - TimeOfDay.MorningLow - paddingLow));
        }
        // sun is setting
        else if (progress > TimeOfDay.SunHigh + paddingHigh && progress < TimeOfDay.EveningLow - paddingLow)
        {
            // animate from strongest to 0
            causticsStrength = math.lerp(strongest, 0f,
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

        oceanMaterial.SetFloat(CausticsStrength, causticsStrength);
    }

    private const float DepthFogDefault = 0.243f;
    private const float DepthFogMax = 0.5f;

    public void UpdateOceanFogDensity(float progress, float paddingLow)
    {
        float depthFogDensityFactor = DepthFogDefault;

        paddingLow = .5f*paddingLow;

        // when the sun is setting, the fog should get denser
        if (progress >= TimeOfDay.EveningLow && progress <= TimeOfDay.EveningLow + paddingLow)
        {
            // animate from default to max
            depthFogDensityFactor = math.lerp(DepthFogDefault, DepthFogMax, (progress - TimeOfDay.EveningLow) / paddingLow);
        }
        // when the sun is rising, the fog should get less dense
        else if (progress >= TimeOfDay.MorningLow - paddingLow && progress <= TimeOfDay.MorningLow)
        {
            // animate from max to default
            depthFogDensityFactor = math.lerp(DepthFogMax, DepthFogDefault, (progress - TimeOfDay.MorningLow + paddingLow) / paddingLow);
            underwaterRenderer._depthFogDensityFactor = depthFogDensityFactor;
        }
        else if (progress >= TimeOfDay.EveningLow + paddingLow || progress <= TimeOfDay.MorningLow - paddingLow)
        {
            depthFogDensityFactor = DepthFogMax;
        }

        underwaterRenderer._depthFogDensityFactor = depthFogDensityFactor;
    }
}