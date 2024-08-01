using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeatherOptions
{
    Sunny = 0,
    Stormy = 1,
}

public class ChangeWeather : InteractableObject
{
    public WeatherController weatherController;
    public WeatherOptions weatherType;

    public GameObject[] weatherButtons;
    private List<Material> _weatherButtonMaterials = new List<Material>();

    private bool _buttonPressed = false;

    public override void TriggerAction()
    {
        if (weatherController && !_buttonPressed)
        {
            if (weatherType == WeatherOptions.Sunny)
            {
                weatherController.ChangeWeatherInGame(0);
            }
            else if (weatherType == WeatherOptions.Stormy)
            {
                weatherController.ChangeWeatherInGame(1);
            }

            _buttonPressed = true;
        }
    }

    public override string GetInteractionName()
    {
        return "Change Weather to " + weatherType;
    }

    private void Start()
    {
        foreach (var button in weatherButtons)
        {
            _weatherButtonMaterials.Add(button.GetComponent<Renderer>().material);
        }
    }

    void Update()
    {
        if (_buttonPressed)
        {
            foreach (Material material in _weatherButtonMaterials)
            {
                if (material.color.a >= 1.0f)
                {
                    // set color alpha to 0.1f
                    material.color = new Color(material.color.r, material.color.g, material.color.b, 0.1f);
                }
            }

            bool stillProgress = weatherController.CheckIfChangingWeather();

            if (!stillProgress)
            {
                _buttonPressed = false;

                foreach (Material material in _weatherButtonMaterials)
                {
                    // set color alpha to 1.0f
                    material.color = new Color(material.color.r, material.color.g, material.color.b, 1.0f);
                }
            }
        }
    }
}