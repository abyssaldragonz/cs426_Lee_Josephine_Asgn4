using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    
    private string[,] clues = {
                            {"keyboard", "mouse", "scanner", "joystick", "microphone"},
                            {"monitor", "speaker", "printer", "headphones", "vibrations"},
                            {"RAM", "SSD", "ROM", "cache", "HDD"},
                            {"AND", "OR", "NOT", "NAND", "NOR"}
                           };
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
        int randCategory = Random.Range(0, clues.GetLength(0)); // choose random category
        List<(int, int)> usedClues = new List<(int, int)>(); // store used clues
        
        while (thiefIndices.Count < TOTAL_THIEVES)
        {
            int randomIndex = Random.Range(0, boxes.Length);
            if (!thiefIndices.Contains(randomIndex)) // doesn't already exist
            {
                thiefIndices.Add(randomIndex);
                boxes[randomIndex].SetThief(true);
                // Debug.Log($"DETECTED {clues[randCategory, (thiefIndices.Count-1)]}");
                boxes[randomIndex].computerPartName = clues[randCategory, (thiefIndices.Count-1)];
                usedClues.Add((randCategory, (thiefIndices.Count-1)));
                Debug.Log($" Thief placed at box {randomIndex} ({boxes[randomIndex].computerPartName})");
            }
        }
        
        Debug.Log($" All {TOTAL_THIEVES} thieves placed!");
        
        // Debug: Verify thieves are set
        
        for (int i = 0; i < boxes.Length; i++)
        {
            if (boxes[i].HasThief())
            {
                Debug.Log($"   Box {i} ({boxes[i].computerPartName}): HAS THIEF ✓");
            }

            else // not thief so assign other computer parts
            {
                int cat = Random.Range(0, clues.GetLength(0));
                int ind = Random.Range(0, 5);
                while (usedClues.Contains((cat,ind)))
                {
                    cat = Random.Range(0, clues.GetLength(0));
                    ind = Random.Range(0, 5);
                }
                usedClues.Add((cat,ind));
                boxes[i].computerPartName = clues[cat, ind];
                
                Debug.Log($"   Box {i} ({boxes[i].computerPartName}): DOES NOT HAVE THIEF X");
            }
        }
    }
}
