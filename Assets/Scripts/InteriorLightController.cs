using System;
using System.Collections;
using System.Collections.Generic;
using Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class LightGroup
{
    public GameObject lightGroup; // for turning specific lights on/off
    public float intensityDay;
    public float intensityNight;
    public bool isOn = true;

    public Light[] lights; // for setting the intensity
}

[System.Serializable]
public class LightRoom
{
    public GameObject roomLightGroup; // for turning all lights in the room on/off
    public List<LightGroup> lightGroups;
    public bool isOn = false;
}

public class InteriorLightController : MonoBehaviour
{
    public List<LightRoom> lightRooms;

    public float eveningStart = 0.66f;
    public float eveningEnd = 0.755f;
    public float morningStart = 0.245f;
    public float morningEnd = 0.28f;

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
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void LightIntensityOverDay(float progress)
    {
        if (progress >= eveningEnd || progress <= morningStart)
        {
            SetAllLightsToNightIntensity();
        }
        else if (progress < eveningStart && progress > morningEnd)
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
                        if (progress >= eveningStart && progress < eveningEnd)
                        {
                            // animate from day intensity to night intensity
                            lightInst.intensity = Mathf.Lerp(lightGroup.intensityDay, lightGroup.intensityNight,
                                (progress - eveningStart) / (eveningEnd - eveningStart));
                        }
                        // sun is rising
                        else if (progress > morningStart && progress < morningEnd)
                        {
                            // animate from night intensity to day intensity
                            lightInst.intensity = Mathf.Lerp(lightGroup.intensityNight, lightGroup.intensityDay,
                                (progress - morningStart) / (morningEnd - morningStart));
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
                    lightInst.intensity = (float)lightIntensity;
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
            }
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
}