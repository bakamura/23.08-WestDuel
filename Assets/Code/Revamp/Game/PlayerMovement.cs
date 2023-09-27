using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [Header("Parameters")]

    [SerializeField] private float _baseSpeed;
    [SerializeField] private float _noBulletSpeed;
    private bool _canInput;

    [Header("Cache")]

    private Vector2 _input;
    private Rigidbody _rb;
    private PlayerShoot _shootScript;
    private Transform _cam; // Not needed?

    private void Awake() {
        _rb = GetComponent<Rigidbody>();
        _shootScript = GetComponent<PlayerShoot>();
        _cam = Camera.main.transform;

        _canInput = true; //testing
    }

    private void Update() {
        Move(_input.normalized);
    }

    public void SetInputDirection(Vector2 direction) {
        _input = direction;
    }

    private void Move(Vector2 direction) {
        _rb.velocity = _canInput && _input != Vector2.zero ?
            (Quaternion.Euler(0, Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + _cam.eulerAngles.y, 0) * Vector3.forward).normalized * (_shootScript.CheckBullet() ? _baseSpeed : _noBulletSpeed) :
            Vector3.zero;
    }

    public void SetActive(bool isActive) {
        _canInput = isActive;
    }

}
