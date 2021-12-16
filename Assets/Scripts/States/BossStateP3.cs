using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class BossStateP3 : BossState
{
    [Space]
    [Space]
    [SerializeField]
    float timeStartCamera;
    [SerializeField]
    float timeCamera;

    [SerializeField]
    AreaOfLightRampant[] lightRampants;

    [Space]
    [SerializeField]
    Camera mainCam = null;
    [SerializeField]
    GameObject cinematicCam = null;

    [Space]
    [SerializeField]
    BossState[] endStates = null;

    float t = 0f;
    bool startCameraZoom = false;

    public override void StartState(Boss boss)
    {
        t = timeStartCamera;

    }

    public override void UpdateState(Boss boss)
    {
        t -= Time.deltaTime;
        if(t <= 0 && !startCameraZoom)
        {
            boss.transform.eulerAngles = new Vector3(0, boss.transform.eulerAngles.y, 0);
            boss.PlayBossAnimationClientRpc(AnimationBoss.Attack3);
            StartCoroutine(LightRampantCoroutine());

            // Cinematic
            boss.isArmor = true;
            NewPhaseClientRPC();

            startCameraZoom = true;
            t = timeCamera;
        }
        else if(t <= 0)
        {
            boss.SetState(endStates[Random.Range(0, endStates.Length)]);

            boss.isArmor = false;
            NewPhaseClientRPC();
        }

    }

    private IEnumerator LightRampantCoroutine()
    {
        for (int i = 0; i < lightRampants.Length; i++)
        {
            lightRampants[i].SetActive(true);
            yield return new WaitForSeconds(0.4f);
        }
    }

    [ClientRpc]
    private void NewPhaseClientRPC()
    {
        mainCam.enabled = !mainCam.enabled;
        cinematicCam.gameObject.SetActive(!cinematicCam.activeInHierarchy);
    }

}
