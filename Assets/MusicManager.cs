using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Background Music")]
    public AudioClip[] backgroundMusic;

    [Header("Boss Music")]
    public List<BossMusicEntry> bossMusicEntries;

    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Transition Settings")]
    public float transitionSpeed = 1.0f;

    private bool isBossActive = false;
    private GameObject activeBoss;
    private bool isTransitioning = false;
    private AudioClip nextClip;

    private Dictionary<GameObject, AudioClip> bossMusicDict;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.volume = 0.3f; // Nastavení výchozí hlasitosti (30%)
        audioSource.loop = false;
        audioSource.playOnAwake = false;

        // Create the boss music dictionary
        bossMusicDict = new Dictionary<GameObject, AudioClip>();
        foreach (var entry in bossMusicEntries)
        {
            bossMusicDict[entry.bossPrefab] = entry.track;
        }

        PlayRandomBackgroundMusic();
    }

    void Update()
    {
        if (isTransitioning)
        {
            HandleTransition();
        }
        else if (!audioSource.isPlaying)
        {
            if (isBossActive)
            {
                PlayBossMusic(activeBoss);
            }
            else
            {
                PlayRandomBackgroundMusic();
            }
        }
    }

    private void PlayRandomBackgroundMusic()
    {
        if (backgroundMusic.Length == 0) return;

        int randomIndex = Random.Range(0, backgroundMusic.Length);
        PlayClip(backgroundMusic[randomIndex]);
    }

    private void PlayBossMusic(GameObject boss)
    {
        if (bossMusicDict.TryGetValue(boss, out var track))
        {
            PlayClip(track);
        }
        else
        {
            Debug.LogWarning($"No music assigned for boss prefab: {boss.name}");
        }
    }

    private void PlayClip(AudioClip clip)
    {
        nextClip = clip;
        audioSource.volume = 0.3f;  // Nastavení základní hlasitosti (0.3 = 30%)
        isTransitioning = true;
    }

    private void HandleTransition()
    {
        if (audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime * transitionSpeed;
        }
        else
        {
            audioSource.clip = nextClip;
            audioSource.Play();
            isTransitioning = false;
            StartCoroutine(FadeIn());
        }
    }

    private System.Collections.IEnumerator FadeIn()
    {
        while (audioSource.volume < 0.35f)
        {
            audioSource.volume += Time.deltaTime * transitionSpeed;
            yield return null;
        }
    }

    public void ActivateBossMusic(GameObject boss)
    {
        if (!isBossActive || activeBoss != boss)
        {
            isBossActive = true;
            activeBoss = boss;
            PlayBossMusic(boss);
        }
    }

    public void DeactivateBossMusic()
    {
        if (isBossActive)
        {
            isBossActive = false;
            PlayRandomBackgroundMusic();
        }
    }
}

[System.Serializable]
public class BossMusicEntry
{
    public GameObject bossPrefab;
    public AudioClip track;
}
