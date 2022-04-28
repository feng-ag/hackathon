﻿using UnityEngine;

public abstract class TargetObject : MonoBehaviour
{
  
    public GameModeSingle gameMode;

    public float TimeGained;

    public LayerMask layerMask;

    public Transform CollectVFXSpawnPoint;

    public AudioClip CollectSound;

    [HideInInspector]
    public bool active;

    void OnEnable()
    {
        active = true;
    }
    
    protected void Register()
    {
        Objective.OnRegisterPickup?.Invoke(this);
    }
}
