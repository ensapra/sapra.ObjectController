using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sapra.ObjectController;

[RequireComponent(typeof(Rigidbody))]
public class CMotor : MonoBehaviour
{
    public Rigidbody rb;
    [DisableIf("True")] public Vector3 gravityDirection;
    [DisableIf("True")] public float gravityMultiplier;

    public Vector3 velocity{get; private set;}
    public Vector3 newVelocity{get{return _newVelocity;}}
    private Vector3 _newVelocity;

    public Vector3 angularVelocity{get; private set;}
    public Vector3 newAngularVelocity{get{return _newAngularVelocity;}}
    private Vector3 _newAngularVelocity;
    [DisableIf("True")] public List<Routine> DisableGravityCount = new List<Routine>();
    public bool Gravity => DisableGravityCount.Count <= 0;
    [SerializeField] [DisableIf("True")] private bool gravity;
    void Start()
    {
        DisableGravityCount.Clear();
        rb = GetComponent<Rigidbody>();
        gravityDirection = Physics.gravity.normalized;
        gravityMultiplier = Physics.gravity.magnitude;
        rb.useGravity = false;
    }

    public void UpdateMotor()
    {
        AddGravity();
        rb.velocity = _newVelocity;
        rb.angularVelocity = _newAngularVelocity;

        velocity = _newVelocity;
        angularVelocity = _newAngularVelocity;
    }
    public void AfterSimulation()
    {
        velocity = _newVelocity = rb.velocity;
        angularVelocity = _newAngularVelocity = rb.angularVelocity;
    }

    private void AddGravity()
    {
        if(Gravity)
            _newVelocity += (gravityDirection*gravityMultiplier)*Time.deltaTime;
        gravity = Gravity;
    }

    public void SetVelocity(Vector3 velocity)
    {
        this._newVelocity = velocity;
    }
    public void AddVelocity(Vector3 velocity)
    {
        this._newVelocity += velocity;
    }

    public void SetAngularVelocity(Vector3 angularVelocity)
    {
        this._newAngularVelocity = angularVelocity;
    }
    public void AddAngularVelocity(Vector3 angularVelocity)
    {
        this.angularVelocity += angularVelocity;
    }

    //Should be changed accordingly
    public void AddForceAtPosition(Vector3 force, Vector3 position, ForceMode mode){
        rb.AddForceAtPosition(force, position, mode);
    }
    public void DisableGravity(Routine routine)
    {
        if(!DisableGravityCount.Contains(routine))
            DisableGravityCount.Add(routine);
    }
    public void EnableGravity(Routine routine)
    {
        if(DisableGravityCount.Contains(routine))
            DisableGravityCount.Remove(routine);
    }
}
