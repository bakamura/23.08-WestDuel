using System.Collections;
using UnityEngine;

public class AnimationsUpdate : MonoBehaviour
{
    [SerializeField] private BoneModificationData[] _bonesToUpdate;
    [SerializeField] private float _tickFrequency;
    [SerializeField, Min(1f)] private float _speed;
    [SerializeField] private ServerLocalInputReader _inputReader;
    private WaitForSeconds _delay;
    private bool _canUpdate = true;
    private Vector3 _direction;
    private Vector3 _mousePosition;
    private Animator _animator;
    private bool _canShoot;
    private Coroutine _updateCoroutine;
    //bool _fliped = false;

    [System.Serializable]
    private struct BoneModificationData
    {
        public GameObject Bone;
        public CalculationMethod BodyPartType;
        public bool ConstrainXAxis;
        public bool ConstrainYAxis;
        public bool ConstrainZAxis;
        [Range(0f, 360f)] public float MinAngleToFlip;
        [Range(0f, 360f)] public float MaxAngleToFlip;
    }

    private enum CalculationMethod
    {
        DirectionalInput,
        MouseInput
    }

    private void Awake()
    {
        _delay = new WaitForSeconds(_tickFrequency);
        _animator = GetComponent<Animator>();

        if (_canUpdate) _updateCoroutine = StartCoroutine(UpdateAnimations());
    }

    private void Start() {
        if(ServerPlayerInfo.player.Count > 0) transform.parent.GetComponent<PlayerShoot>().onShoot.AddListener(TriggerShootAnim);
    }

    private IEnumerator UpdateAnimations()
    {
        while (_canUpdate)
        {
            Vector3 direction;
            float dotProduct;
            float movmentSpeed = 0;
            Quaternion finalRotation;
            bool fliped = false;

            if (_inputReader != null)
            {
                SetMousePosition(_inputReader.ShootInput);
                SetDirection(_inputReader.MovementInput);
            }

            for (int i = 0; i < _bonesToUpdate.Length; i++)
            {
                switch (_bonesToUpdate[i].BodyPartType)
                {
                    case CalculationMethod.DirectionalInput:
                        direction = new Vector3(_direction.x, 0, _direction.y).normalized;
                        movmentSpeed = direction.sqrMagnitude;
                        dotProduct = Mathf.Abs(Vector3.Dot(_bonesToUpdate[i].Bone.transform.right, direction));
                        if (direction != Vector3.zero)
                        {
                            finalRotation = Quaternion.RotateTowards(_bonesToUpdate[i].Bone.transform.rotation, Quaternion.LookRotation(direction), _speed * dotProduct);
                            finalRotation = new Quaternion(_bonesToUpdate[i].ConstrainXAxis ? 0 : finalRotation.x,
                                _bonesToUpdate[i].ConstrainYAxis ? 0 : finalRotation.y,
                                _bonesToUpdate[i].ConstrainZAxis ? 0 : finalRotation.z,
                                finalRotation.w);
                            _bonesToUpdate[i].Bone.transform.rotation = finalRotation;
                            fliped = finalRotation.eulerAngles.y > _bonesToUpdate[i].MinAngleToFlip && finalRotation.eulerAngles.y < _bonesToUpdate[i].MaxAngleToFlip;
                        }
                        break;
                    case CalculationMethod.MouseInput:
                        direction = (_mousePosition - _bonesToUpdate[i].Bone.transform.position).normalized;
                        dotProduct = Mathf.Abs(Vector3.Dot(_bonesToUpdate[i].Bone.transform.right, direction));
                        finalRotation = Quaternion.RotateTowards(_bonesToUpdate[i].Bone.transform.rotation, Quaternion.LookRotation(direction), _speed * dotProduct);
                        finalRotation = new Quaternion(_bonesToUpdate[i].ConstrainXAxis ? 0 : finalRotation.x,
                            _bonesToUpdate[i].ConstrainYAxis ? 0 : finalRotation.y,
                            _bonesToUpdate[i].ConstrainZAxis ? 0 : finalRotation.z,
                            finalRotation.w);
                        _bonesToUpdate[i].Bone.transform.rotation = finalRotation;
                        //if (direction != Vector3.zero)
                        //{
                        //    if (!fliped) fliped = _bonesToUpdate[i].Bone.transform.rotation.eulerAngles.y > _bonesToUpdate[i].MinAngleToFlip && _bonesToUpdate[i].Bone.transform.rotation.eulerAngles.y < _bonesToUpdate[i].MaxAngleToFlip;
                        //}
                        break;
                }
            }
            if (fliped)
            {
                _bonesToUpdate[0].Bone.transform.root.transform.eulerAngles = new Vector3(
                                    _bonesToUpdate[0].Bone.transform.root.transform.eulerAngles.x,
                                    180,
                                    _bonesToUpdate[0].Bone.transform.root.transform.eulerAngles.z);
            }
            else
            {
                _bonesToUpdate[0].Bone.transform.root.transform.eulerAngles = new Vector3(
                                    _bonesToUpdate[0].Bone.transform.root.transform.eulerAngles.x,
                                    0,
                                    _bonesToUpdate[0].Bone.transform.root.transform.eulerAngles.z);
            }
            _animator.SetFloat("VELOCITY", movmentSpeed);
            if (_canShoot)
            {
                _animator.SetTrigger("SHOOT");
                _canShoot = false;
            }
            yield return _delay;
        }
        _updateCoroutine = null;
    }

    public void SetDirection(Vector3 pos)
    {
        _direction = pos;
    }

    public void SetMousePosition(Vector3 pos)
    {
        _mousePosition = pos;
    }

    [ContextMenu("Shoot")]
    public void TriggerShootAnim()
    {
        _canShoot = true;
    }

    public Vector3 GetMousePosition()
    {
        return _mousePosition;
    }

    public Vector3 GetDirection()
    {
        return _direction;
    }

    public void UpdateState(bool canUpdate)
    {
        _canUpdate = canUpdate;
        if (_updateCoroutine == null && canUpdate)
        {
            _updateCoroutine = StartCoroutine(UpdateAnimations());
        }
    }
}
