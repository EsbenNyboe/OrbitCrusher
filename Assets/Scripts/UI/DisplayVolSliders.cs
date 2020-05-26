using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayVolSliders : MonoBehaviour
{
    public GameObject sliderMasterDisplay;

    public void ToggleSliderDisplay(bool show)
    {
        sliderMasterDisplay.SetActive(show);
    }
}
