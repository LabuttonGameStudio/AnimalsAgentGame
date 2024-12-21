using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmadilloBallState : MovementState
{
    ArmadilloMovementController movementCtrl;
    MovementFormStats stats;
    Vector3 previousVelocityInput = Vector3.zero;
    private Vector3 velocity = Vector3.zero;

    private Vector3 currentVelocity = Vector3.zero;

    private Transform playerVisual;

    private bool isPlayerRollingAudio;
    public override void EnterState(ArmadilloMovementController movementControl)
    {
        stats = movementControl.ballFormStats;
        movementCtrl = movementControl;
        movementCtrl.rb.constraints = RigidbodyConstraints.None;
        ArmadilloPlayerController.Instance.audioControl.StopMovingAudio();
        ArmadilloPlayerController.Instance.hpControl.currentShield = ArmadilloPlayerController.Instance.ballShieldAmount;
        ArmadilloPlayerController.Instance.hpControl.UpdateHealthBar();
        shieldCheck_Ref = ArmadilloPlayerController.Instance.StartCoroutine(ShieldCheck_Coroutine());
    }


    public override void FixedUpdateState()
    {
        currentVelocity = movementCtrl.rb.velocity;
        MovePlayer();
    }

    public override void UpdateState()
    {
        movementCtrl.rb.useGravity = !movementCtrl.isOnSlope;
        SpeedControl();
    }
    public override void ExitState()
    {
        if (shieldCheck_Ref != null)
        {
            ArmadilloPlayerController.Instance.StopCoroutine(shieldCheck_Ref);
            shieldCheck_Ref = null;
            ArmadilloPlayerController.Instance.hpControl.currentShield = 0;
            ArmadilloPlayerController.Instance.hpControl.UpdateHealthBar();
        }
        previousVelocityInput = Vector3.zero;
        velocity = Vector3.zero;
        isPlayerRollingAudio = false;
        ArmadilloPlayerController.Instance.audioControl.onBallRoll.Stop();
    }
    public void OnBreakObject()
    {
        movementCtrl.rb.velocity = currentVelocity;
    }
    //-----Player Movement-----
    private void MovePlayer()
    {
        Camera mainCamera = ArmadilloPlayerController.Instance.cameraControl.mainCamera;
        Vector3 inputDirection = mainCamera.transform.forward * movementCtrl.movementInputVector.y
            + mainCamera.transform.right * movementCtrl.movementInputVector.x;
        Vector3 moveDirection = inputDirection;
        moveDirection.y = 0;

        if (movementCtrl.grounded)
        {
            if (movementCtrl.isOnSlope) moveDirection = movementCtrl.GetSlopeMoveDirection(moveDirection.normalized) * stats.moveSpeedMax * 10;
            else moveDirection = moveDirection.normalized * stats.moveSpeedMax * 10;

            moveDirection = Vector3.SmoothDamp(previousVelocityInput, moveDirection, ref velocity, 1 / stats.moveSpeedAcceleration);
            movementCtrl.rb.AddForce(moveDirection, ForceMode.Acceleration);
            previousVelocityInput = moveDirection;

            if (moveDirection.magnitude > 0.1f) movementCtrl.rb.MoveRotation(Quaternion.LookRotation(moveDirection));
        }
        else
        {
            Vector3 movementInAir = moveDirection.normalized * stats.moveSpeedMax * stats.onAirSpeedMultiplier * 10;
            if (movementCtrl.rb.velocity.y < 0)
            {
                movementCtrl.rb.AddForce((Vector3.up * Physics.gravity.y * 2.0f * movementCtrl.timeSinceTouchedGround * 3 / 1.5f) * (movementCtrl.rb.mass / 50), ForceMode.Acceleration);
            }
            movementInAir = Vector3.SmoothDamp(previousVelocityInput, movementInAir, ref velocity, 1 / stats.moveSpeedAcceleration);
            movementCtrl.rb.AddForce(movementInAir, ForceMode.Acceleration);
            previousVelocityInput = movementInAir;
            if (movementInAir.magnitude > 0.1f)
            {
                movementCtrl.rb.MoveRotation(Quaternion.LookRotation(movementInAir));
            }
        }
    }
    //-----Player Jump-----
    public override void Jump()
    {
        movementCtrl.readyToJump = false;
        movementCtrl.rb.velocity = new Vector3(movementCtrl.rb.velocity.x, 0, movementCtrl.rb.velocity.z);
        movementCtrl.rb.AddForce(Vector3.up * stats.jumpForce, ForceMode.VelocityChange);
        ArmadilloPlayerController.Instance.audioControl.onBallJump.Play();
    }
    private void SpeedControl()
    {
        if (new Vector2(movementCtrl.rb.velocity.x, movementCtrl.rb.velocity.z).magnitude > 0.5f)
        {
            if (!isPlayerRollingAudio)
            {
                isPlayerRollingAudio = true;
                ArmadilloPlayerController.Instance.audioControl.onBallRoll.Play();
            }
        }
        else if (isPlayerRollingAudio)
        {
            isPlayerRollingAudio = false;
            ArmadilloPlayerController.Instance.audioControl.onBallRoll.Stop();
        }
    }

    private Coroutine shieldCheck_Ref;
    private IEnumerator ShieldCheck_Coroutine()
    {
        while (true)
        {
            if (ArmadilloPlayerController.Instance.hpControl.currentShield <= 0)
            {
                ArmadilloPlayerController.Instance.ChangeToDefaultForm();
                shieldCheck_Ref = null;
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
