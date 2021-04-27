using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inu : Boid, IAnimalBehaviour
{
    [Header("SFX")]
    public AudioClip[] borks;

    [Header("References")]
    public Transform playerTarget;

    private AudioSource _source;
    private TailBehaviour _tailBehaviour;

    private Transform _pickedUpObject;

    private AnimalState _currentState;
    private AnimalState _lastState;

    private void Start()
    {
        OnStart();
        seek = GetComponent<Seek>();
        arrive = GetComponent<Arrive>();
        _tailBehaviour = GetComponentInChildren<TailBehaviour>();
        _source = GetComponent<AudioSource>();
        Idle();
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
        _currentState = AnimalState.Bork;
        StartCoroutine(PlaySound(borks[Random.Range(0, borks.Length)], 2, false));
    }

    public void Seek(Transform target)
    {
        if (_currentState != AnimalState.Seek)
        {
            _lastState = _currentState;
            _currentState = AnimalState.Seek;
        }
        arrive.targetGameObject = null;
        seek.targetGameObject = target.gameObject;
    }

    public void Return()
    {
        if (_currentState != AnimalState.Return)
        {
            _lastState = _currentState;
            _currentState = AnimalState.Return;
        }
        seek.targetGameObject = null;
        arrive.targetGameObject = playerTarget.gameObject;
    }

    public void PickupObject(Transform targetObject) {
        if (_currentState != AnimalState.Pickup)
        {
            _lastState = _currentState;
            _currentState = AnimalState.Pickup;
        }
        if (_pickedUpObject)
            _pickedUpObject.SetParent(null); //Drop current object if another one is picked up.

        _pickedUpObject = transform;
        _pickedUpObject.SetParent(transform);
    }

    public void DropObject() {
        if (_currentState != AnimalState.Drop)
        {
            _lastState = _currentState;
            _currentState = AnimalState.Drop;
        }
        if (_pickedUpObject)
            _pickedUpObject.SetParent(null);
    }

    public void OnArriveAtTarget(Transform target) {
        if (_currentState != AnimalState.Arrive)
        {
            _lastState = _currentState;
            _currentState = AnimalState.Arrive;
        }
        if (!_pickedUpObject)
            PickupObject(target);
        else
            DropObject();
    }

    public void Idle() {
        _currentState = AnimalState.Idle;
    }
}

public interface IAnimalBehaviour {
    void Bork();
    void Seek(Transform target);
    void Return();
    void PickupObject(Transform transform);
    void DropObject();
    void OnArriveAtTarget(Transform transform);
    void Idle();
}

public enum AnimalState { 
    Bork,
    Seek,
    Return,
    Pickup,
    Drop,
    Arrive,
    Idle
}