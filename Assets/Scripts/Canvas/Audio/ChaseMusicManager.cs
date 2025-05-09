using System.Collections;
using UnityEngine;
using Zenject;

public class ChaseMusicManager : MonoBehaviour
{
    [SerializeField]
    private ChaseTrack[] _tracks;
    [SerializeField]
    private AudioSource _musicPlayer;

    [Inject]
    private GameManager _gameManager;

    private int _trackIndex;
    private bool _playing;
    private Coroutine _midTrackCoroutine;

    private void Start()
    {
        _gameManager.OnChase += HandleChaseMusic;
    }

    private void HandleChaseMusic(bool onChase)
    {
        if (_playing == onChase)
            return;

        Debug.Log("CHANGE TO: " + onChase);
        _playing = onChase;

        if (_playing)
        {
            _trackIndex = Random.Range(0, _tracks.Length);
            _musicPlayer.loop = false;
            _musicPlayer.clip = _tracks[_trackIndex].Entry;
            _musicPlayer.Play();

            if (_midTrackCoroutine != null)
                StopCoroutine(_midTrackCoroutine);
            
            _midTrackCoroutine = StartCoroutine(WaitAndPlayMid(_musicPlayer.clip.length));
        }
        else
        {
            if (_midTrackCoroutine != null)
            {
                StopCoroutine(_midTrackCoroutine);
                _midTrackCoroutine = null;
            }

            _musicPlayer.clip = _tracks[_trackIndex].End;
            _musicPlayer.loop = false;
            _musicPlayer.Play();
        }
    }

    private IEnumerator WaitAndPlayMid(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayMid();
    }

    private void PlayMid()
    {
        _musicPlayer.clip = _tracks[_trackIndex].Mid;
        _musicPlayer.loop = true;
        _musicPlayer.Play();
    }
}

[System.Serializable]
public class ChaseTrack
{
	public AudioClip Entry;
	public AudioClip Mid;
	public AudioClip End;
}
