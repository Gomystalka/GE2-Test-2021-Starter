using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// FPSController script by Bryan Duggan modified by Tomasz Galka C18740411
/// </summary>
public class FPSController : MonoBehaviour
{
    public float speed = 50.0f;
    public float lookSpeed = 150.0f;
    public bool allowPitch = true;
    public Rigidbody ballPrefab;

    [Header("Input")]
    public KeyCode fireKey = KeyCode.Space;
    public KeyCode quitKey = KeyCode.Escape;
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Throwing")]
    public float throwStrength;

    private float _invcosTheta1;
    private GameObject _camera;
    private IAnimalBehaviour[] _doubutsuTachi;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _camera = Camera.main.gameObject;
        _doubutsuTachi = FindObjectsOfType<Inu>();

        Invoke(nameof(Activate), 2f);
    }

    void Yaw(float angle)
    {
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
        transform.rotation = rot * transform.rotation;
    }

    //void Roll(float angle)
    //{
    //    Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);
    //    transform.rotation = rot * transform.rotation;
    //}

    void Pitch(float angle)
    {        
        float theshold = 0.95f;
        if ((angle > 0 && _invcosTheta1 < -theshold) || (angle < 0 && _invcosTheta1 > theshold))
        {
            return;
        }
        // A pitch is a rotation around the right vector
        Quaternion rot = Quaternion.AngleAxis(angle, transform.right);

        transform.rotation = rot * transform.rotation;
    }

    void Walk(float units)
    {
        Vector3 forward = _camera.transform.forward;
        forward.y = 0; 
        forward.Normalize();
        transform.position += forward * units;
    }

    //void Fly(float units)
    //{
    //    transform.position += Vector3.up * units;
    //}

    void Strafe(float units) => transform.position += _camera.transform.right * units;

    private bool _active = false;
    void Activate() => _active = true;

    void Update()
    {
        if (!_active) return;
  
        float mouseX, mouseY;
        float speed = this.speed;

        _invcosTheta1 = Vector3.Dot(transform.forward, Vector3.up);

        HandleInput();

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        Yaw(mouseX * lookSpeed * Time.deltaTime);
        if (allowPitch)
            Pitch(-mouseY * lookSpeed * Time.deltaTime);

        float contWalk = Input.GetAxis("Vertical");
        float contStrafe = Input.GetAxis("Horizontal");
        Walk(contWalk * speed * Time.deltaTime);
        Strafe(contStrafe * speed * Time.deltaTime);
    }

    private void HandleInput() {
        if (Input.GetKey(quitKey))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }

        if (Input.GetKeyDown(fireKey))
            ThrowBall();
    }

    private void ThrowBall() {
        Rigidbody rb = Instantiate(ballPrefab, transform.position, Quaternion.identity);
        foreach (IAnimalBehaviour animal in _doubutsuTachi)
        {
            animal.Bork();
            animal.Seek(rb.transform, true);
            if(animal is Inu inu)
                StartCoroutine(DelayPickupLogic(3f, inu));
        }
        rb.AddRelativeForce(transform.forward * throwStrength, ForceMode.Impulse);
    }

    private IEnumerator DelayPickupLogic(float delay, Inu inu) {
        inu.CanPickUpObject = false;
        yield return new WaitForSeconds(delay);
        inu.CanPickUpObject = true;
    }
}