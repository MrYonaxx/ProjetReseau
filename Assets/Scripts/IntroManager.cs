using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Playables;

public class IntroManager : MonoBehaviour
{
    [SerializeField]
    Camera cameraIntro = null;

    [SerializeField]
    PlayableDirector director = null;

    [SerializeField]
    Animator fade = null;

    [SerializeField]
    Transform[] positions;

    [SerializeField]
    int nbPlayers = 3;
    [SerializeField]
    GameObject waitZone;

    Character character = null;

    // Start is called before the first frame update
    void Start()
    {
        PlayerTeam.Instance.OnPlayerAdded += StartIntro;
    }

    void StartIntro(Character c, int id)
    {

        c.transform.position = positions[id-1].position;

        if (id >= nbPlayers)
            waitZone.gameObject.SetActive(false);

        if ((ulong)(id-1) == NetworkManager.Singleton.LocalClientId)
        {
            character = c;
            cameraIntro.enabled = false;
            director.gameObject.SetActive(true);
            StartCoroutine(IntroCoroutine());
        }
    }

    IEnumerator IntroCoroutine()
    {
        float t = 0f;
        while(t < 10)
        {
            t += Time.deltaTime;
            if (Input.GetMouseButtonDown(0))
                t = 10f;
            yield return null;
        }
        EndIntro();
    }

    void EndIntro()
    {
        StartCoroutine(FadeCoroutine());
    }

    IEnumerator FadeCoroutine()
    {
        fade.SetTrigger("Feedback");
        yield return new WaitForSeconds(1f);
        cameraIntro.enabled = true;
        character.Active();
        director.gameObject.SetActive(false);
    }
}
