using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[System.Serializable]
public class LightGroup
{
    public GameObject lightGroup; // for turning specific lights on/off
    public float intensityDay;
    public float intensityNight;
    public bool isOn = true;

    public Light[] lights; // for setting the intensity

    [ItemCanBeNull] public GameObject[] emissiveObjects; // for setting the emissive material intensity
}

[System.Serializable]
public class LightRoom
{
    public GameObject roomLightGroup; // for turning all lights in the room on/off
    public List<LightGroup> lightGroups;
    public bool isOn;
}

[System.Serializable]
public class ReflProbe
{
    public HDProbe reflProbe;
    public bool isOn;
}

public class InteriorLightController : MonoBehaviour
{
    public List<LightRoom> lightRooms;

    private float _progress;

    private const float EveningStart = 0.66f;
    private const float EveningEnd = 0.755f;
    private const float MorningStart = 0.245f;
    private const float MorningEnd = 0.28f;

    // Start is called before the first frame update
    void Start()
    {
        // set all lightrooms according to their initial state
        foreach (LightRoom lightRoom in lightRooms)
        {
            foreach (LightGroup lightGroup in lightRoom.lightGroups)
            {
                lightGroup.lightGroup.SetActive(lightRoom.isOn);
            }
        }

        foreach (var reflProbe in reflectionProbes)
        {
            UpdateReflectionProbe(reflProbe.reflProbe);
        }

        StartCoroutine(UpdateLightProbesCoroutine());
    }

    public void LightIntensityOverDay(float progress)
    {
        _progress = progress;

        if (progress >= EveningEnd || progress <= MorningStart)
        {
            SetAllLightsToNightIntensity();
        }
        else if (progress < EveningStart && progress > MorningEnd)
        {
            SetAllLightsToDayIntensity();
        }
        else
        {
            foreach (LightRoom lightRoom in lightRooms)
            {
                foreach (LightGroup lightGroup in lightRoom.lightGroups)
                {
                    foreach (Light lightInst in lightGroup.lights)
                    {
                        // sun is setting
                        if (progress >= EveningStart && progress < EveningEnd)
                        {
                            // animate from day intensity to night intensity
                            lightInst.intensity = Mathf.Lerp(lightGroup.intensityDay, lightGroup.intensityNight,
                                (progress - EveningStart) / (EveningEnd - EveningStart));
                        }
                        // sun is rising
                        else if (progress > MorningStart && progress < MorningEnd)
                        {
                            // animate from night intensity to day intensity
                            lightInst.intensity = Mathf.Lerp(lightGroup.intensityNight, lightGroup.intensityDay,
                                (progress - MorningStart) / (MorningEnd - MorningStart));
                        }
                    }
                }
            }
        }
    }

    public void SetLightTimeOfDay(bool isNight)
    {
        foreach (LightRoom lightRoom in lightRooms)
        {
            foreach (LightGroup lightGroup in lightRoom.lightGroups)
            {
                float lightIntensity = isNight ? lightGroup.intensityNight : lightGroup.intensityDay;

                foreach (Light lightInst in lightGroup.lights)
                {
                    lightInst.intensity = lightIntensity;
                }
            }
        }
    }

    public void ToggleRoomLights(GameObject roomLightGroup, bool ?isOn = null)
    {
        foreach (LightRoom lightRoom in lightRooms)
        {
            if (lightRoom.roomLightGroup != roomLightGroup) continue; // skip if not the room we want to toggle

            foreach (LightGroup lightGroup in lightRoom.lightGroups)
            {
                if (isOn != null)
                {
                    lightGroup.lightGroup.SetActive((bool) isOn);
                }
                else
                {
                    lightGroup.lightGroup.SetActive(!lightRoom.isOn);
                }
            }

            lightRoom.isOn = lightRoom.lightGroups[0].isOn;
        }
    }

    public void ToggleSpecificLightGroup(GameObject lightGroup, bool ?isOn = null)
    {
        foreach (LightRoom room in lightRooms)
        {
            foreach (LightGroup group in room.lightGroups)
            {
                if (group.lightGroup != lightGroup) continue; // skip if not the group we want to toggle

                foreach (Light lightInst in group.lights)
                {
                    if (isOn != null)
                    {
                        lightInst.enabled = (bool) isOn;
                    }
                    else
                    {
                        lightInst.enabled = !group.isOn;
                    }
                }

                group.isOn = group.lights[0].enabled;

                if (group.emissiveObjects.Length > 0)
                {
                    // foreach child object of the light group
                    foreach (GameObject emissiveObject in group.emissiveObjects)
                    {
                        if (emissiveObject)
                        {
                            Debug.Log("Toggling emissive material: " + emissiveObject.name);
                            ToggleEmissiveMaterialIntensity(emissiveObject, group.isOn);
                        }
                    }
                }
            }
        }
    }

