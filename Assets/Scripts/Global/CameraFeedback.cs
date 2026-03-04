using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraFeedback : MonoBehaviour
{
    [Header("Camera Shake")]
    [SerializeField] private Transform shakeTarget;
    [SerializeField] private float defaultDuration = 0.12f;
    [SerializeField] private float defaultStrength = 0.15f;
    [SerializeField] private float defaultFrequency = 40f;

    [Header("Vignette")]
    [SerializeField] private Volume postProcessVolume;
    [SerializeField] private Color hitVignetteColor = new Color(0.75f, 0f, 0f, 1f);
    [SerializeField, Range(0f, 1f)] private float vignettePeakIntensity = 0.45f;
    [SerializeField] private float vignetteFadeInDuration = 0.04f;
    [SerializeField] private float vignetteFadeOutDuration = 0.16f;

    private Coroutine shakeRoutine;
    private Coroutine vignetteRoutine;
    private Vector3 baseLocalPosition;
    private Vignette vignette;
    private float baseVignetteIntensity;
    private Color baseVignetteColor;

    void Awake()
    {
        if (shakeTarget == null && Camera.main != null)
        {
            shakeTarget = Camera.main.transform;
        }

        if (shakeTarget != null)
        {
            baseLocalPosition = shakeTarget.localPosition;
        }

        if (postProcessVolume == null)
        {
            postProcessVolume = FindFirstObjectByType<Volume>();
        }

        if (TryGetVignette(out var resolvedVignette))
        {
            vignette = resolvedVignette;
            baseVignetteIntensity = vignette.intensity.value;
            baseVignetteColor = vignette.color.value;

            vignette.intensity.overrideState = true;
            vignette.color.overrideState = true;
            ResetVignette();
        }
    }

    public void CameraShake()
    {
        CameraShake(defaultDuration, defaultStrength, defaultFrequency);
    }

    public void CameraShake(float duration, float strength, float frequency)
    {
        if (shakeTarget == null) return;
        if (duration <= 0f || strength <= 0f)
        {
            ResetShakeTarget();
            return;
        }

        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            ResetShakeTarget();
        }

        shakeRoutine = StartCoroutine(CameraShakeRoutine(duration, strength, frequency));
    }

    public void PlayerHitVignette()
    {
        PlayerHitVignette(vignettePeakIntensity, vignetteFadeInDuration, vignetteFadeOutDuration);
    }

    public void PlayerHitVignette(float peakIntensity, float fadeInDuration, float fadeOutDuration)
    {
        if (vignette == null) return;

        if (vignetteRoutine != null)
        {
            StopCoroutine(vignetteRoutine);
        }

        ResetVignette();
        vignetteRoutine = StartCoroutine(PlayerHitVignetteRoutine(peakIntensity, fadeInDuration, fadeOutDuration));
    }

    private IEnumerator CameraShakeRoutine(float duration, float strength, float frequency)
    {
        float elapsed = 0f;
        baseLocalPosition = shakeTarget.localPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);
            float falloff = 1f - progress;
            float currentStrength = strength * falloff;

            float time = Time.time * Mathf.Max(0f, frequency);
            float x = (Mathf.PerlinNoise(time, 0f) - 0.5f) * 2f;
            float y = (Mathf.PerlinNoise(0f, time) - 0.5f) * 2f;

            Vector3 offset = new Vector3(x, y, 0f) * currentStrength;
            shakeTarget.localPosition = baseLocalPosition + offset;

            yield return null;
        }

        ResetShakeTarget();
        shakeRoutine = null;
    }

    private IEnumerator PlayerHitVignetteRoutine(float peakIntensity, float fadeInDuration, float fadeOutDuration)
    {
        if (vignette == null)
        {
            vignetteRoutine = null;
            yield break;
        }

        vignette.color.value = hitVignetteColor;

        float clampedPeakIntensity = Mathf.Clamp01(peakIntensity);
        float fadeIn = Mathf.Max(0f, fadeInDuration);
        float fadeOut = Mathf.Max(0f, fadeOutDuration);

        if (fadeIn > 0f)
        {
            float elapsed = 0f;
            while (elapsed < fadeIn)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fadeIn);
                vignette.intensity.value = Mathf.Lerp(baseVignetteIntensity, clampedPeakIntensity, t);
                yield return null;
            }
        }
        else
        {
            vignette.intensity.value = clampedPeakIntensity;
        }

        if (fadeOut > 0f)
        {
            float elapsed = 0f;
            while (elapsed < fadeOut)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fadeOut);
                vignette.intensity.value = Mathf.Lerp(clampedPeakIntensity, baseVignetteIntensity, t);
                yield return null;
            }
        }

        ResetVignette();
        vignetteRoutine = null;
    }

    private void ResetShakeTarget()
    {
        if (shakeTarget == null) return;
        shakeTarget.localPosition = baseLocalPosition;
    }

    private bool TryGetVignette(out Vignette resolvedVignette)
    {
        resolvedVignette = null;
        if (postProcessVolume == null) return false;

        VolumeProfile profile = postProcessVolume.profile != null
            ? postProcessVolume.profile
            : postProcessVolume.sharedProfile;

        if (profile == null) return false;
        return profile.TryGet(out resolvedVignette);
    }

    private void ResetVignette()
    {
        if (vignette == null) return;

        vignette.intensity.value = baseVignetteIntensity;
        vignette.color.value = baseVignetteColor;
    }

    void OnDisable()
    {
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            shakeRoutine = null;
        }

        if (vignetteRoutine != null)
        {
            StopCoroutine(vignetteRoutine);
            vignetteRoutine = null;
        }

        ResetShakeTarget();
        ResetVignette();
    }
}
