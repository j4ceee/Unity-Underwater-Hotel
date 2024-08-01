using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ChangeExtLightColours : InteractableObject
{
    public ExteriorLightController exteriorLightController;
    public LightColourMode lightColourMode;

    public override void TriggerAction()
    {
        if (exteriorLightController)
        {
            exteriorLightController.SetLightColourMode(lightColourMode);
        }
    }

    public override string GetInteractionName()
    {
        // insert a space before each uppercase letter in the enum value
        string lightColourModeName = Regex.Replace(lightColourMode.ToString(), "(?<!^)([A-Z])", " $1");

        // split the string on the space character and join with a slash
        lightColourModeName = string.Join(" / ", lightColourModeName.Split(' '));

        return "Change Colour Scheme to " + lightColourModeName;
    }
}
