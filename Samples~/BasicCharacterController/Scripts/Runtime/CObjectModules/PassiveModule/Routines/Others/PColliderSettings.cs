using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sapra.ObjectController
{
    [System.Serializable]
    public class PColliderSettings : AbstractPassive
    {
        public override PassivePriority whenDo => PassivePriority.AfterActive;
        private PWalkableDetection _pWalkableDetection;
        private SDimensions _sDim;
        private CapsuleCollider colliderCom;
        private List<PhysicMaterial> materials = new List<PhysicMaterial>();
        private float currentHeight{
            get{
                if(temporalHeight != -1)
                {   
                    return temporalHeight;
                }
                else
                    return _sDim.CharacterHeight/2f;
            }
            set{
                temporalHeight = value;
            }
        }
        private float currentRadious{
            get{
                if(temporalRadious != -1)            
                    return temporalRadious;
                else
                    return _sDim.characterRadious;
            }
            set{
                temporalRadious = value;
            }
        }
        private float currentFactor{
            get{
                if(temporalFactor != -1)
                {
                    return temporalFactor;
                }
                else
                    return 2;
            }
            set{
                temporalFactor = value;
            }
        }
        private float temporalHeight = -1;
        private float temporalRadious = -1;
        private float temporalFactor = -1;
        // Start is called before the first frame update
        protected override void AwakeRoutine(AbstractCObject controller)        {
            _pWalkableDetection = controller.RequestModule<PassiveModule>().RequestRoutine<PWalkableDetection>(true);
            _sDim = controller.RequestModule<StatModule>().RequestRoutine<SDimensions>(true);
            colliderCom = controller.RequestComponent<CapsuleCollider>(true);

            if(materials.Count < 2)
            {
                materials.Clear();
                GenerateMaterials(0.6f,0.6f,0, PhysicMaterialCombine.Average);
                GenerateMaterials(0,0,0,PhysicMaterialCombine.Multiply);
            }
        }

        private void SetGround()
        {
            colliderCom.material = materials[0];
        }
        private void SetFlying()
        {
            colliderCom.material = materials[1];
        }
        public void SetFactor(float factor)
        {
            currentFactor = factor;
        }
        private void SetCollider(float height, float radious, float factor)
        {
            colliderCom.height = height*factor;
            colliderCom.radius = radious;
            colliderCom.center = new Vector3(0,height*(2-factor/2),0);
            _sDim.currentHeight = height;
            _sDim.currentRadious = radious;
            ResetCurrents();
        }   

        void GenerateMaterials(float dynamicFriction, float staticFriction, float bounciness, PhysicMaterialCombine combine)
        {
            PhysicMaterial material1 = new PhysicMaterial();
            material1.dynamicFriction = dynamicFriction;
            material1.staticFriction = staticFriction;
            material1.bounciness = bounciness;
            material1.frictionCombine = combine;
            material1.bounceCombine = combine;
            materials.Add(material1);
        }

        public override void DoPassive(Vector3 position, InputValues input)
        {
            if(_pWalkableDetection != null && _pWalkableDetection.Walkable)        
                SetGround();        
            else
                SetFlying();   

            SetCollider(currentHeight,currentRadious, currentFactor);
        }
        private void ResetCurrents()
        {
            currentRadious = -1;
            currentHeight = -1;
            currentFactor = -1;
        }
        public void ChangeSettings(float height, float radious)
        {
            currentRadious = radious;
            currentHeight = height;
        }
    }
}