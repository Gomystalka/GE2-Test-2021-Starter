using UnityEngine;

public class Arrive : SteeringBehaviour
{
    public Vector3 targetPosition = Vector3.zero;
    public float slowingDistance = 15.0f;

    public GameObject targetGameObject = null;

    public override Vector3 Calculate() => boid.ArriveForce(targetPosition, constraints, slowingDistance);

    public void Update()
    {
        if (targetGameObject != null)
            targetPosition = targetGameObject.transform.position;
    }
}