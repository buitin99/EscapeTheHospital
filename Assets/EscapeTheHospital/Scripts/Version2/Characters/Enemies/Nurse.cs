using System.Collections.Generic;
using UnityEngine;

public class Nurse : Enemy
{
    public GameObject questionMask;
    public GameObject exclanmationMask;
    public GameObject lostKeyMask;
    public GameObject lostEleMask;
    public GameObject tickMask;
    public GameObject xMask;
    public Vector3 Keypos;
    public Vector3 Doorpos;
    private EnemyState state;

    protected override void OnEnable()
    {
        base.OnEnable();
        gameManager.onPlayerDetected.AddListener(StatePatrolDetected);
        gameManager.onPlayerHasKey.AddListener(StateLostKey);
    }

    protected override void Update()
    {
        base.Update();
        //To do
        StateManager();
    }

    protected override void StateManager()
    {
        switch(state)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.PatrolDetected:
                PatrolWhenDetected();
                break;
            case EnemyState.PatrolElectric:
                PatrolWhenLostElectric();
                break;
            case EnemyState.PatrolKey:
                PatrolWhenLostKey();
                break;
        }
    }

    protected override void Idle()
    {
        base.Idle();
        state = EnemyState.Patrol;
    }

    protected override void PatrolWhenDetected()
    {
        agent.SetDestination(pos);
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            state = EnemyState.Idle;
        }
    }


    protected override void PatrolWhenLostElectric()
    {
        throw new System.NotImplementedException();
    }

    protected override void PatrolWhenLostKey()
    {
        agent.SetDestination(pos);
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            state = EnemyState.Idle;
        }
    }


    private void StatePatrolDetected(Vector3 playerDetectedPos)
    {
        pos = playerDetectedPos;
        state = EnemyState.PatrolDetected;
    }

    private void StateLostKey(Vector3 keyPos)
    {
        Debug.Log(123);
        pos = keyPos;
        state = EnemyState.PatrolKey;
    }

    private void HandleWhenDetected(List<RaycastHit> hitList)
    {
        pos = playerScanner.DetectSingleTarget(hitList).position;
        gameManager.EndGame(false);
    }

    protected override void OnEndGame(bool end)
    {
        base.OnEndGame(end);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        gameManager.onPlayerDetected.RemoveListener(StatePatrolDetected);
        gameManager.onPlayerHasKey.RemoveListener(StateLostKey);
    }
}
