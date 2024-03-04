using UnityEngine;
using UnityEngine.UI;
using System;

public class StatusBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image Fill;

    public void SetMaxValue(int value)
    {
        slider.maxValue = value;
        slider.value = value;
        Fill.color = gradient.Evaluate(1f);
    }

    public void SetCurrentValue(int value)
    {
        slider.value = value;
        Fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void SetMinValue(int value)
    {
        slider.minValue = value;
    }

}
