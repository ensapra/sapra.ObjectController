using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using sapra.ObjectController;

namespace sapra.InputHandler
{
    [RequireComponent(typeof(InputContainer))]
    public class CInput : MonoBehaviour
    {
        private InputController input;
        private InputValues values;
        private CEntity controller;

        public Transform cam;

        private float UpDown;
        void Awake()
        {
            input = new InputController();
            values = GetComponent<InputContainer>().input;   
            controller = GetComponent<CEntity>();
            InputSystem.settings.SetInternalFeatureFlag("DISABLE_SHORTCUT_SUPPORT", true);
        }
        void Start()
        {
            if(cam != null)
            {
                //Sets the camera to be the reference for the player in case there's an controller
                PDirectionManager directionManager = null;
                PassiveModule passiveModule = controller.RequestModule<PassiveModule>();
                if(controller != null)
                    directionManager = passiveModule.RequestRoutine<PDirectionManager>(false);
                if(directionManager != null)
                        directionManager.setReference(cam);
            }
        }
        void Update()
        {
            values.UpdateLerpedValues();
            values._upDown += UpDown;
        }
        void OnEnable()
        {
            if(input == null)
                ReloadInputs(); 
            input.Player.LeftClick.started += LeftC;
            input.Player.LeftClick.canceled += LeftC;
            
            input.Player.RightClick.started += RightC;
            input.Player.RightClick.canceled += RightC;  

            input.Player.Crouch.started += Crouch;
            input.Player.Crouch.canceled += Crouch;

            input.Player.Run.started += Run;
            input.Player.Run.canceled += Run;

            input.Player.Walk.started += Walk;

            input.Player.Jump.started += Jump;
                    
            input.Player.SkillMenu.started += SkillMenu;

            input.Player.Ground.started += Move;
            input.Player.Ground.performed += Move;
            input.Player.Ground.canceled += Move;
            
            input.Player.Interact.started += SGrabItem;
            input.Player.Interact.performed += PGrabItem;
            input.Player.Interact.canceled += CGrabItem;

            input.Player.SwimUpDown.started += SwimUp;
            input.Player.SwimUpDown.performed += SwimUp;
            input.Player.SwimUpDown.canceled += SwimUp;
            
            input.Player.Look.started += GetDelta; 
            input.Player.Look.performed += GetDelta; 
            input.Player.Look.canceled += GetDelta; 

            input.Player.ScrollTroughItems.started += Scroll; 
            input.Player.ScrollTroughItems.performed += Scroll; 
            input.Player.ScrollTroughItems.canceled += Scroll; 
            input.Player.Enable();

        }
        void OnDisable()
        {
            input.Player.LeftClick.started -= LeftC;
            input.Player.LeftClick.canceled -= LeftC;     
            
            input.Player.RightClick.started -= RightC;
            input.Player.RightClick.canceled -= RightC;   

            input.Player.Crouch.started -= Crouch;
            input.Player.Crouch.canceled -= Crouch;

            input.Player.Run.started -= Run;
            input.Player.Run.canceled -= Run;

            input.Player.Walk.started -= Walk;

            input.Player.Jump.started -= Jump;
            
            input.Player.SkillMenu.started -= SkillMenu;

            input.Player.Ground.started -= Move;
            input.Player.Ground.performed -= Move;
            input.Player.Ground.canceled -= Move;
            
            input.Player.Interact.started -= SGrabItem;
            input.Player.Interact.performed -= PGrabItem;
            input.Player.Interact.canceled -= CGrabItem;

            input.Player.SwimUpDown.started -= SwimUp;
            input.Player.SwimUpDown.performed -= SwimUp;
            input.Player.SwimUpDown.canceled -= SwimUp;

            input.Player.Look.started -= GetDelta; 
            input.Player.Look.performed -= GetDelta; 
            input.Player.Look.canceled -= GetDelta; 
            
            input.Player.ScrollTroughItems.started -= Scroll; 
            input.Player.ScrollTroughItems.performed -= Scroll; 
            input.Player.ScrollTroughItems.canceled -= Scroll; 
            input.Player.Disable();
        }
        void Move(InputAction.CallbackContext context)
        {values._inputVectorRaw = context.ReadValue<Vector2>();}

        void Crouch(InputAction.CallbackContext context)
        {values._wantCrouch = context.ReadValue<float>() > 0.9f; }

        void LeftC(InputAction.CallbackContext context)
        {values._wantLeftClick = context.ReadValue<float>() > 0.9f; }

        void RightC(InputAction.CallbackContext context)
        {values._wantRightClick = context.ReadValue<float>() > 0.9f; 
        if(values._wantRightClick)
            values._triggerRightClick = true;}

        void SkillMenu(InputAction.CallbackContext context)
        {values._wantInventory = !values._wantInventory;    }

        void Run(InputAction.CallbackContext context)
        {values._wantRun = context.ReadValue<float>() > 0.9f;    }

        void SGrabItem(InputAction.CallbackContext context)
        {values._wantInteractClick = true; }    

        void PGrabItem(InputAction.CallbackContext context)
        {values._wantInteractPress = true; }    
        
        void CGrabItem(InputAction.CallbackContext context)
        {   
            values._wantInteractPress = false;       
        }

        void Walk(InputAction.CallbackContext context)
        {values._wantWalk = !values._wantWalk;    }

        void SwimUp(InputAction.CallbackContext context)
        {values._upDownRaw = context.ReadValue<float>();}

        void Jump(InputAction.CallbackContext context)
        {values._wantToJump = true; }

        void Scroll(InputAction.CallbackContext context)
        {values._scrollValue = context.ReadValue<float>();}
        
        void GetDelta(InputAction.CallbackContext context)
        {values._mouseInput = context.ReadValue<Vector2>()/10;    }

        private void ReloadInputs()
        {
            this.input = new InputController();
        }
    }
}