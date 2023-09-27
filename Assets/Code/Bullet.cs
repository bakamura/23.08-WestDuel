//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Bullet : MonoBehaviour {
//    [SerializeField] private float _tickFrequency;
//    [SerializeField] private float _speed;
//    [SerializeField] private SoundManager.SfxAudioData _audioData;

//    //[SerializeField] private Vector3 _dir;
//    //[SerializeField] private Transform _init;

//    //private WaitForSeconds _delay;
//    //private Coroutine _moveCoroutine;
//    //private Vector3 _currentDirection;
//    [HideInInspector] public Transform owner;
//    private Rigidbody _rb;
//    private PlayerHealth _adversaryPlayerHealth;
//    private Collider _adversaryPlayerCol;

//    private void Awake()
//    {
//        //_delay = new WaitForSeconds(_tickFrequency);
//        _rb = GetComponent<Rigidbody>();
//    }

//    private void Start()
//    {
//        PlayerHealth[] pHArr = FindObjectsOfType<PlayerHealth>();
//        foreach(PlayerHealth ph in pHArr)
//        {
//            if(ph.gameObject.name != owner.name)
//            {
//                _adversaryPlayerHealth = ph;
//                _adversaryPlayerCol = ph.GetComponent<Collider>();
//            }
//        }
//    }

//    public void Shoot(Vector3 initialPos, Vector3 direction) {
//        transform.position = initialPos;
//        //_currentDirection = direction;
//        if (this.isActiveAndEnabled) _rb.velocity = direction * _speed;
//        //if(_moveCoroutine == null) _moveCoroutine = StartCoroutine(Movment());
//    }

//    //IEnumerator Movment()
//    //{
//    //    while (this.isActiveAndEnabled)
//    //    {
//    //        transform.position += _speed * _tickFrequency * _currentDirection;
//    //        yield return _delay;
//    //    }
//    //    _moveCoroutine = null;
//    //}

//    private void OnCollisionEnter(Collision collision)
//    {
//        if(collision.collider == _adversaryPlayerCol) _adversaryPlayerHealth.Die();
//        gameObject.SetActive(false);
//        _rb.velocity = Vector3.zero;
//    }

//    public void UpdateState(bool isActive)
//    {
//        SoundManager.Instance.PlaySoundEffect(_audioData, transform.position);
//        gameObject.SetActive(isActive);
//        if(!isActive) _rb.velocity = Vector3.zero; 
//    }
//    //[ContextMenu("Test")]
//    //private void DebugShoot()
//    //{
//    //    gameObject.SetActive(true);
//    //    Shoot(_init.position, _dir);
//    //}

//}