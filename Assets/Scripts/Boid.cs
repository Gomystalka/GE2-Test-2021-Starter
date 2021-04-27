using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private readonly List<SteeringBehaviour> _behaviours = new List<SteeringBehaviour>();

    public Vector3 force = Vector3.zero;
    public Vector3 acceleration = Vector3.zero;
    public Vector3 velocity = Vector3.zero;
    public float mass = 1;

    [Range(0.0f, 10.0f)]
    public float damping = 0.01f;

    [Range(0.0f, 1.0f)]
    public float banking = 0.1f;
    public float maxSpeed = 5.0f;
    public float maxForce = 10.0f;

    protected Seek seek;
    protected Arrive arrive;

    [Header("Custom Settings")]
    public float stoppingDistanceUnits;

    public void OnStart()
    {
        seek = GetComponent<Seek>();
        arrive = GetComponent<Arrive>();

        SteeringBehaviour[] behaviours = GetComponents<SteeringBehaviour>();
        foreach (SteeringBehaviour b in behaviours)
            this._behaviours.Add(b);
    }

    public Vector3 SeekForce(Vector3 target, AxisConstraints constraints, bool ignoreStoppingDistance = true)
    {
        Vector3 desired = target - transform.position;
        desired.x = constraints.x ? 0 : desired.x;
        desired.y = constraints.y ? 0 : desired.y;
        desired.z = constraints.z ? 0 : desired.z;

        if (!ignoreStoppingDistance && desired.magnitude <= stoppingDistanceUnits)
            return Vector3.zero;

        desired.Normalize();
        desired *= maxSpeed;
        return desired - velocity;
    }

    public Vector3 ArriveForce(Vector3 target, AxisConstraints constraints, float slowingDistance = 15.0f)
    {
        Vector3 toTarget = target - transform.position;
        toTarget.x = constraints.x ? 0 : toTarget.x;
        toTarget.y = constraints.y ? 0 : toTarget.y;
        toTarget.z = constraints.z ? 0 : toTarget.z;

        float distance = toTarget.magnitude;
        if (distance <= stoppingDistanceUnits)
            return Vector3.zero;
        float ramped = maxSpeed * (distance / slowingDistance);

        float clamped = Mathf.Min(ramped, maxSpeed);
        Vector3 desired = clamped * (toTarget / distance);

        return desired - velocity;
    }


    Vector3 Calculate()
    {
        force = Vector3.zero;

        // Weighted prioritised truncated running sum
        // 1. Behaviours are weighted
        // 2. Behaviours are prioritised
        // 3. Truncated
        // 4. Running sum


        foreach (SteeringBehaviour b in _behaviours)
        {
            if (b.isActiveAndEnabled)
            {
                force += b.Calculate() * b.weight;

                float f = force.magnitude;
                if (f >= maxForce)
                {
                    force = Vector3.ClampMagnitude(force, maxForce);
                    break;
                }
            }
        }

        return force;
    }

    public void PerformForceCalculations()
    {
        force = Calculate();
        Vector3 newAcceleration = force / mass;
        acceleration = Vector3.Lerp(acceleration, newAcceleration, Time.deltaTime);
        velocity += acceleration * Time.deltaTime;

        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        if (velocity.magnitude > float.Epsilon)
        {
            Vector3 tempUp = Vector3.Lerp(transform.up, Vector3.up + (acceleration * banking), Time.deltaTime * 3f);
            transform.LookAt(transform.position + velocity, tempUp);
            if (force == Vector3.zero) return;

            transform.position += velocity * Time.deltaTime;
            velocity *= (1.0f - (damping * Time.deltaTime));
        }
    }
}

[System.Serializable]
public struct AxisConstraints {
    public bool x;
    public bool y;
    public bool z;
}
