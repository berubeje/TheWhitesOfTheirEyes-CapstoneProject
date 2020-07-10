//using BehaviorDesigner.Runtime;
//using BehaviorDesigner.Runtime.Tasks;
//using UnityEngine;

//public class TurnToTarget : MonoBehaviour
//{
//    public SharedFloat turnSpeed;
//    public SharedBool turnToPlayer;
//    public SharedFloat angleDeadzone = 2;

//    private Animator _animator;
//    private JimController _player;
//    private SharedString _playerSide;
//    private Vector3 _target;
//    private float _speed;


//    public override void OnAwake()
//    {
//        _animator = GetComponent<Animator>();
//        _player = GetComponent<BossPlayerDetectionLogic>().player;
//        _playerSide = GetComponent<BehaviorTree>().GetVariable("Side Detected") as SharedString;
//    }

//    // Start is called before the first frame update
//    public override void OnStart()
//    {
//        if(turnToPlayer.Value)
//        {
//            Vector3 targetDireciton = _player.transform.position - transform.position;
//            targetDireciton.y = transform.position.y;
//            _target = targetDireciton;

//            if(_playerSide.Value == "Left")
//            {
//                _animator.SetBool("Turn Left", true);
//            }
//            else
//            {
//                _animator.SetBool("Turn Right", true);
//            }
//        }


//        _speed = turnSpeed.Value * Time.deltaTime;
//    }

//    // Update is called once per frame
//    public override TaskStatus OnUpdate()
//    {
//        if(turnToPlayer.Value)
//        {
//            Vector3 newDirection = Vector3.RotateTowards(transform.forward, _target, _speed, 0.0f);
//            Debug.DrawRay(transform.position, newDirection, Color.red);
//            transform.rotation = Quaternion.LookRotation(newDirection);

//            if(Vector3.Angle(transform.forward, _player.transform.position - transform.position) < angleDeadzone.Value)
//            {

//            }
//        }
//        else
//        {

//        }
//    }
//}
