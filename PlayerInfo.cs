using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public int n_playerNumber;
    public float playersCurrentHealth;
    public bool canSmashNextBlock;
    public bool hasWonGame;
    public bool hasLostGame;
    public bool special2Triggered;
    public Dictionary<string, bool> allPlayerInputs = new Dictionary<string, bool>();
}
