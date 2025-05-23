using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SceneFade : MonoBehaviour
{
    private Image fadeImage;

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
    }

    public IEnumerator FadeInCoroutine(float duration)
    {
        Color startColour = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);
        Color targetColour = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);

        yield return FadeCoroutine(startColour, targetColour, duration);

        gameObject.SetActive(false);
    }
    public  IEnumerator FadeOutCoroutine(float duration)
    {
        Color startColour = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);
        Color targetColour = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 1);

        gameObject.SetActive(true);
        yield return FadeCoroutine(startColour, targetColour, duration);
    }
    private IEnumerator FadeCoroutine(Color startColour, Color targetColour, float duration)
    {
        float elapsedTime = 0;
        float elapsedPercantage = 0;
        while (elapsedPercantage < 1)
        {
            elapsedPercantage = elapsedTime / duration;
            fadeImage.color = Color.Lerp(startColour, targetColour, elapsedPercantage);

            yield return null;
            elapsedTime += Time.deltaTime;
        }
    }
}
