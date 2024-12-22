using UnityEngine;
using UnityEngine.UI;

public class SliderBar : MonoBehaviour
{
    public Slider slider;

    private bool is_player_dead = false;

    public void SetMaxValue(float value)
    {
        slider.maxValue = value;
        slider.value = value;
    }

    public void SetValue(float value)
    {
        slider.value = value;
        if(!is_player_dead)
            CheckForDeath();
    }

    // This should not be in here
    public void CheckForDeath()
    {
        if (slider.value <= 0)
        {
            is_player_dead = true;
            Debug.LogWarning("Yer dead !");
        }
    }
}
