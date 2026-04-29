using UnityEngine;

/// <summary>
/// Normal bullet, no special behavior, just a simple projectile that moves forward with a constant speed.
/// TODO: Lifetime, damage, collision detection and other stuff that a normal bullet should have.
/// </summary>
public class NormalBullet : MonoBehaviour
{
    private Rigidbody _rb;

    private void Awake() => _rb = GetComponent<Rigidbody>();

    private void Start() => _rb.AddForce(transform.forward * 1000f);
}
