using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class AbstractCObject : MonoBehaviour
    {
        [Tooltip("Enables Continuous check of new components")]
        public bool continuosCheck = true;
        public static float TimeScale = 1f;   
        [HideInInspector] public Rigidbody rb;

        void Start() {
            InitializeObject(true);
        }
        
        public abstract void InitializeObject(bool forcedRestart);
        public T RequestComponent<T>(bool required) where T : Component
        {
            T requested = GetComponent<T>();
            if(requested == null && required)        
                requested = this.gameObject.AddComponent<T>();
            return requested;
        }
        public abstract void GetAllComponents();
    }
}

