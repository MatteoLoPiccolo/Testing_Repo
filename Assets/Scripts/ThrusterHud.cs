using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrusterHud : MonoBehaviour
{
    private Slider _thrusterSlider;

    private void Start()
    {
        _thrusterSlider = GetComponent<Slider>();
    }

    public void SetThrusterSliderValue(float speed)
    {
        _thrusterSlider.value = speed;
    }
}