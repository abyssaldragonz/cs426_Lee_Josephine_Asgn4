using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private BoxController[] boxes; // Assign all 16 boxes here in Inspector
    
    private const int TOTAL_THIEVES = 5;
    
    void Awake()
    {
        Instance = this;
        Debug.Log(" GameManager Awake called");
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log(" GameManager OnNetworkSpawn called");
        
        if (IsServer)
        {
            Debug.Log(" GameManager is on Server, initializing game...");
            InitializeGame();
        }
        else
        {
            Debug.Log(" GameManager is on Client (not initializing)");
        }
    }

    void InitializeGame()
    {
        Debug.Log(" Initializing game - placing thieves...");
        
        if (boxes == null || boxes.Length == 0)
        {
            Debug.LogError(" ERROR: No boxes assigned to GameManager!");
            return;
        }
        
        Debug.Log($"Found {boxes.Length} boxes");
        
        // Randomly place 5 thieves
        List<int> thiefIndices = new List<int>();
        
        while (thiefIndices.Count < TOTAL_THIEVES)
        {
            int randomIndex = Random.Range(0, boxes.Length);
            if (!thiefIndices.Contains(randomIndex))
            {
                thiefIndices.Add(randomIndex);
                boxes[randomIndex].SetThief(true);
                Debug.Log($" Thief placed at box {randomIndex} ({boxes[randomIndex].gameObject.name})");
            }
        }
        
        Debug.Log($" All {TOTAL_THIEVES} thieves placed!");
        
        // Debug: Verify thieves are set
        for (int i = 0; i < boxes.Length; i++)
        {
            if (boxes[i].HasThief())
            {
                Debug.Log($"   Box {i} ({boxes[i].gameObject.name}): HAS THIEF ✓");
            }
        }
    }
}
