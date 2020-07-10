//using BehaviorDesigner.Runtime;
//using BehaviorDesigner.Runtime.Tasks;
//using UnityEngine;
//using UnityEngine.Events;

//[TaskCategory("Unity/Animator")]
//[TaskDescription("Waits for an Animation callback.")]
//public class WaitForAnimation : Action
//{
//    public SharedString animationName;

//    private AnimationListener animationListener;
//    private UnityAction<int> onAnimationCompletedCallback;

//    private bool animationCompeted = false;
//    private int ANIM_Name;

//    public override void OnStart()
//    {
//        ANIM_Name = Animator.StringToHash(animationName.Value);
//        animationCompeted = false;

//        animationListener = gameObject.GetComponent<AnimationListener>();
//        onAnimationCompletedCallback = new UnityAction<int>(OnAnimationCompleted);
//        animationListener.addAnimationCompletedCallback(ANIM_Name, onAnimationCompletedCallback);
//    }

//    private void OnAnimationCompleted(int hashCode)
//    {
//        if (hashCode == ANIM_Name)
//        {
//            animationCompeted = true;
//        }
//    }

//    public override TaskStatus OnUpdate()
//    {
//        return (animationCompeted ? TaskStatus.Success : TaskStatus.Running);
//    }

//    public override void OnEnd()
//    {
//        animationListener.removeAnimationCompletedCallback(ANIM_Name, onAnimationCompletedCallback);
//    }

//}
