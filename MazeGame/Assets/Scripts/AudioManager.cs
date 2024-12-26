using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _mainGameAudio;
    private AudioSource[] _audioSources;
    // Start is called before the first frame update
    void Start()
    {
        _audioSources = new AudioSource[4];
        _audioSources = _mainGameAudio.GetComponents<AudioSource>();
        PlayMainGameAudio();
    }

    public void PlayMainGameAudio()
    {
        for (int i = 0; i < _audioSources.Length; i++)
        {
            if (_audioSources[i] != null)
            {
                _audioSources[i].Play();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
