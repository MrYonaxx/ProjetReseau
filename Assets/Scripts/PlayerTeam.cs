using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTeam : MonoBehaviour
{
    public List<Character> players = new List<Character>();

    public delegate void ActionCharacter(Character c, int id);
    public event ActionCharacter OnPlayerAdded;

    public static PlayerTeam Instance;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void AddToTeam(Character c)
    {
        players.Add(c);
        OnPlayerAdded.Invoke(c, players.Count);
    }

}
