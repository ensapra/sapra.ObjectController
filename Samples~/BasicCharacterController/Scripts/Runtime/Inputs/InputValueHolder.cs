using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController
{
    public class InputValueHolder : MonoBehaviour
    {
        [HideInInspector] public InputValues input{
            get{ 
                if(inputSO == null)                
                    inputSO = ScriptableObject.CreateInstance<InputValues>();
                return inputSO;}
            set{
                inputSO = value;
            }}
        [SerializeField] private InputValues inputSO;
        void Awake()
        {
            if(inputSO == null)        
                Debug.Log("SInputValues: Working with a temporal SO, please insert a constant one");
        }
    }
}
