using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlackoutController : MonoBehaviour
{
    public RawImage BlackoutImage;
    Coroutine blackoutCoroutine;
    bool isBlackoutHandlerStopped = false;

    void Start()
    {
        Color color = BlackoutImage.material.color;
        color.a = 0f;
        BlackoutImage.material.color = color;
        blackoutCoroutine = StartCoroutine(BlackoutHandler());
    }

    IEnumerator BlackoutHandler()
    {
        Material mat = BlackoutImage.material;
        Color color = mat.color;

        yield return new WaitForSeconds(Random.Range(30f, 50f));
        float sanity = PowerController.Instance.CurrentSanity;

        while (sanity > 1f)
        {
            sanity = Mathf.Clamp(PowerController.Instance.CurrentSanity, 0f, 100f);
        
            float cooldown = Mathf.Clamp(Random.Range(sanity * 0.75f, sanity * 1f), 0f, 100f);
            yield return new WaitForSeconds(cooldown);
        
            int blackoutCount = Random.Range(0, 5 - Mathf.FloorToInt(sanity / 20f));
        
            for (int i = 0; i <= blackoutCount; i++)
            {
                float maxOpacity = Random.Range((100f - sanity) / 2f, 60f) / 100f;
                float duration = Random.Range((100f - sanity) / 40f, 3f) / 3f;
        
                yield return FadeMaterialAlpha(mat, color, 0f, maxOpacity, duration);
                yield return FadeMaterialAlpha(mat, color, maxOpacity, 0f, duration);
            }
        
            color.a = 0f;
            mat.color = color;
        }
    }

    IEnumerator FadeMaterialAlpha(Material mat, Color baseColor, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            baseColor.a = alpha;
            mat.color = baseColor;
            yield return null;
        }
    }

    IEnumerator FadeInInsanity(Material mat, Color color)
    {
        while (!GameSceneController.Instance.isGameStopped)
        {
            float sanity = Mathf.Clamp(PowerController.Instance.CurrentSanity, 0f, 100f);
            float alpha = Mathf.Lerp(0f, 1f, 1f - sanity);
            color.a = alpha;
            mat.color = color;
            yield return null;
        }
    }

    private void Update()
    {
        if (PowerController.Instance.CurrentSanity < 1f && !isBlackoutHandlerStopped)
        {
            StopCoroutine(blackoutCoroutine);
            Material mat = BlackoutImage.material;
            Color color = mat.color;
            StartCoroutine(FadeMaterialAlpha(mat, color, color.a, 0f, 1f));
            isBlackoutHandlerStopped = true;
            StartCoroutine(FadeInInsanity(mat, color));
        }
    }
}
