using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public class TrackWheelRow
{
    [SerializeField] private WheelCollider[] colliders;
    [SerializeField] private Transform[] meshes;

    public float minRpm;
    public void SetTorque(float motorTorque)
    {
       for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].motorTorque = motorTorque;
        }
    }

    public void Break(float breakTorque)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].brakeTorque = breakTorque;
        }
    }

    public void Reset()
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].brakeTorque = 0;
            colliders[i].motorTorque = 0;
        }
    }

    public void SetSideWayStifness(float stiffness)
    {
        WheelFrictionCurve wheelFrictionCurve = new WheelFrictionCurve();

        for(int i = 0; i < colliders.Length; i++)
        {
            wheelFrictionCurve = colliders[i].sidewaysFriction;
            wheelFrictionCurve.stiffness = stiffness;

            colliders[i].sidewaysFriction = wheelFrictionCurve;
        }
    }

    public void UpdateMeshTransform()
    {
        List<float> allRpm = new List<float>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if(colliders[i].isGrounded == true)
            {
                allRpm.Add(colliders[i].rpm);
            }
        }

        if(allRpm.Count > 0)
        {
            minRpm = Mathf.Abs(allRpm[0]);
            for (int i = 0; i < allRpm.Count; i++)
            {
                if(Mathf.Abs(allRpm[i]) < minRpm)
                {
                    minRpm = Mathf.Abs (allRpm[i]);
                }
            }

            minRpm = minRpm * Mathf.Sign(allRpm[0]);
        }

        float angle = minRpm * 360.0f / 60.0f * Time.fixedDeltaTime;


        for (int i = 0; i < meshes.Length; i++)
        {
            Vector3 position;
            Quaternion rotation;

            colliders[i].GetWorldPose(out position, out rotation);
            meshes[i].position = position;
            meshes[i].Rotate(angle,0,0);
        }
    }

    public void UpdateMeshRotationByRpm(float rpm)
    {
        float angle = rpm * 360.0f / 60.0f * Time.fixedDeltaTime;

        for (int i =0; i < meshes.Length; i++)
        {
            Vector3 position;
            Quaternion rotation;

            colliders[i].GetWorldPose(out position, out rotation);

            meshes[i].position = position;
            meshes[i].Rotate(angle, 0, 0);
        }
    }
}
[RequireComponent(typeof(Rigidbody))]
public class TrackTank : Vehicle
{
    public override float LinearVelocity => rigidBody.velocity.magnitude;

    [SerializeField] private Transform centerOfMass;

    [SerializeField] private TrackWheelRow leftWheelRow;
    [SerializeField] private TrackWheelRow rightWheelRow;
    [SerializeField] private GameObject destroyedPrefab;
    [SerializeField] private GameObject visualModel;

    [SerializeField] private ParameterCurve forwardTorqueCurve;    
    [SerializeField] private float maxForwardTorque;
    [SerializeField] private float maxBackwardMotorTorque;
    [SerializeField] private ParameterCurve backwardTorqueCurve;
    [SerializeField] private float breakTorque;
    [SerializeField] private float rollingResistance;

    [SerializeField] private float rotateTorqueInPlace;
    [SerializeField] private float rotateBreakInPlace;
    [Space(2)]
    [SerializeField] private float rotateTorqueInMotion;
    [SerializeField] private float rotateBreakInMotion;

    [SerializeField] private float minSidewayStiffnessInPlace;
    [SerializeField] private float minSidewayStiffnessInMotion;

    private Rigidbody rigidBody;
    [SerializeField]  private float currentMotorTorque;

    public float LeftWheelRpm => leftWheelRow.minRpm;
    public float RightWheelRpm => rightWheelRow.minRpm;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {       
        rigidBody.centerOfMass = centerOfMass.localPosition;
        Destroyed += OnTrackTankDestroyed;
    }

    private void OnDestroy()
    {
        Destroyed -= OnTrackTankDestroyed;
    }

    private void FixedUpdate()
    {
        if(isServer == true)
        {
            UpdateMotorTorque();
            SvUpdateWheelRpm(LeftWheelRpm, RightWheelRpm);
            SvUpdateLinearVelocity(LinearVelocity);
        }

        if(hasAuthority == true)
        {
            UpdateMotorTorque();
            CmdUpdateWheelRpm(LeftWheelRpm, RightWheelRpm);
            CmdUpdateLinearVelocity(LinearVelocity);
        }
    }
    private void OnTrackTankDestroyed(Destructible arg0)
    {
        GameObject ruinedVisualModel = Instantiate(destroyedPrefab);
        ruinedVisualModel.transform.position = visualModel.transform.position;
        ruinedVisualModel.transform.rotation = visualModel.transform.rotation;
    }

    [Command]
    private void CmdUpdateLinearVelocity(float velocity)
    {
        SvUpdateLinearVelocity(velocity);
    }

