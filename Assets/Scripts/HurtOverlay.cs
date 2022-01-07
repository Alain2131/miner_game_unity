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
    
    public void Hurt()
    {
        StartCoroutine("HurtAnimation");
    }
    
    IEnumerator HurtAnimation()
    {
        float maxAlpha = 0.75f;

        float t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / 0.0125f; // fadein time

            color.a = Mathf.Lerp(0, maxAlpha, t); // fade in
            image.color = color;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        yield return new WaitForSeconds(0.05f); // small pause

        t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / 0.1f; // fadeout time

            color.a = Mathf.Lerp(maxAlpha, 0, t); // fade out
            image.color = color;

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
