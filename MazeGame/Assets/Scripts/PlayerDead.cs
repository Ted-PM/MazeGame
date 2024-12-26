using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDead : MonoBehaviour
{
    private AudioSource _playerDead;
    // Start is called before the first frame update
    void Start()
    {
        _playerDead = GetComponent<AudioSource>();
        StartCoroutine(PlayDeathSound());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator PlayDeathSound()
    {
        if (_playerDead != null)
        {
            _playerDead.Play();
            yield return new WaitForSeconds(4);
            _playerDead.Stop();
        }
    }
}
