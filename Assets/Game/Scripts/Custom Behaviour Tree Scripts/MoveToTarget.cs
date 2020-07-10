//using BehaviorDesigner.Runtime;
//using BehaviorDesigner.Runtime.Tasks;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;

//public class MoveToTarget : Action
//{

//    private SharedVector3 _target;
//    private NavMeshAgent _agent;
//    private Animator _animator;
//    private bool _started = false;


//    // Start is called before the first frame update
//    public override void OnAwake()
//    {
//        BehaviorTree tree = GetComponent<BehaviorTree>();
//        _target = tree.GetVariable("Wander Target") as SharedVector3;
//        _agent = GetComponent<NavMeshAgent>();
//        _animator = GetComponent<Animator>();
//        _started = true;
//    }

//    public override void OnStart()
//    {
//        _agent.SetDestination(_target.Value);
//        _agent.isStopped = false;
//        _animator.SetFloat("Speed", 1.0f);
//    }

//    // Update is called once per frame
//    public override TaskStatus OnUpdate()
//    {
//        Vector3 bossPosition = transform.position;
//        bossPosition.y = _target.Value.y;

//        if (Vector3.Distance(bossPosition, _target.Value) <= _agent.stoppingDistance)
//        {
//            _agent.isStopped = true;
//            _animator.SetFloat("Speed", 0.0f);

//            return TaskStatus.Success;
//        }

//        _agent.speed = (_animator.deltaPosition / Time.deltaTime).magnitude;

//        return TaskStatus.Running;
//    }

//    public override void OnDrawGizmos()
//    {
//        if(_started)
//        {
//            Gizmos.color = Color.blue;
//            Gizmos.DrawWireSphere(_target.Value, 2.0f);
//        }

//    }
//}
