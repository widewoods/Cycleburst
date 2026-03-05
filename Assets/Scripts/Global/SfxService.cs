using UnityEngine;

[DisallowMultipleComponent]
public class SfxService : MonoBehaviour
{
    public static SfxService Instance { get; private set; }

    [Header("Output")]
    [SerializeField] private AudioSource oneShotSource;

    [Header("Defaults")]
    [SerializeField] private float defaultVolume = 1f;
    [SerializeField] private Vector2 randomPitchRange = new Vector2(1f, 1f);
    [SerializeField] private float spatialBlend = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate SfxService detected. Destroying the newer instance.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (oneShotSource == null)
        {
            oneShotSource = GetComponent<AudioSource>();
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void Play(AudioClip clip)
    {
        Play(clip, defaultVolume, false);
    }

    public void Play(AudioClip clip, float volumeScale, bool randomizePitch = false)
    {
        if (clip == null || oneShotSource == null) return;

        float originalPitch = oneShotSource.pitch;
        if (randomizePitch)
        {
            oneShotSource.pitch = GetRandomPitch();
        }

        oneShotSource.PlayOneShot(clip, volumeScale);
        oneShotSource.pitch = originalPitch;
    }

    public void PlayRandom(AudioClip[] clips)
    {
        PlayRandom(clips, defaultVolume, true);
    }

    public void PlayRandom(AudioClip[] clips, float volumeScale, bool randomizePitch = true)
    {
        AudioClip clip = GetRandomClip(clips);
        if (clip == null) return;

        Play(clip, volumeScale, randomizePitch);
    }

    public void PlayAtPoint(AudioClip clip, Vector3 position)
    {
        PlayAtPoint(clip, position, defaultVolume, false);
    }

    public void PlayAtPoint(AudioClip clip, Vector3 position, float volumeScale, bool randomizePitch = false)
    {
        if (clip == null) return;

        GameObject tempObject = new GameObject($"SFX_{clip.name}");
        tempObject.transform.position = position;

        AudioSource tempSource = tempObject.AddComponent<AudioSource>();
        tempSource.clip = clip;
        tempSource.volume = volumeScale;
        tempSource.pitch = randomizePitch ? GetRandomPitch() : 1f;
        tempSource.spatialBlend = spatialBlend;
        tempSource.Play();

        Destroy(tempObject, clip.length / Mathf.Max(0.01f, tempSource.pitch));
    }

    public void PlayRandomAtPoint(AudioClip[] clips, Vector3 position, float volumeScale = 1f, bool randomizePitch = true)
    {
        AudioClip clip = GetRandomClip(clips);
        if (clip == null) return;

        PlayAtPoint(clip, position, volumeScale, randomizePitch);
    }

    private AudioClip GetRandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return null;
        int index = Random.Range(0, clips.Length);
        return clips[index];
    }

    private float GetRandomPitch()
    {
        float min = Mathf.Min(randomPitchRange.x, randomPitchRange.y);
        float max = Mathf.Max(randomPitchRange.x, randomPitchRange.y);
        return Random.Range(min, max);
    }
}
