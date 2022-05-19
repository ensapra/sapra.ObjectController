using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra
{
    public class AbstractCObject : MonoBehaviour
    {
        [Tooltip("Enables Continuous check of new components")]
        public bool continuosCheck = true;
        public static float TimeScale = 1f;
        
        [HideInInspector] public Rigidbody rb;

    }
}
