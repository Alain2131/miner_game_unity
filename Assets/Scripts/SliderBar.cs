using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    public Slider slider;

    private bool isDead = false;

    public void SetMaxValue(float value)
    {
        slider.maxValue = value;
        slider.value = value;
    }

    public void SetValue(float value)
    {
        slider.value = value;
        if(!isDead)
            CheckForDeath();
    }

    // This should not be in here
    public void CheckForDeath()
    {
        if (slider.value <= 0)
        {
            isDead = true;
            Debug.LogWarning("Yer dead !");
        }
    }
}
