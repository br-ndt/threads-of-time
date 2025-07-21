using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Combat;
using Assets.Scripts.Configs;
using Assets.Scripts.Events;
using Assets.Scripts.States;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public enum AudioContext { None, Title, Overworld, Battle, Victory, Defeat }

        private AudioSource bgmSource;
        private AudioSource loaderSource;

        [SerializeField, Range(0, 1)] private float masterVolume = 0.4f;
        [SerializeField, Range(0, 0.5f)] private float pausedVolume = 0.2f;
        [SerializeField, Range(0.15f, 0.75f)] private float trackFadeDuration = 0.75f;

        [Space(10)]
        [Header("Event Channels")]
        [SerializeField] private GameStateChangeEvent gameStateChanged;
        [SerializeField] private BattleEndEvent battleEndEvent;

        [Space(10)]
        [Header("BGM Lists")]
        [SerializeField] private AudioClip titleTrack;
        [SerializeField] private List<AudioClip> overworldTracklist;
        [SerializeField] private List<AudioClip> battleTracklist;

        [Space(10)]
        [Header("Victory/Defeat Tunes")]
        [SerializeField] private AudioClip victoryTune;
        [SerializeField] private AudioClip defeatTune;
        private AudioClip currentTrack;
        private float startTime = 0f;

        private Coroutine fadeCoroutine;

        [Space(10)]
        [Header("Events")]
        [SerializeField] private PauseEvent pauseEvent;

        private void OnEnable()
        {
            gameStateChanged.OnEventRaised += HandleGameStateChange;
            // battleStartEvent.OnEventRaised += HandleBattleStart;
            battleEndEvent.OnEventRaised += HandleBattleEnd;
            pauseEvent.OnEventRaised += OnPausePressed;
        }

        private void OnDisable()
        {
            gameStateChanged.OnEventRaised -= HandleGameStateChange;
            // battleStartEvent.OnEventRaised -= HandleBattleStart;
            battleEndEvent.OnEventRaised -= HandleBattleEnd;
            pauseEvent.OnEventRaised -= OnPausePressed;
        }

        private void Start()
        {
            // Setup sources
            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources.Length < 2)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                loaderSource = gameObject.AddComponent<AudioSource>();
            }
            else
            {
                bgmSource = sources[0];
                loaderSource = sources[1];
            }

            bgmSource.playOnAwake = false;
            loaderSource.playOnAwake = false;

            loaderSource.volume = 0f;

            bgmSource.clip = titleTrack;
            bgmSource.loop = true;
            bgmSource.volume = masterVolume;
            currentTrack = titleTrack;

            foreach (AudioClip track in overworldTracklist) SilentlyLoadTrack(track);
            foreach (AudioClip track in battleTracklist) SilentlyLoadTrack(track);
            SilentlyLoadTrack(victoryTune);
            SilentlyLoadTrack(defeatTune);
        }

        private void SilentlyLoadTrack(AudioClip track)
        {
            loaderSource.clip = track;
            loaderSource.Play();
            loaderSource.Pause();
        }

        private void HandleGameStateChange((GameState state, GameConfig config) payload)
        {
            AudioContext ctx = payload.state switch
            {
                GameState.TitleScreen => AudioContext.Title,
                GameState.Battle => AudioContext.Battle,
                GameState.Overworld => AudioContext.Overworld,
                GameState.None => AudioContext.None,
                _ => AudioContext.None
            };

            PlayBGM(ctx);
        }

        public void OnPausePressed(bool paused)
        {
            if (paused) bgmSource.volume = pausedVolume;
            else bgmSource.volume = masterVolume;
        }

        private void HandleBattleEnd(bool didPlayerWin)
        {
            PlayBGM(didPlayerWin ? AudioContext.Victory : AudioContext.Defeat);
        }

        private void PlayBGM(AudioContext ctx)
        {
            AudioClip selectedClip = null;
            startTime = 0f;

            switch (ctx)
            {
                case AudioContext.Title:
                    selectedClip = titleTrack;
                    break;
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

    }
}
