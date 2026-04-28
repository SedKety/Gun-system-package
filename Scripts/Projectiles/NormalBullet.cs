using UnityEngine;

public class NormalBullet : MonoBehaviour
{
    private Rigidbody _rb;

    private void Awake() => _rb = GetComponent<Rigidbody>();

    private void Start() => _rb.AddForce(transform.forward * 1000f);
}
