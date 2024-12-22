using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Handles the animation of the Hurt Overlay UI

[RequireComponent(typeof(Image))]
public class HurtOverlay : MonoBehaviour
{
    Image image;
    Color color;
    
    void Start()
    {
        image = GetComponent<Image>();
        color = image.color;

        // Make sure the overlay is not visible on start
        color.a = 0;
        image.color = color;
    }
    
    public void Hurt(float opacity)
    {
        StartCoroutine("HurtAnimation", opacity);
    }
    
    IEnumerator HurtAnimation(float opacity)
    {
        float max_alpha = 0.75f * opacity;

        float time = 0f;
        while (time < 1)
        {
            time += Time.deltaTime / 0.0125f; // fadein time

            color.a = Mathf.Lerp(0, max_alpha, time); // fade in
            image.color = color;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        yield return new WaitForSeconds(0.05f); // small pause

        time = 0f;
        while (time < 1)
        {
            time += Time.deltaTime / 0.1f; // fadeout time

            color.a = Mathf.Lerp(max_alpha, 0, time); // fade out
            image.color = color;

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
