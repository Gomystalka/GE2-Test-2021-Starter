using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inu : Boid, IAnimalBehaviour
{
    public AudioClip[] bork;
    private AudioSource _source;
    private TailBehaviour _tailBehaviour;
    private Transform _playerTarget;

    private Transform _pickedUpObject;

    private void Start()
    {
        OnStart();
        seek = GetComponent<Seek>();
        arrive = GetComponent<Arrive>();
        _tailBehaviour = GetComponentInChildren<TailBehaviour>();
        _source.GetComponent<AudioSource>();
    }

    void Update()
    {
        PerformForceCalculations();
    }

    private IEnumerator PlaySound(AudioClip clip, byte playCount, bool waitForEnd, float delay = 1f) {
        byte index = 0;
        _source.clip = clip;
        while (index < playCount) {
            if (waitForEnd && _source.isPlaying) continue;
            if (waitForEnd)
                _source.Play();
            else
            {
                yield return new WaitForSeconds(delay);
                _source.PlayOneShot(clip);
            }
            index++;
            yield return new WaitForEndOfFrame();
        }
    }

    public void Bork()
    {
        StartCoroutine(PlaySound(bork[Random.Range(0, bork.Length)], 2, false));
    }

    public void Seek(Transform target)
    {
        seek.targetGameObject = target.gameObject;
    }

    public void Return()
    {
        seek.targetGameObject = null;
        arrive.targetGameObject = _playerTarget.gameObject;
    }

    public void PickupObject(Transform transform) {

    }

    public void DropObject(Transform transform) { 
        
    }
}

public interface IAnimalBehaviour {
    void Bork();
    void Seek(Transform target);
    void Return();
    void PickupObject(Transform transform);
    void DropObject(Transform transform);
}

public enum AnimalState { 
    Seeking,
    Returning
}