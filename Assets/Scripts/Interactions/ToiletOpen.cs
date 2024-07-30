using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToiletOpen : MonoBehaviour
{
    public GameObject toiletLid;
    public GameObject toiletLightGroup;

    public InteriorLightController interiorLightController;
    private static readonly int IsOpen = Animator.StringToHash("IsOpen");

    private void OnTriggerEnter(Collider other)
    {
        toiletLid.GetComponent<Animator>().SetBool(IsOpen, true);
        interiorLightController.ToggleSpecificLightGroup(toiletLightGroup, true);
    }

    private void OnTriggerExit(Collider other)
    {
        toiletLid.GetComponent<Animator>().SetBool(IsOpen, false);
        interiorLightController.ToggleSpecificLightGroup(toiletLightGroup, false);
    }
}
