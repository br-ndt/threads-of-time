using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Audio { 
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        public string CurrentScene => SceneManager.GetActiveScene().name;
        public enum AudioContext { None, Overworld, Battle, Victory, Defeat }

        private AudioSource bgmSource;

        [SerializeField, Range(0, 1)] private float masterVolume = 0.4f;
        [SerializeField, Range(0.15f, 0.75f)] private float trackFadeDuration = 0.75f;

        [Header("BGM Lists")]
        [SerializeField] private List<AudioClip> overworldTracklist;
        [SerializeField] private List<AudioClip> battleTracklist;
        [Header("Victory/Defeat Tunes")]
        [SerializeField] private AudioClip victoryTune;
        [SerializeField] private AudioClip defeatTune;
        private AudioClip currentTrack;
        private float startTime = 0f;

        private Coroutine fadeCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Start()
        {
            bgmSource = GetComponent<AudioSource>();

            foreach (AudioClip track in overworldTracklist)
            {
                SilentlyLoadTrack(track);
            }

            foreach (AudioClip track in battleTracklist)
            {
                SilentlyLoadTrack(track);
            }

            SilentlyLoadTrack(victoryTune);
            SilentlyLoadTrack(defeatTune);

            bgmSource.loop = true;
            bgmSource.volume = masterVolume;
        }

        private void SilentlyLoadTrack(AudioClip track)
        {
            if (bgmSource.volume > 0) bgmSource.volume = 0f;
            bgmSource.clip = track;
            bgmSource.Play();
            bgmSource.Pause();
        }

        public void PlayBGM(AudioContext ctx)
        {
            AudioClip selectedClip = null;
            startTime = 0f;

            switch (ctx)
            {
                case AudioContext.Overworld:
                    if (overworldTracklist != null && overworldTracklist.Count > 0)
                        selectedClip = overworldTracklist[Random.Range(0, overworldTracklist.Count)];
                    break;
                case AudioContext.Battle:
                    if (battleTracklist != null && battleTracklist.Count > 0)
                        selectedClip = battleTracklist[Random.Range(0, battleTracklist.Count)];
                    break;
                case AudioContext.Victory:
                    selectedClip = victoryTune;
                    startTime = 49f; // todo (vn) un-hardcode these later
                    break;
                case AudioContext.Defeat:
                    selectedClip = defeatTune;
                    startTime = 12f; // todo (vn) un-hardcode these later
                    break;
            }

            if (selectedClip != null)
            {
                bool shouldLoop = (ctx == AudioContext.Overworld || ctx == AudioContext.Battle);
                if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                fadeCoroutine = StartCoroutine(FadeToNewBGM(selectedClip, shouldLoop));
            }
        }

        private IEnumerator FadeToNewBGM(AudioClip newClip, bool loop)
        {
            float duration = trackFadeDuration;
            float startVolume = bgmSource.volume;

            // Fade out
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
                yield return null;
            }
            bgmSource.volume = 0f;

            // Switch track
            bgmSource.clip = newClip;
            bgmSource.loop = loop;

            if (startTime > 0) bgmSource.time = startTime;

            bgmSource.Play();

            // Fade in
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(0f, startVolume, t / duration);
                yield return null;
            }
            bgmSource.volume = masterVolume;
        }

        private void Update()
        {
            bgmSource.volume = masterVolume;
        }
    }
}
