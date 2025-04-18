using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance {get;private set;}
    private CinemachineImpulseSource cinemachineImpulseSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one ScreenShake Object!" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
        
    }


    public void Shake(float intensity = 1f)
    {
        cinemachineImpulseSource.GenerateImpulse(intensity);
    }
}
    