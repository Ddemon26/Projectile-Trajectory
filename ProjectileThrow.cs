using UnityEngine;

[RequireComponent(typeof(TrajectoryPredictor))]
public class ProjectileThrow : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField]
    private Rigidbody objectToThrow;

    [SerializeField, Range(0.0f, 50.0f)]
    private float throwForce;

    [SerializeField]
    private Transform startPosition;

    private TrajectoryPredictor trajectoryPredictor;

    private void Awake()
    {
        trajectoryPredictor = GetComponent<TrajectoryPredictor>();
        ValidateComponents();
    }

    private void Update()
    {
        PredictProjectileTrajectory();
    }

    private void PredictProjectileTrajectory()
    {
        trajectoryPredictor.PredictTrajectory(GetProjectileProperties());
    }

    private ProjectileProperties GetProjectileProperties()
    {
        return new ProjectileProperties
        {
            direction = startPosition.forward,
            initialPosition = startPosition.position,
            initialSpeed = throwForce,
            mass = objectToThrow.mass,
            drag = objectToThrow.drag
        };
    }

    // Public method to throw the projectile
    public void ThrowProjectile()
    {
        Rigidbody thrownObject = Instantiate(objectToThrow, startPosition.position, Quaternion.identity);
        thrownObject.AddForce(startPosition.forward * throwForce, ForceMode.Impulse);
    }

    private void ValidateComponents()
    {
        if (startPosition == null)
        {
            startPosition = transform;
        }
    }
}
