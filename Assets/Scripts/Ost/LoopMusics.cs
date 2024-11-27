using System.Collections;
using UnityEngine;

public class LoopMusics : MonoBehaviour
{
    public static LoopMusics Instance;

    private bool isPlaying;
    private bool isPaused;
    [SerializeField] private AudioSource[] osts;

    private int currentPlayingIndex;

    [SerializeField] private bool startOnPlay;
    [SerializeField] private float minDurationInterval;
    [SerializeField] private float maxDurationInterval;
    private void Start()
    {
        if (startOnPlay) Play(true);
    }
    public void Play(bool playFromStart)
    {
        if (isPlaying)
        {
            Stop();
        }
        isPlaying = true;
        if (playFromStart) currentPlayingIndex = 0;
        playLoop_Ref = StartCoroutine(PlayLoop_Coroutine());

    }
    public void Stop()
    {
        isPlaying = false;
        if (playLoop_Ref != null)
        {
            StopCoroutine(playLoop_Ref);
            playLoop_Ref = null;
        }
    }

    private Coroutine playLoop_Ref;
    private IEnumerator PlayLoop_Coroutine()
    {
        while (true)
        {
            int startIndex = currentPlayingIndex;
            for (int i = startIndex; i < osts.Length; i++)
            {
                currentPlayingIndex = i;
                osts[i].Play();
                while (osts[i].isPlaying || isPaused)
                {
                    yield return new WaitForSecondsRealtime(0.5f);
                }
                yield return new WaitForSecondsRealtime(Random.Range(minDurationInterval, maxDurationInterval));
            }
        }
    }

    public void Pause()
    {
        isPaused= true;
        osts[currentPlayingIndex].Pause();
    }
    public void Resume()
    {
        isPaused= false;
        osts[currentPlayingIndex].UnPause();
    }
}
