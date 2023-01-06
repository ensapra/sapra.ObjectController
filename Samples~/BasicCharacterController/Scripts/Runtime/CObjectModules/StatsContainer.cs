using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StatsContainer : MonoBehaviour
{
    public bool debug;
    [Header("Player reachings")]
    [Tooltip("Character height from feet to head")]
    public float CharacterHeight = 2;
    [Tooltip("Character basic radious of collider")]
    public float CharacterRadius = 0.5f;

    [Tooltip("Character maximum walkable angle")]
    public float WalkableAngle = 40;
    [Tooltip("Offset from object center to the foot, [DEBUG] Black line, should be on the foot")]
    [SerializeField] private Vector3 footOffsetLocal;
    public Vector3 FootOffset {get{ return this.transform.TransformDirection(footOffsetLocal);}}
    private List<DynamicModifier> StatModifiers = new List<DynamicModifier>();

    [HideInInspector] public float currentRadious;
    [HideInInspector] public float currentHeight;

    public float DynamicMovingSpeed{get; private set;} = 0;
    public float MaximumSpeed{get; private set;} = 13;
    public float MinimumSpeed{get; private set;} = 6;
    public float MiddleSpeed{get; private set;} = 10;
    public float MaximumSlideSpeed{get; private set;} = 6;
    public float MinimumSlideSpeed{get; private set;} = 16;
    public float MaximumSwimSpeed{get; private set;} = 13;
    public float MinimumSwimSpeed{get; private set;} = 15;
    public float MaximumJumpForce{get; private set;} = 15;

    private float DynamicSpeedBaseValue;
    public void SetDynamicSpeed(float value){
            var dynamicSpeedWasChaged = value != DynamicSpeedBaseValue;        
            DynamicSpeedBaseValue = value;
            if(dynamicSpeedWasChaged) ProcessDynamicModifiers();
        }

    public void SetMaximumSpeed(float value){MaximumSpeed = value;}
    public void SetMinimumSpeed(float value){MinimumSpeed = value;}
    public void SetMiddleSpeed(float value){MiddleSpeed = value;}
    public void SetMaximumSlideSpeed(float value){MaximumSlideSpeed = value;}
    public void SetMinimumSlideSpeed(float value){MinimumSlideSpeed = value;}
    public void SetMaximumSwimSpeed(float value){MaximumSwimSpeed = value;}
    public void SetMinimumSwimSpeed(float value){MinimumSwimSpeed = value;}
    public void SetMaximumJumpForce(float value){MaximumJumpForce = value;}
    public List<DynamicModifier> GetCurrentModifiers(){return StatModifiers;}
    void Awake()
    {
        currentRadious = CharacterRadius;
        currentHeight = CharacterHeight;
        StatModifiers = new List<DynamicModifier>();
    }

    private void Update()
    {
        if(debug)
        {
            Debug.DrawRay(transform.position + FootOffset, transform.forward, Color.black);
        }
        ProcessDynamicModifiers();
    }
    
    /// <summary>
    /// Adds a limiter to the value
    /// <summary/>
    public void AddDynamicModifier(DynamicModifier limit)
    {
        if(!StatModifiers.Contains(limit))
            StatModifiers.Add(limit);
    }
    /// <summary>
    /// Removes a limiter to the value
    /// <summary/>
    public void RemoveDynamicModifier(DynamicModifier limit)
    {
        if(StatModifiers.Contains(limit))
            StatModifiers.Remove(limit);
    }
    /// <summary>
    /// Processes the limiters on the value
    /// <summary/>
    public void ProcessDynamicModifiers()
    {
        float intialValue = DynamicSpeedBaseValue;
        foreach(DynamicModifier modifier in StatModifiers)
        {
            modifier.ApplyDynamicModifier();
            if(modifier.TagetValue < intialValue)
                intialValue = modifier.TagetValue;
        }
        DynamicMovingSpeed = intialValue;
    }
}
public interface DynamicModifier
{
    public float TagetValue{get;set;}
    public void ApplyDynamicModifier();
}