    private static readonly int EmissionIntensity = Shader.PropertyToID("_Emission_Intensity");
    private void ToggleEmissiveMaterialIntensity(GameObject lightObject, bool isOn)
    {
        // get the material of the light object (of shader type Shader Graph/Lamp Emissive)
        Material[] material = lightObject.GetComponent<Renderer>().materials;

        foreach (Material mat in material)
        {
            Debug.Log("Material name: " + mat.shader.name);
            if (mat.shader.name != "Shader Graphs/Lamp Emissive") continue;
            mat.SetFloat(EmissionIntensity, isOn ? 100 : 0);
        }
    }

    [ContextMenu("Set all lights to day intensity")]
    public void SetAllLightsToDayIntensity()
    {
        SetLightTimeOfDay(false);
    }

    [ContextMenu("Set all lights to night intensity")]
    public void SetAllLightsToNightIntensity()
    {
        SetLightTimeOfDay(true);
    }

    [ContextMenu("Fetch new light groups")]
    public void FetchLightGroupsInLightRooms()
    {

        foreach (LightRoom lightRoom in lightRooms)
        {
            // get all light groups (GameObject children) in the room & add them to the corresponding light array
            for (int i = 0; i < lightRoom.roomLightGroup.transform.childCount; i++)
            {
                GameObject lightGroup = lightRoom.roomLightGroup.transform.GetChild(i).gameObject;
                // if it already exists in lightRoom.lightGroups, skip
                if (lightRoom.lightGroups.Exists(lg => lg.lightGroup == lightGroup)) continue;

                // add the light group to the light groups list
                lightRoom.lightGroups.Add(new LightGroup
                {
                    lightGroup = lightGroup,
                    lights = lightGroup.GetComponentsInChildren<Light>()
                });
            }
        }
    }

    [ContextMenu("Fetch lights in existing light groups")]
    public void FetchLightsInLightGroups()
    {

        foreach (LightRoom lightRoom in lightRooms)
        {
            // fetch all lights in the light groups & add them to the corresponding light array
            foreach (LightGroup lightGroup in lightRoom.lightGroups)
            {
                lightGroup.lights = lightGroup.lightGroup.GetComponentsInChildren<Light>();
            }
        }
    }

    /// <summary>
    /// Reflection Probes handling
    /// </summary>

    [Tooltip("Group of Reflection Probes to update")]
    public List<ReflProbe> reflectionProbes; // list of Reflection Probes to update
    [Tooltip("Interval in seconds between updates")]
    public float updateInterval = 4.0f; // interval in seconds between updates
    [Tooltip("Delay in frames between each Reflection Probe update (on top of Update Interval for each Reflection Probe)")]
    public int frameDelay = 7; // delay in frames between each LightProbe update

    private const float MorningStartUpdate = 0.225f;
    private const float MorningEndUpdate = .3f;
    private const float EveningStartUpdate = .73f;
    private const float EveningEndUpdate = .79f;

    private IEnumerator UpdateLightProbesCoroutine()
    {
        int waitCounter = 0;
        int inactiveDelay = 1;

        while (true)
        {
            if ((_progress >= MorningStartUpdate && _progress <= MorningEndUpdate) || (_progress >= EveningStartUpdate && _progress <= EveningEndUpdate))
            {
                yield return new WaitForSeconds(0.25f * updateInterval);
            }
            else
            {
                yield return new WaitForSeconds(updateInterval);
            }

            for (int i = 0; i < reflectionProbes.Count; i++)
            {
                if (!reflectionProbes[i].isOn && waitCounter < inactiveDelay)
                {
                    continue;
                }

                UpdateReflectionProbe(reflectionProbes[i].reflProbe);
                yield return new WaitForSeconds(frameDelay * Time.deltaTime);
            }
            waitCounter++;

            if (waitCounter > inactiveDelay)
            {
                waitCounter = 0;
            }
        }
    }

    private void UpdateReflectionProbe(HDProbe reflProbe)
    {
        reflProbe.RequestRenderNextUpdate();
    }

    public void ToggleReflectionProbes(HDProbe[] reflProbes, bool isOn)
    {
        foreach (HDProbe tmpHdProbe in reflProbes)
        {
            if (isOn && tmpHdProbe)
            {
                UpdateReflectionProbe(tmpHdProbe);
            }

            // set isOn in the corresponding ReflProbe object
            foreach (ReflProbe reflProbe in reflectionProbes)
            {
                if (reflProbe.reflProbe == tmpHdProbe)
                {
                    reflProbe.isOn = isOn;
                }
            }
        }
    }
}