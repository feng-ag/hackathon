using UnityEngine;


public class CrashObject : TargetObject
{
   
    public ParticleSystem CollectVFX;

    public Vector3 centerOfMass;

    public float forceUpOnCollide;

    Rigidbody m_rigid;

    void Start()
    {
        m_rigid = GetComponent<Rigidbody>();
        m_rigid.centerOfMass = centerOfMass;
        Register();
    }

    void OnCollect(Collider other)
    {
        active = false;
        if (CollectVFX)
            CollectVFX.Play();
               
        if (m_rigid) m_rigid.AddForce(forceUpOnCollide*Vector3.up, ForceMode.Impulse);
        
        Objective.OnUnregisterPickup(this);

        TimeManager.OnAdjustTime(TimeGained);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active) return;
        
        if ((layerMask.value & 1 << other.gameObject.layer) > 0)
            OnCollect(other);
    }
    
}
