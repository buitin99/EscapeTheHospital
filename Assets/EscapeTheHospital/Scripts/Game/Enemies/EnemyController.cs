using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{

private enum State
{
    IDLE,
    PATROL,
    ENTER,
    WARNING,
    ALL
}
public enum TypePatrol 
{
    STANDINPLACE,
    MOVEAROUND
}
    private int _patrolIndex = 0;
    private Vector3 _playerPosition;
    private Transform _player;
    private float _speed;
    private int _velocityHash;
    private NavMeshAgent _agent;
    private State _state, _preState;
    private float _idleTime;
    private GameObject _fieldOfView;
    private Animator _animator;

    private GameManager _gameManager;

    private bool _isStartGame;
    [SerializeField]
    private Scanner _playerScanner = new Scanner();
    public Vector3 standPos;
    public TypePatrol typePatrol;
    public Vector3[] patrolList;
    public Transform rootScanner;
    [Range(0, 360)]
    public float detectionAngle;
    public float viewDistance;
    public GameObject questionMask;

    private void Awake() 
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _velocityHash = Animator.StringToHash("Velocity");
        _gameManager = GameManager.Instance;
    }

    private void OnEnable() 
    {
        _playerScanner.OnDetectedTarget.AddListener(HandleWhenDetected);
        _gameManager.onStart.AddListener(OnStartGame);
    }
    // Start is called before the first frame update
    void Start()
    {
        _fieldOfView = _playerScanner.CreataFieldOfView(rootScanner, rootScanner.position,detectionAngle,viewDistance);
        // GameEventManager.Instance.onDocumentTriggerEnter += StatePatrolEnter;
        // GameManager.Instance.onPlayerDetected.AddListener(StatePatrolEnter);
        _gameManager.onPlayerDetected.AddListener(StatePatrolEnter);
    }



    // Update is called once per frame
    void Update()
    {
        _playerScanner.Scan();
        if (!_isStartGame)
        {
            return;
        }
        StateManager();
        HandleAnimation();
    }

    private void StateManager()
    {
        switch(_state)
        {
            case State.IDLE:
                Patrol();
                _preState = _state;
                break;
            case State.PATROL:
                _preState = _state;
                Patrol();
                break;
            case State.ENTER:
                PatrolEnter();
                break;
            case State.WARNING:
                PatrolWarning();
                break;
        }
    }

    private void Idle()
    {
        _idleTime += Time.deltaTime;
        _agent.SetDestination(transform.position);
    }

    private void Patrol()
    {
        if (patrolList != null && patrolList.Length > 0)
        {
            Vector3 patrolPoint = patrolList[_patrolIndex];
            switch (typePatrol)
            {   
                case TypePatrol.STANDINPLACE:
                    _agent.SetDestination(standPos);
                    if (_agent.remainingDistance <= _agent.stoppingDistance)
                    _idleTime += Time.deltaTime;
                    transform.rotation = LerpRotation(patrolPoint, transform.position, 10f); 
                    {
                        if (_idleTime > 2)
                        {
                            _patrolIndex++;
                            if (_patrolIndex >= patrolList.Length)
                            {
                                _patrolIndex = 0;
                            }
                            _idleTime = 0;
                        }  
                    }
                    break;
                case TypePatrol.MOVEAROUND:
                    if (_agent.remainingDistance <= _agent.stoppingDistance)
                    {   
                        _idleTime += Time.deltaTime;
                        if (_idleTime > 2)
                        {
                            _patrolIndex++;
                            if (_patrolIndex >= patrolList.Length)
                            {
                                _patrolIndex = 0;
                            }
                            _agent.SetDestination(patrolPoint);
                            _idleTime = 0;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void PatrolEnter()
    {
        _agent.SetDestination(_playerPosition);
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            _state = State.PATROL;
        }
    }


    private void PatrolWarning()
    {

    }

    private void StatePatrolEnter(Vector3 pos)
    {
        _playerPosition = pos;
        _state = State.ENTER;
    }

    private Quaternion LerpRotation(Vector3 pos1, Vector3 pos2, float speed)
    {
        Vector3 dirLook = pos1 - pos2;
        Quaternion rotLook = Quaternion.LookRotation(dirLook.normalized);
        rotLook.x = 0;
        rotLook.z = 0;
        return Quaternion.Lerp(transform.rotation, rotLook, speed*Time.deltaTime);
    }

    public void HandleWhenDetected(List<RaycastHit> hitList) {
        _player = _playerScanner.DetectSingleTarget(hitList);
        _playerPosition = _player.position;
        // _playerPosition = _playerScanner.DetectSingleTarget(hitList).position;
        GameManager.Instance.EndGame(false);
    }

    private void HandleAnimation() 
    {
        Vector3 horizontalVelocity = new Vector3(_agent.velocity.x, 0, _agent.velocity.z);
        float Velocity = horizontalVelocity.magnitude/3;
        if(Velocity > 0) {
            _animator.SetFloat(_velocityHash, Velocity);
        } else {
            float v = _animator.GetFloat(_velocityHash);
            v = Mathf.Lerp(v, -0.1f, 20f * Time.deltaTime);
            _animator.SetFloat(_velocityHash, v);
        }
    }

    private void OnStartGame()
    {
        _isStartGame = true;
    }

    private void OnDisable() 
    {
        _playerScanner.OnDetectedTarget.RemoveListener(HandleWhenDetected);
        _gameManager.onStart.RemoveListener(OnStartGame);
    }
}

