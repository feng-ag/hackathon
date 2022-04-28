using UnityEngine;


public class LapObject : TargetObject
{

    public bool finishLap;

    [HideInInspector]
    public bool lapOverNextPass;

    void Start() {
        Register();
    }
    
    void OnEnable()
    {
        lapOverNextPass = false;
    }

    private void OnTriggerEnter(Collider other)
    {    

        if ((layerMask.value & 1 << other.gameObject.layer) > 0)
        {
            Objective.OnUnregisterPickup?.Invoke(this);
        }
    }
}
