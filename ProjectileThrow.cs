using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Input Settings")]
    public InputAction fireAction;

    private TrajectoryPredictor trajectoryPredictor;

    private void Awake()
    {
        trajectoryPredictor = GetComponent<TrajectoryPredictor>();
        ValidateComponents();
    }

    private void OnEnable()
    {
        fireAction.Enable();
        fireAction.performed += ThrowObject;
    }

    private void OnDisable()
    {
        fireAction.Disable();
        fireAction.performed -= ThrowObject;
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

    private void ThrowObject(InputAction.CallbackContext context)
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
