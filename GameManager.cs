using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // serializable miscellaneous
    public int n_nbPlayer = 1;
    public GameObject playerPrefab;
    public int n_maxBlockDisplayed = 10;
    public int n_maxBlocksInGame = 50;
    public float f_initialHealth = 5;

    // players
    public Dictionary<int, GameObject> allPlayersInstances = new Dictionary<int, GameObject>();
    public Dictionary<string, List<GameObject>> allInputSmashAllowness = new Dictionary<string, List<GameObject>>();

    // blocks
    public List<GameObject> blocksToSpawn = new List<GameObject>();
    public List<int> f_blocksSpawnRate = new List<int>();
    public GameObject blockSpecial1;
    public GameObject blockSpecial2;
    public Dictionary<int, bool> special2Triggered = new Dictionary<int, bool>();

    // inputs smash allowness
    public List<GameObject> inputASmashAllowness = new List<GameObject>();
    public List<GameObject> inputBSmashAllowness = new List<GameObject>();
    public List<GameObject> inputXSmashAllowness = new List<GameObject>();
    public List<GameObject> inputYSmashAllowness = new List<GameObject>();
    public List<GameObject> inputSpecialSmashAllowness = new List<GameObject>();

    // UI
    public GameObject textGameObject;
    public Dictionary<int, Text> allPlayerCurrentBlocksInGameDisplayed = new Dictionary<int, Text>();

    // singleton instance
    public static GameManager _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        SetUpInputSmashAllowness();
        SetUpPlayerInstances();
        SetUpBlockInGameDisplayed();
    }

    private void SetUpInputSmashAllowness()
    {
        allInputSmashAllowness.Add("A", inputASmashAllowness);
        allInputSmashAllowness.Add("B", inputBSmashAllowness);
        allInputSmashAllowness.Add("X", inputXSmashAllowness);
        allInputSmashAllowness.Add("Y", inputYSmashAllowness);
        allInputSmashAllowness.Add("Special", inputSpecialSmashAllowness);
    }

    private void SetUpPlayerInstances()
    {
        for (int i = 0; i < n_nbPlayer; i++)
        {
            GameObject newPlayerToInstanciate = Instantiate(playerPrefab);
            newPlayerToInstanciate.transform.SetParent(GameObject.Find("Players").transform);
            newPlayerToInstanciate.transform.position = new Vector3((4 * i) - (n_nbPlayer * 2) + 2, -4, 0);
            newPlayerToInstanciate.GetComponent<PlayerInfo>().n_playerNumber = i;
            newPlayerToInstanciate.GetComponent<PlayerInfo>().playersCurrentHealth = f_initialHealth;
            newPlayerToInstanciate.GetComponent<PlayerInfo>().special2Triggered = false;
            newPlayerToInstanciate.GetComponent<PlayerInfo>().canSmashNextBlock = true;
            newPlayerToInstanciate.GetComponent<PlayerInfo>().hasWonGame = false;
            newPlayerToInstanciate.GetComponent<PlayerInfo>().hasLostGame = false;
            allPlayersInstances.Add(i, newPlayerToInstanciate);
        }
    }

    private void SetUpBlockInGameDisplayed()
    {
        for (int i = 0; i < n_nbPlayer; i++)
        {
            GameObject currentBlocksInGameDisplayed = Instantiate(textGameObject);
            currentBlocksInGameDisplayed.transform.SetParent(GameObject.Find("Canvas").transform);
            currentBlocksInGameDisplayed.GetComponent<RectTransform>().localPosition = new Vector3((107 * i) - (n_nbPlayer * 53.5f) + 53.5f, -150, 0);
            allPlayerCurrentBlocksInGameDisplayed.Add(i, currentBlocksInGameDisplayed.GetComponent<Text>());
        }
    }

    public void SetVictory(int playerNumber)
    {
        allPlayersInstances[playerNumber].GetComponent<PlayerInfo>().hasWonGame = true;
        EnableSmashingBLockForAllPlayers(false);
        DisplayWhoWinsInsteadOfCurrentBlockInGame();
    }

    public void SetPlayerLoose(int playerNumber)
    {
        allPlayersInstances[playerNumber].GetComponent<PlayerInfo>().hasLostGame = true;
        allPlayerCurrentBlocksInGameDisplayed[playerNumber].text = "Loose";
        bool b_allPlayerLoose = true;

        foreach (KeyValuePair<int, GameObject> player in allPlayersInstances)
        {
            if (!player.Value.GetComponent<PlayerInfo>().hasLostGame)
            {
                b_allPlayerLoose = false;
            }
        }

        if(b_allPlayerLoose)
        {
            SetGameOver();
        }
    }

    public void SetGameOver()
    {
        DisplayWhoWinsInsteadOfCurrentBlockInGame();
    }

    public void EnableSmashingBLockForAllPlayers(bool isAllowed)
    {
        foreach(KeyValuePair<int, GameObject> player in allPlayersInstances)
        {
            player.Value.GetComponent<PlayerInfo>().canSmashNextBlock = isAllowed;
        }
    }

    public void DisplayWhoWinsInsteadOfCurrentBlockInGame()
    {
        foreach (KeyValuePair<int, GameObject> player in allPlayersInstances)
        {
            if(player.Value.GetComponent<PlayerInfo>().hasWonGame)
            {
                allPlayerCurrentBlocksInGameDisplayed[player.Value.GetComponent<PlayerInfo>().n_playerNumber].text = "Win !";
            }
            else
            {
                allPlayerCurrentBlocksInGameDisplayed[player.Value.GetComponent<PlayerInfo>().n_playerNumber].text = "Loose";
            }
        }
    }
}
