using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class FPSController : MonoBehaviour
{
    public GameObject mainCamera;
    public float speed = 50.0f;
    public float lookSpeed = 150.0f;
    public bool allowPitch = true;

    private float _invcosTheta1;
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (mainCamera == null)
            mainCamera = Camera.main.gameObject;

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
        Vector3 forward = mainCamera.transform.forward;
        forward.y = 0; 
        forward.Normalize();
        transform.position += forward * units;
    }

    //void Fly(float units)
    //{
    //    transform.position += Vector3.up * units;
    //}

    void Strafe(float units) => transform.position += mainCamera.transform.right * units;

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
        if (Input.GetKey(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }

        if (Input.GetKey(KeyCode.LeftShift))
            speed *= 5.0f;
    }
}