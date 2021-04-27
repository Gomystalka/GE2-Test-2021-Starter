using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inu : Boid, IAnimalBehaviour
{
    [Header("SFX")]
    public AudioClip[] borks;

    [Header("References")]
    public Transform playerTarget;
    public Transform ballParent;

    private AudioSource _source;
    private TailBehaviour _tailBehaviour;

    private Transform _pickedUpObject;

    private AnimalState _currentState;
    private AnimalState _lastState;
    public bool CanPickUpObject { get; set; }
    public TMPro.TextMeshProUGUI stateDisplay;

    private void Start()
    {
        OnStart();
        seek = GetComponent<Seek>();
        arrive = GetComponent<Arrive>();
        _tailBehaviour = GetComponentInChildren<TailBehaviour>();
        _source = GetComponent<AudioSource>();
        Idle(false);
    }

    void Update()
    {
        PerformForceCalculations();
        stateDisplay.text = $"{_currentState}";
    }

    private IEnumerator PlaySound(AudioClip clip, byte playCount, bool waitForEnd, float delay = 0.2f) {
        byte index = 0;
        _source.clip = clip;
        while (index < playCount) {
            if (waitForEnd && _source.isPlaying) continue;
            if (waitForEnd)
                _source.Play();
            else
            {
                _source.PlayOneShot(clip);
                yield return new WaitForSeconds(delay);
            }
            index++;
            yield return new WaitForEndOfFrame();
        }
    }

    public void Bork()
    {
        _currentState = AnimalState.Bork;
        StartCoroutine(PlaySound(borks[Random.Range(0, borks.Length)], 1, false, 0.2f));
    }

    public void Seek(Transform target, bool pickupObject)
    {
        if (_currentState != AnimalState.Seek)
        {
            _lastState = _currentState;
            _currentState = AnimalState.Seek;
            seek.enabled = true;
            arrive.targetGameObject = null;
            arrive.enabled = false;
            seek.targetGameObject = target.gameObject;
        }
    }

    public void Return()
    {
        if (_currentState != AnimalState.Return)
        {
            _lastState = _currentState;
            _currentState = AnimalState.Return;
            arrive.enabled = true;
            seek.targetGameObject = null;
            seek.enabled = false;
            arrive.targetGameObject = playerTarget.gameObject;
        }
    }

    public void PickupObject(Transform targetObject) {
        if (_currentState != AnimalState.Pickup)
        {
            _lastState = _currentState;
            _currentState = AnimalState.Pickup;
            Return();
            if (_pickedUpObject)
                _pickedUpObject.SetParent(null); //Drop current object if another one is picked up.

            _pickedUpObject = targetObject;
            Rigidbody rb = _pickedUpObject.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                rb.isKinematic = true;
            }
            _pickedUpObject.SetParent(ballParent);
            _pickedUpObject.localPosition = Vector3.zero;
        }
    }

    public void TryDropObject()
    {
        if (!_pickedUpObject) return;
        if (_currentState != AnimalState.Drop)
        {
            _lastState = _currentState;
            _currentState = AnimalState.Drop;
            _pickedUpObject.SetParent(null);
            Rigidbody rb = _pickedUpObject.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = false;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
            _pickedUpObject = null;
            Idle(true);
        }
    }

    public void Idle(bool performIdleAction) {
        _currentState = AnimalState.Idle;
        if(performIdleAction)
            StartCoroutine(PlaySound(borks[Random.Range(0, borks.Length)], 2, false, 0.5f));
    }

    protected override void OnArriveTargetReached() {
        TryDropObject();
        Idle(false);
    }

    protected override void OnSeekTargetReached()
    {
        if (CanPickUpObject)
            PickupObject(seek.targetGameObject.transform);
    }

    protected override void OnVelocityCalculated(Vector3 force, Vector3 acceleration, Vector3 velocity)
    {
        _tailBehaviour.wagRate = velocity.magnitude;
    }
}

public interface IAnimalBehaviour {
    void Bork();
    void Seek(Transform target, bool pickup);
    void Return();
    void PickupObject(Transform transform);
    void TryDropObject();
    void Idle(bool performIdleAction);
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