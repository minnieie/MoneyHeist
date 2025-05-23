using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneFade : MonoBehaviour
{
    private Image fadeImage;

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
        // Make sure the fade image starts fully opaque and visible
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f);
        gameObject.SetActive(true);
    }

    public IEnumerator FadeInCoroutine(float duration)
    {
        // Ensure object is active and fully opaque at start
        gameObject.SetActive(true);
        Color startColour = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f);
        Color targetColour = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0f);

        fadeImage.color = startColour;  // reset alpha to 1

        yield return FadeCoroutine(startColour, targetColour, duration);

        // After fading in (to transparent), hide the image object
        gameObject.SetActive(false);
    }

    public IEnumerator FadeOutCoroutine(float duration)
    {
        // Make visible and transparent at start
        gameObject.SetActive(true);
        Color startColour = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0f);
        Color targetColour = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1f);

        fadeImage.color = startColour;  // reset alpha to 0

        yield return FadeCoroutine(startColour, targetColour, duration);
    }

    private IEnumerator FadeCoroutine(Color startColour, Color targetColour, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            fadeImage.color = Color.Lerp(startColour, targetColour, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Ensure the target color is set at the end to avoid minor inaccuracies
        fadeImage.color = targetColour;
    }
}
