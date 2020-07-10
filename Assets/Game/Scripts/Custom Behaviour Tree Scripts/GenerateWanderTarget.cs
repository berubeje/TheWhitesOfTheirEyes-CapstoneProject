//using BehaviorDesigner.Runtime;
//using BehaviorDesigner.Runtime.Tasks;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;

//[TaskCategory("Custom")]
//[TaskDescription("Wanders using the navmesh for a certain amount of time")]

//public class GenerateWanderTarget : Action
//{
//    public SharedFloat wanderRadius;

//    private SharedVector3 _wanderTarget;
//    private Vector3 _staringPostion;
//    private bool _started = false;

//    public override void OnAwake()
//    {
//        _staringPostion = transform.position;
//        BehaviorTree tree = GetComponent<BehaviorTree>();
//        _wanderTarget = tree.GetVariable("Wander Target") as SharedVector3;
//        _started = true;
//    }


//    // Update is called once per frame
//    public override TaskStatus OnUpdate()
//    {
//        Vector3 target = Vector3.zero;
//        target = RandomWanderTarget(_staringPostion, wanderRadius.Value);

//        if (target == Vector3.zero)
//        {
//            return TaskStatus.Running;
//        }

//        _wanderTarget.Value = target;
//        return TaskStatus.Success;
//    }

//    public Vector3 RandomWanderTarget(Vector3 origin, float dist)
//    {
//        Vector3 randDirection = Random.insideUnitSphere * dist;
//        randDirection += origin;
//        NavMeshHit navHit;
//        NavMesh.SamplePosition(randDirection, out navHit, dist, -1);

//        return navHit.position;
//    }

//    public override void OnDrawGizmos()
//    {
//        if (_started)
//        {
//            Gizmos.color = Color.red;
//            Gizmos.DrawWireSphere(_staringPostion, wanderRadius.Value);
//        }
//    }
//}
