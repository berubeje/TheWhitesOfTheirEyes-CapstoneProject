using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JimController : IPlayerControllable
{
    public float rotationSpeed;
    public float turnAroundThreshold;
    public float directionDampTime;
    public float directionSpeed;

    private Animator _jimAnimator;

    private Vector2 _leftStickInput;
    private Vector2 _rightStickInput;

    private Vector3 moveDirection;
    private Vector3 leftStickDirection;

    private bool _isPivoting;
    void Start()
    {
        _jimAnimator = GetComponent<Animator>();
    }

    void Update()
    {
    }

   
    public override void LeftAnalogStick()
    {
        _leftStickInput.x = Input.GetAxis("Left Horizontal");
        _leftStickInput.y = Input.GetAxis("Left Vertical");

        _jimAnimator.SetFloat("leftInputMagnitude", _leftStickInput.magnitude, 0.15f, Time.deltaTime);

        leftStickDirection = new Vector3(_leftStickInput.x, 0.0f, _leftStickInput.y);

        // Get players forward and kill the y value
        Vector3 playerDirection = transform.forward;
        playerDirection.y = 0.0f;

        // Create rotation from the players forward to the direction the joystick is being held
        Quaternion referenceShift = Quaternion.FromToRotation(playerDirection, leftStickDirection);

        // Convert joystick input to world space
        moveDirection = referenceShift * leftStickDirection;

        // y value of this vector is used to figure out if the direction is left or right of the player
        Vector3 axisSign = Vector3.Cross(moveDirection, transform.forward);

        float direction = Vector3.Angle(transform.forward, moveDirection) * (axisSign.y > 0 ? -1.0f : 1.0f);
        direction /= 180.0f;
        direction *= directionSpeed;

        _jimAnimator.SetFloat("direction", direction, directionDampTime, Time.deltaTime);

        // Directly rotate the player if the joystick is moving 
        if (_leftStickInput != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(leftStickDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);
        }
    }

    public override void RightAnalogStick()
    {
        _rightStickInput.x = Input.GetAxis("Right Horizontal");
        _rightStickInput.y = Input.GetAxis("Right Vertical");
    }


    private void OnDrawGizmos()
    {
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), moveDirection, Color.red);
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), leftStickDirection, Color.green);
    }
}
