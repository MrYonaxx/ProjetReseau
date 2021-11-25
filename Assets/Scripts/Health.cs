using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Health : NetworkBehaviour
{
    private NetworkVariable<float> hp = new NetworkVariable<float>(5.0f);

    void OnEnable()
    {
        hp.OnValueChanged += ValueChanged;
    }

    void OnDisable()
    {
        hp.OnValueChanged -= ValueChanged;
    }

    void ValueChanged(float prevF, float newF)
    {
        Debug.Log("myFloat went from " + prevF + " to " + newF);
    }
}
