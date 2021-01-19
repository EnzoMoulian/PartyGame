using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    private Dictionary<string, bool> currentPlayerInputs = new Dictionary<string, bool>();

    void Start()
    {
        currentPlayerInputs.Add("A", false);
        currentPlayerInputs.Add("B", false);
        currentPlayerInputs.Add("X", false);
        currentPlayerInputs.Add("Y", false);
        currentPlayerInputs.Add("Special", false);

        GetComponent<PlayerInfo>().allPlayerInputs = currentPlayerInputs;
    }

    void Update()
    {
        Dictionary<string, bool> temp = currentPlayerInputs;

        foreach (KeyValuePair<string, bool> input in currentPlayerInputs.ToList())
        {
            temp[input.Key] = Input.GetButtonDown(input.Key + GetComponent<PlayerInfo>().n_playerNumber.ToString());
        }

        currentPlayerInputs = temp;
        GetComponent<PlayerInfo>().allPlayerInputs = currentPlayerInputs;
    }
}
