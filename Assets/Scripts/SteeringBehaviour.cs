using UnityEngine;

public abstract class SteeringBehaviour : MonoBehaviour
{
    [Header("Physics")]
    public float weight = 1.0f;
    public Vector3 force;

    [Header("Custom Settings")]
    public AxisConstraints constraints;

    [HideInInspector]
    public Boid boid;

    public void Awake() => boid = GetComponent<Boid>();

    public abstract Vector3 Calculate();
}