    [Server]
    private void SvUpdateLinearVelocity(float velocity)
    {
        syncLinearVelocity = velocity;
    }



    [Command]
    private void CmdUpdateWheelRpm(float leftRpm, float rightRpm)
    {
        SvUpdateWheelRpm(leftRpm, rightRpm);
    }

    [Server]
    private void SvUpdateWheelRpm(float leftRpm, float rightRpm)
    {
        RpcUpdateWheelRpm(leftRpm, rightRpm);
    }

    [ClientRpc (includeOwner =false)]
    private void RpcUpdateWheelRpm(float leftRpm, float rightRpm)
    {
        leftWheelRow.minRpm = leftRpm;
        rightWheelRow.minRpm = rightRpm;

        leftWheelRow.UpdateMeshRotationByRpm(leftRpm);
        rightWheelRow.UpdateMeshRotationByRpm(rightRpm);
    }

    private void UpdateMotorTorque()
    {
        float targetMotorTorque = targetInputControl.z > 0 ? maxForwardTorque * Mathf.RoundToInt(targetInputControl.z) : maxBackwardMotorTorque * Mathf.RoundToInt(targetInputControl.z);
        float breakTorque = this.breakTorque * targetInputControl.y;
        float steering = targetInputControl.x;

        if (targetMotorTorque > 0)
        {
            currentMotorTorque = forwardTorqueCurve.MoveTowards(Time.fixedDeltaTime) * targetMotorTorque;
        }

        if (targetMotorTorque < 0)
        {
            currentMotorTorque = backwardTorqueCurve.MoveTowards(Time.fixedDeltaTime) * targetMotorTorque;
        }

        if (targetMotorTorque == 0)
        {
            currentMotorTorque = forwardTorqueCurve.Reset();
            currentMotorTorque = backwardTorqueCurve.Reset();
        }

        leftWheelRow.Break(breakTorque);
        rightWheelRow.Break(breakTorque);

        if (targetMotorTorque == 0 && steering == 0)
        {
            leftWheelRow.Break(rollingResistance);
            rightWheelRow.Break(rollingResistance);
        }
        else
        {
            leftWheelRow.Reset();
            rightWheelRow.Reset();
        }

        if (targetMotorTorque == 0 && steering != 0)
        {
            if (Mathf.Abs(leftWheelRow.minRpm) < 1 || Mathf.Abs(rightWheelRow.minRpm) < 1)
            {
                rightWheelRow.SetTorque(rotateBreakInPlace);
                leftWheelRow.SetTorque(rotateTorqueInPlace);
            }
            else
            {
                if (steering < 0)
                {
                    leftWheelRow.Break(rotateBreakInPlace);
                    rightWheelRow.SetTorque(rotateTorqueInPlace);
                }

                if (steering > 0)
                {
                    leftWheelRow.SetTorque(rotateTorqueInPlace);
                    rightWheelRow.Break(rotateBreakInPlace);
                }
            }
            leftWheelRow.SetSideWayStifness(1.0f + minSidewayStiffnessInPlace - Mathf.Abs(steering));
            rightWheelRow.SetSideWayStifness(1.0f + minSidewayStiffnessInPlace - Mathf.Abs(steering));
        }

        if (targetMotorTorque != 0)
        {
            if (steering == 0)
            {
                if (LinearVelocity < maxLinearVelocity)
                {
                    leftWheelRow.SetTorque(currentMotorTorque);
                    rightWheelRow.SetTorque(currentMotorTorque);
                }
            }

            if (steering != 0 && (Mathf.Abs(leftWheelRow.minRpm) < 1 || Mathf.Abs(rightWheelRow.minRpm) < 1))
            {
                rightWheelRow.SetTorque(rotateTorqueInMotion * Mathf.Sign(currentMotorTorque));
                leftWheelRow.SetTorque(rotateTorqueInMotion * Mathf.Sign(currentMotorTorque));
            }
            else
            {
                if (steering < 0)
                {
                    leftWheelRow.Break(rotateBreakInMotion);
                    rightWheelRow.SetTorque(rotateTorqueInMotion * Mathf.Sign(currentMotorTorque));
                }

                if (steering > 0)
                {
                    leftWheelRow.SetTorque(rotateTorqueInMotion * Mathf.Sign(currentMotorTorque));
                    rightWheelRow.Break(rotateBreakInMotion);
                }
            }

            leftWheelRow.SetSideWayStifness(1.0f + minSidewayStiffnessInMotion - Mathf.Abs(steering));
            rightWheelRow.SetSideWayStifness(1.0f + minSidewayStiffnessInMotion - Mathf.Abs(steering));
        }
        leftWheelRow.UpdateMeshTransform();
        rightWheelRow.UpdateMeshTransform();
    }


}
