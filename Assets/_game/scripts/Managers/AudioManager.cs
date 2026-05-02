using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public struct NamedAudioClip
    {
        public string clipName;
        public AudioClip clip;
    }

    [System.Serializable]
    public struct NamedAudioClipArray
    {
        public string groupName;
        public AudioClip[] clips;
    }

    [Header("Audio Players Components")]
    [SerializeField] private AudioSource EffectsSource;
    [SerializeField] private AudioSource AmbienceSource;

    [Header("Random Pitch Adjustment Range")]
    public float LowPitchRange = .95f;
    public float HighPitchRange = 1.05f;

    public static AudioManager Instance = null;

    [Header("Audio Library")]
    [SerializeField] private List<NamedAudioClip> audioLibrary = new List<NamedAudioClip>();

    [Header("Random Audio Library")]
    [SerializeField] private List<NamedAudioClipArray> randomAudioLibrary = new List<NamedAudioClipArray>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Safely play ambience
        PlayAmbience("LoopMusic");
    }

    // Play a single clip by string key through the sound effects source.
    public void Play(string clipName)
    {
        AudioClip clip = GetClip(clipName);
        if (clip != null)
        {
            EffectsSource.clip = clip;
            EffectsSource.Play();
        }
        else
        {
            Debug.LogWarning($"Audio clip '{clipName}' not found in AudioManager.");
        }
    }

    // Play a random clip from the random library by string key.
    public void PlayRandomSoundEffect(string groupName)
    {
        AudioClip[] clips = GetRandomClips(groupName);

        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning($"Random audio group '{groupName}' not found or is empty in AudioManager.");
            return;
        }

        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(LowPitchRange, HighPitchRange);

        EffectsSource.pitch = randomPitch;
        EffectsSource.clip = clips[randomIndex];
        EffectsSource.Play();
    }

    // ==========================================
    // ===== AMBIENCE CROSSFADING METHODS =======
    // ==========================================

    public void TransitionAmbience(string clipName, float fadeDuration = 1.5f)
    {
        AudioClip newAmbience = GetClip(clipName);
        if (newAmbience != null)
        {
            StartCoroutine(FadeAmbienceRoutine(newAmbience, fadeDuration));
        }
        else
        {
            Debug.LogWarning($"Ambience clip '{clipName}' not found in AudioManager.");
        }
    }

    public void PlayAmbience(string clipName)
    {
        AudioClip newAmbience = GetClip(clipName);
        if (newAmbience != null)
        {
            AmbienceSource.clip = newAmbience;
            AmbienceSource.loop = true;
            AmbienceSource.Play();
        }
    }

    private IEnumerator FadeAmbienceRoutine(AudioClip newAmbience, float duration)
    {
        float startVolume = AmbienceSource.volume;

        // Fade out current ambience
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            AmbienceSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
            yield return null;
        }

        AmbienceSource.Stop();

        // Switch to the new clip and play it
        AmbienceSource.clip = newAmbience;
        AmbienceSource.Play();

        // Fade in new ambience
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            AmbienceSource.volume = Mathf.Lerp(0, startVolume, t / duration);
            yield return null;
        }

        AmbienceSource.volume = startVolume;
    }

    // Helper method to look up audio clips by name
    private AudioClip GetClip(string targetName)
    {
        foreach (var item in audioLibrary)
        {
            if (item.clipName == targetName)
            {
                return item.clip;
            }
        }
        return null;
    }

    // Helper method to look up clip arrays by name
    private AudioClip[] GetRandomClips(string targetName)
    {
        foreach (var item in randomAudioLibrary)
        {
            if (item.groupName == targetName)
            {
                return item.clips;
            }
        }
        return null;
    }
}