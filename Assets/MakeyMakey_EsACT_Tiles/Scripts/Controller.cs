using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

public class Controller : MonoBehaviour {

    enum ViewMode {
        Start,
        Video,
        Image
    }

    [Header("References:")]

    [SerializeField] RectTransform StartScreen;
    [SerializeField] Image ImageDisplayer;
    [SerializeField] AudioSource AudioSource;
    [SerializeField] RawImage VideoDisplayer;
    [SerializeField] VideoPlayer VideoPlayer;

    [Header("Media")]
    [SerializeField] VideoClip[] VideoClips;
    [SerializeField] AudioClip[] AudioClips;

    [Header("Key Bindings")]
    [SerializeField] UnityEvent OnUpPress;
    [SerializeField] UnityEvent OnRightPress;
    [SerializeField] UnityEvent OnLeftPress;
    [SerializeField] UnityEvent OnDownPress;
    [SerializeField] UnityEvent OnSpacePress;
    [SerializeField] UnityEvent OnClickPress;

    ViewMode _currentMode = ViewMode.Image;
    int _currentClipIndex = -1;

    Coroutine _muteSoundCoroutine = null;
    Coroutine _randomVideoCoroutine = null;


    // Start is called before the first frame update
    void Start() {
        VideoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        VideoPlayer.controlledAudioTrackCount = 1;
        VideoPlayer.SetTargetAudioSource(0, AudioSource);
        AudioSource.volume = 1f;
        VideoPlayer.EnableAudioTrack(0, true);

        StartScreen.gameObject.SetActive(true);

    }

    // Update is called once per frame
    void Update() {

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            OnUpPress.Invoke();

        } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            OnRightPress.Invoke();

        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            OnLeftPress.Invoke();

        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            OnDownPress.Invoke();

        } else if (Input.GetKeyDown(KeyCode.Space)) {
            OnSpacePress.Invoke();

        } else if (Input.GetKeyDown(KeyCode.Mouse0)) { 
            OnClickPress.Invoke();

        }else if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();

        }   

    }

    // --------- | Utility


    private void ToggleMode(ViewMode to) {
        if (StartScreen.gameObject.activeSelf)
            StartScreen.gameObject.SetActive(false);


        if (to != _currentMode) {
            switch (_currentMode) {
                case ViewMode.Start:

                    break;

                case ViewMode.Image:
                    ImageDisplayer.gameObject.SetActive(false); 
                    break;

                case ViewMode.Video:
                    VideoPlayer.Stop(); 
                    VideoDisplayer.gameObject.SetActive(false);
                    break;

            }

            switch (to) {
                case ViewMode.Image:
                    ImageDisplayer.gameObject.SetActive(true);
                    break;

                case ViewMode.Video:
                    VideoDisplayer.gameObject.SetActive(true);
                    break;

            }

        }
    }

    IEnumerator MuteSoundTimer(int time) {
        yield return new WaitForSeconds(time);
        AudioSource.volume = 0f;

    }

    IEnumerator PlayRandomVideoTimer(int time) {
        while (true) {
            int random = UnityEngine.Random.Range(0, VideoClips.Length);

            while (random == _currentClipIndex)
                random = UnityEngine.Random.Range(0, VideoClips.Length);

            yield return new WaitForSeconds(time);

            _currentClipIndex = random;
            VideoPlayer.clip = VideoClips[random];
            VideoPlayer.Play();

        }

    }

    // --------- | Actions

    public void PlaySound(int index) {
        if (AudioSource.isPlaying)
            AudioSource.Stop();

        AudioSource.PlayClipAtPoint(AudioClips[index], Vector3.zero);

    }

    public void PlayVideo(int index) {
        ToggleMode(ViewMode.Video);

        if (VideoPlayer.isPlaying)
            VideoPlayer.Stop();

        if (_muteSoundCoroutine != null)
            StopCoroutine(_muteSoundCoroutine);

        if (_randomVideoCoroutine != null)
            StopCoroutine(_randomVideoCoroutine);


        _currentClipIndex = index;
        VideoPlayer.clip = VideoClips[index];
        VideoPlayer.Play();

    }


    public void SetVolume(float value) {
        AudioSource.volume = value > 1 ? 1 : value < 0 ? 0 : value;

    }

    public void MuteAfter(int seconds) {
        _muteSoundCoroutine = StartCoroutine(MuteSoundTimer(seconds));

    }

    public void PlayRandomVideoAfter(int seconds) {
        _randomVideoCoroutine = StartCoroutine(PlayRandomVideoTimer(seconds));

    }

    public void DisplayImage(Sprite image) {
        ToggleMode(ViewMode.Image);

        ImageDisplayer.sprite = image;

    }

}
