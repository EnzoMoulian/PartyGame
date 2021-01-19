using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlocksBehavior : MonoBehaviour
{
    private Dictionary<int, GameObject> listOfBlocks = new Dictionary<int, GameObject>();
    private List<GameObject> spawnProbabilities = new List<GameObject>();

    private int n_currentBlocksInGame;
    private bool b_isBlockSmashed;

    void Start()
    {
        n_currentBlocksInGame = GameManager._instance.n_maxBlocksInGame;
        GameManager._instance.allPlayerCurrentBlocksInGameDisplayed[GetComponentInParent<PlayerInfo>().n_playerNumber].text = n_currentBlocksInGame.ToString();

        for (int i = 0; i < GameManager._instance.blocksToSpawn.Count; i++)
        {
            for (int j = 0; j < GameManager._instance.f_blocksSpawnRate[i]; j++)
            {
                if (GameManager._instance.blocksToSpawn[i] == GameManager._instance.blockSpecial2 && GameManager._instance.n_nbPlayer <= 0)
                {
                    int n_randomBlockIndex = UnityEngine.Random.Range(0, 3);
                    spawnProbabilities.Add(GameManager._instance.blocksToSpawn[n_randomBlockIndex]);
                }
                else
                {
                    spawnProbabilities.Add(GameManager._instance.blocksToSpawn[i]);
                }
            }
        }

        for (int i = 0; i < GameManager._instance.n_maxBlockDisplayed; i++)
        {
            GenerateRandomBlock(i, false);
        }
    }

    void Update()
    {
        if (GetComponentInParent<PlayerInfo>().canSmashNextBlock && n_currentBlocksInGame > 0)
        {
            GameObject firstBlock = listOfBlocks[0];
            CheckAllInputsAndApplyEffects(firstBlock);
            CheckSpecial2AndApplyEffects();
            UpdateWinAndLoose();
        }
    }

    private void CheckAllInputsAndApplyEffects(GameObject firstBlock)
    {
        foreach (KeyValuePair<string, bool> input in GetComponentInParent<PlayerInfo>().allPlayerInputs)
        {
            if (input.Value)
            {
                InputAction(firstBlock, input);
            }
        }
    }

    private void InputAction(GameObject firstBlock, KeyValuePair<string, bool> input)
    {
        b_isBlockSmashed = false;

        foreach (GameObject block in GameManager._instance.allInputSmashAllowness[input.Key])
        {
            CheckSmashSpecial1BlockAndApplyEffects(firstBlock, block, input);
            CheckSmashSpecial2BlockAndApplyEffects(firstBlock, block, input);
            CheckSmashNormalBlockAndApplyEffects(firstBlock, block);
        }

        UpdateHealth();
    }

    private void CheckSmashSpecial1BlockAndApplyEffects(GameObject firstBlock, GameObject blockCheckerAllowness, KeyValuePair<string, bool> input)
    {
        if ((!GetComponentInParent<PlayerInfo>().special2Triggered && !b_isBlockSmashed) && firstBlock.name == blockCheckerAllowness.name + "(Clone)")
        {
            if (firstBlock.name == GameManager._instance.blockSpecial1.name + "(Clone)")
            {
                if (input.Key == "Special")
                {
                    RemoveBlockAndCreateNewOne(firstBlock);
                    Special1();
                }
                else
                {
                    ReplaceBlockWithRandomOne(0, true);
                }

                b_isBlockSmashed = true;
            }
        }
    }

    private void CheckSmashSpecial2BlockAndApplyEffects(GameObject firstBlock, GameObject blockCheckerAllowness, KeyValuePair<string, bool> input)
    {
        if ((!GetComponentInParent<PlayerInfo>().special2Triggered && !b_isBlockSmashed) && firstBlock.name == blockCheckerAllowness.name + "(Clone)")
        {
            if (firstBlock.name == GameManager._instance.blockSpecial2.name + "(Clone)")
            {
                if (input.Key == "Special")
                {
                    if (GameManager._instance.n_nbPlayer > 1)
                    {
                        RemoveBlockAndCreateNewOne(firstBlock);
                        GameManager._instance.allPlayersInstances[GetRandomOtherPlayerIndex()].GetComponent<PlayerInfo>().special2Triggered = true;
                    }
                    else
                    {
                        RemoveBlockAndCreateNewOne(firstBlock);
                    }
                }
                else
                {
                    ReplaceBlockWithRandomOne(0, true);
                }

                b_isBlockSmashed = true;
            }
        }
    }

    private void CheckSmashNormalBlockAndApplyEffects(GameObject firstBlock, GameObject blockCheckerAllowness)
    {
        if ((!GetComponentInParent<PlayerInfo>().special2Triggered && !b_isBlockSmashed) && firstBlock.name == blockCheckerAllowness.name + "(Clone)")
        {
            RemoveBlockAndCreateNewOne(firstBlock);
            b_isBlockSmashed = true;
        }
    }

    private void UpdateHealth()
    {
        if (!b_isBlockSmashed)
        {
            GetComponentInParent<PlayerInfo>().playersCurrentHealth--;
        }
    }

    private void CheckSpecial2AndApplyEffects()
    {
        if (GetComponentInParent<PlayerInfo>().special2Triggered)
        {
            Special2();
            GetComponentInParent<PlayerInfo>().special2Triggered = false;
        }
    }

    private void UpdateWinAndLoose()
    {
        if (GetComponentInParent<PlayerInfo>().playersCurrentHealth <= 0)
        {
            GetComponentInParent<PlayerInfo>().canSmashNextBlock = false;
            GameManager._instance.SetPlayerLoose(GetComponentInParent<PlayerInfo>().n_playerNumber);
        }

        if (n_currentBlocksInGame <= 0)
        {
            GameManager._instance.SetVictory(GetComponentInParent<PlayerInfo>().n_playerNumber);
        }
    }

    private GameObject PickRandomBlock()
    {
        int n_randomBlockIndex = Random.Range(0, spawnProbabilities.Count);
        GameObject randomBlock = spawnProbabilities[n_randomBlockIndex];
        return randomBlock;
    }

    private GameObject PickRandomNormalBlock()
    {
        int n_maxRange = spawnProbabilities.Count - GameManager._instance.f_blocksSpawnRate[4] - GameManager._instance.f_blocksSpawnRate[5] - GameManager._instance.f_blocksSpawnRate[6];
        int n_randomBlockIndex = Random.Range(0, n_maxRange);
        GameObject randomBlock = spawnProbabilities[n_randomBlockIndex];
        return randomBlock;
    }

    private void GenerateRandomBlock(int indexInList, bool generateNormalBlock)
    {
        GameObject newBlock = Instantiate((generateNormalBlock ? PickRandomNormalBlock() : PickRandomBlock()), new Vector3(transform.position.x, (indexInList <= 0 ? transform.position.y - 1 : (listOfBlocks[indexInList - 1]).transform.position.y + 1.4f), transform.position.z), Quaternion.identity);
        newBlock.transform.parent = transform;
        listOfBlocks.Add((indexInList >= 0 ? indexInList : 0), newBlock);
    }

    private void GenerateBlock(int indexInList, int indexOfBLock)
    {
        GameObject newBlock = Instantiate(GameManager._instance.blocksToSpawn[indexOfBLock], new Vector3(transform.position.x, (indexInList <= 0 ? transform.position.y - 1 : (listOfBlocks[indexInList - 1]).transform.position.y + 1.4f), transform.position.z), Quaternion.identity);
        newBlock.transform.parent = transform;
        listOfBlocks.Add((indexInList >= 0 ? indexInList : 0), newBlock);
    }

    private void RemoveBlockAndCreateNewOne(GameObject firstBlockOfTheList)
    {
        n_currentBlocksInGame--;
        GameManager._instance.allPlayerCurrentBlocksInGameDisplayed[GetComponentInParent<PlayerInfo>().n_playerNumber].text = n_currentBlocksInGame.ToString();
        listOfBlocks.Remove(0);
        Destroy(firstBlockOfTheList);
        Dictionary<int, GameObject> tempList = new Dictionary<int, GameObject>();

        foreach (var entry in listOfBlocks)
        {
            tempList.Add(entry.Key - 1, entry.Value);
        }

        listOfBlocks = tempList;

        if (n_currentBlocksInGame >= GameManager._instance.n_maxBlockDisplayed)
        {
            GenerateRandomBlock(GameManager._instance.n_maxBlockDisplayed - 1, false);
        }
    }

    private void ReplaceBlockWithRandomOne(int indexOfBlock, bool generateNormalBlock)
    {
        GameObject blockToReplace = listOfBlocks[indexOfBlock];
        listOfBlocks.Remove(indexOfBlock);
        Destroy(blockToReplace);
        GenerateRandomBlock(indexOfBlock, generateNormalBlock);
    }

    private void ReplaceBlockWithAnotherOne(int indexOfBlock, int indexOfAnotherBlock)
    {
        GameObject blockToReplace = listOfBlocks[indexOfBlock];
        Destroy(blockToReplace);
        listOfBlocks.Remove(indexOfBlock);
        GenerateBlock(indexOfBlock, indexOfAnotherBlock);
    }

    private int GetRandomOtherPlayerIndex()
    {
        HashSet<int> excludePlayerNumber = new HashSet<int>() { GetComponentInParent<PlayerInfo>().n_playerNumber };
        IEnumerable<int> playersWhoCanBeTargeted = Enumerable.Range(0, GameManager._instance.n_nbPlayer).Where(i => !excludePlayerNumber.Contains(i));
        int n_randomPlayerNumber = Random.Range(0, playersWhoCanBeTargeted.Count() - 1);
        return playersWhoCanBeTargeted.ElementAt(n_randomPlayerNumber);
    }

    private void Special1()
    {
        for (int i = listOfBlocks.Count - 1; i >= 0; i--)
        {
            ReplaceBlockWithAnotherOne(i, 4);
        }
    }

    private void Special2()
    {
        for (int i = listOfBlocks.Count - 1; i >= 0; i--)
        {
            ReplaceBlockWithRandomOne(i, true);
        }
    }
}
