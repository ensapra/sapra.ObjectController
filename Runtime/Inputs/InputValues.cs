using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "InputValues", menuName = "DataObjects/InputValues")]
public class InputValues : ScriptableObject
{
    [Tooltip("Mouse sensitivity")]
    public Vector2 mouseSens = new Vector2(2,2);

    [Header("Lerped Inputs")]
    [Tooltip("Input in local coordinates set by controls lerped")]
    public Vector2 _inputVector = Vector2.zero;
    [Tooltip("Input in local coordinates set by controls in raw format")]
    public Vector2 _inputVectorRaw = Vector2.zero;
    [Tooltip("Input by the mouse")]
    public Vector2 _mouseInput = Vector2.zero;

    [Header("Control States")]
    public bool _wantRun;
    public bool _wantCrouch;
    public bool _wantWalk;
    public bool _wantToJump;
    public bool _wantInventory;
    public bool _wantSkill;
    public bool _wantInteractPress;
    public bool _wantInteractClick;
    public bool _wantLeftClick;
    public bool _wantRightClick;
    public bool _triggerRightClick;
    
    [Tooltip("Mouse input multiplied by sensitivity")]
    public Vector2 mouseSensed{
        get{
            return mouseSens*_mouseInput;
        }
    }
    /////
    //Need to be redefined
    /////
    [Tooltip("Input vertical in local coordinates raw")]
    public float _upDownRaw = 0;
    [Tooltip("Input vertical in local coordinates lerped")]
    public float _upDown = 0;
    [Tooltip("Extra vertical movement")]
    public float _extraCameraInducedVertical;
    public float _scrollValue = 0;

    public void UpdateLerpedValues()
    {
        _inputVector = Vector2.Lerp(_inputVector, _inputVectorRaw, Time.deltaTime*10);    
        _upDown = Mathf.Lerp(_upDown, _upDownRaw, Time.deltaTime*10);
        if(_inputVector.magnitude < 0.01f)
            _inputVector = Vector2.zero;
            
        if(Mathf.Abs(_upDown) < 0.01f)
            _upDown = 0;
    }
    public void SetInputFromDirection(Vector3 direction, Transform transform)
    {
        Vector3 normalDirection = transform.InverseTransformDirection(direction).normalized;
        Vector2 inputRaw = new Vector2(normalDirection.x, normalDirection.z);
        _inputVectorRaw = inputRaw;
        _upDownRaw = normalDirection.y;
    }
/*     public void Reset()
    {
        _inputVector = Vector2.zero;
        _mouseInput = Vector2.zero;
        _upDownRaw = 0;
        _upDown = 0;    
        _wantRun = false;
        _wantCrouch = false;
        _wantWalk = false;
        _wantToJump = false;
        _wantInventory = false;
        _wantSkill = false;
        _wantInteractPress = false;
        _wantInteractClick = false;
        _wantLeftClick = false;
    } */
}