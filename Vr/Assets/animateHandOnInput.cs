using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class animateHandOnInput : MonoBehaviour
{
    [SerializeField] private InputActionProperty triggerValue;
    [SerializeField] private InputActionProperty gripValue;
    
    [SerializeField] private Animator handAnimator;

    private static readonly int triggerHash = Animator.StringToHash("Trigger");
    private static readonly int gripHash = Animator.StringToHash("Grip");

    private void Awake()
    {
        if(handAnimator == null)
        { 
            handAnimator = GetComponent<Animator>();
        }

    }

    private void OnEnable()
    {
        if(triggerValue != null && triggerValue.action != null)
            triggerValue.action.Enable();
            //handAnimator=GetComponent<Animator>();

        if (gripValue != null && gripValue.action != null)
            gripValue.action.Enable();
    }

    private void OnDisable()
    {
        if (triggerValue != null && triggerValue.action != null)
            triggerValue.action.Disable();

        if (gripValue != null && gripValue.action != null)
            gripValue.action.Disable();
    }

    private void Update()
    {
        if (handAnimator == null)
            return;

        float trigger = 0f;
        float grip = 0f;

        if (triggerValue != null&&  triggerValue.action != null)
            trigger = triggerValue.action.ReadValue<float>();

        if (gripValue != null && gripValue.action != null)
            grip = gripValue.action.ReadValue<float>();


        handAnimator.SetFloat(triggerHash, trigger);
        handAnimator.SetFloat(gripHash, grip);
    }

}
