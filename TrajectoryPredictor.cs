using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPredictor : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField, Tooltip("The marker will show where the projectile will hit")]
    private Transform hitMarker;

    [SerializeField, Range(10, 100), Tooltip("The maximum number of points for the trajectory")]
    private int maxPoints = 50;

    [SerializeField, Range(0.01f, 0.5f), Tooltip("The time increment for trajectory calculation")]
    private float timeIncrement = 0.025f;

    [SerializeField, Range(1.05f, 2f), Tooltip("Raycast overlap multiplier for trajectory points")]
    private float rayOverlap = 1.1f;

    private LineRenderer trajectoryLine;

    private void Awake()
    {
        trajectoryLine = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        SetTrajectoryVisible(true);
    }

    /// <summary>
    /// Predicts and displays the projectile's trajectory.
    /// </summary>
    public void PredictTrajectory(ProjectileProperties projectile)
    {
        Vector3 velocity = CalculateInitialVelocity(projectile);
        Vector3 position = projectile.initialPosition;

        for (int i = 0; i < maxPoints; i++)
        {
            velocity = UpdateVelocity(velocity, projectile.drag);
            Vector3 nextPosition = CalculateNextPosition(position, velocity);

            if (CheckForCollision(position, nextPosition, velocity, out RaycastHit hit))
            {
                UpdateTrajectory(i, hit.point);
                PositionHitMarker(hit);
                break;
            }

            UpdateTrajectory(i, position);
            position = nextPosition;
        }
    }

    /// <summary>
    /// Sets the visibility of the trajectory and hit marker.
    /// </summary>
    public void SetTrajectoryVisible(bool visible)
    {
        trajectoryLine.enabled = visible;
        hitMarker.gameObject.SetActive(visible);
    }

    // Helper methods (private)

    private Vector3 CalculateInitialVelocity(ProjectileProperties projectile)
    {
        return projectile.direction * (projectile.initialSpeed / projectile.mass);
    }

    private Vector3 UpdateVelocity(Vector3 velocity, float drag)
    {
        velocity += Physics.gravity * timeIncrement;
        velocity *= Mathf.Clamp01(1f - drag * timeIncrement);
        return velocity;
    }

    private Vector3 CalculateNextPosition(Vector3 position, Vector3 velocity)
    {
        return position + velocity * timeIncrement;
    }

    private bool CheckForCollision(Vector3 position, Vector3 nextPosition, Vector3 velocity, out RaycastHit hit)
    {
        float overlap = Vector3.Distance(position, nextPosition) * rayOverlap;
        return Physics.Raycast(position, velocity.normalized, out hit, overlap);
    }

    private void UpdateTrajectory(int pointIndex, Vector3 position)
    {
        trajectoryLine.positionCount = pointIndex + 1;
        trajectoryLine.SetPosition(pointIndex, position);
    }

    private void PositionHitMarker(RaycastHit hit)
    {
        hitMarker.gameObject.SetActive(true);
        hitMarker.position = hit.point + hit.normal * 0.025f; // Offset marker from the surface
        hitMarker.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
    }
}
