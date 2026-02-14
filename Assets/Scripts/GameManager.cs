using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    // Clues organized by category - all 5 thieves will be from ONE category
    private string[,] clues = {
        {"keyboard", "mouse", "scanner", "joystick", "microphone"},
        {"monitor", "speaker", "printer", "headphones", "vibrations"},
        {"RAM", "SSD", "ROM", "cache", "HDD"},
        {"AND", "OR", "NOT", "NAND", "NOR"}
    };

    [SerializeField] private BoxController[] boxes; // Assign all 16 boxes here in Inspector
    [SerializeField] private TextMeshProUGUI joinCodeText;

    private const int TOTAL_THIEVES = 5;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        Debug.Log("GameManager Awake called");
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log("GameManager OnNetworkSpawn called");

        if (IsServer)
        {
            Debug.Log(" GameManager is on Server, initializing game...");
            DisplayJoinCode();
            InitializeGame();
        }
        else
        {
            Debug.Log("GameManager is on Client (not initializing)");
        }
    }

    private void DisplayJoinCode()
    {
        if (joinCodeText == null)
        {
            Debug.LogError("joinCodeText is not assigned in the Inspector!");
            return;
        }

        // Try singleton first, then search for the component
        if(NetworkManager.Singleton.IsHost)
        {
            string code = NetworkManagerUI.Instance.joinCode;
            if (string.IsNullOrEmpty(code))
            {
                Debug.LogWarning("Join code is empty - it may not be set yet");
                StartCoroutine(WaitForJoinCode());
            }
            else
            {
                joinCodeText.text = $"Join Code: {code}";
                Debug.Log($"Join code displayed: {code}");
            }
        }
    }

    private IEnumerator WaitForJoinCode()
    {
        int attempts = 0;
        while (attempts < 10) // Try up to 10 times
        {
            yield return new WaitForSeconds(0.3f);

            NetworkManagerUI networkUI = NetworkManagerUI.Instance;
            if (networkUI == null)
            {
                networkUI = FindObjectOfType<NetworkManagerUI>();
            }

            if (networkUI == null)
            {
                Debug.LogWarning($"Attempt {attempts + 1}: NetworkUI still not found");
                attempts++;
                continue;
            }

            string code = networkUI.joinCode;
            if (!string.IsNullOrEmpty(code))
            {
                if (joinCodeText != null)
                {
                    joinCodeText.text = $"Join Code: {code}";
                    Debug.Log($"Join code displayed (after {attempts + 1} attempts): {code}");
                }
                yield break;
            }

            Debug.LogWarning($"Attempt {attempts + 1}: Join code is still empty");
            attempts++;
        }

        Debug.LogError("Failed to display join code after 10 attempts");
    }

    void InitializeGame()
    {
        Debug.Log("Initializing game - placing thieves...");

        if (boxes == null || boxes.Length == 0)
        {
            Debug.LogError("ERROR: No boxes assigned to GameManager!");
            return;
        }

        Debug.Log($"Found {boxes.Length} boxes");

        List<int> thiefIndices = new List<int>();

        // Randomly select ONE category for all 5 thieves
        int randCategory = Random.Range(0, clues.GetLength(0));
        Debug.Log($"Selected category {randCategory}");

        List<(int, int)> usedClues = new List<(int, int)>();

        // Place 5 thieves from the selected category
        while (thiefIndices.Count < TOTAL_THIEVES)
        {
            int randomIndex = Random.Range(0, boxes.Length);
            if (!thiefIndices.Contains(randomIndex))
            {
                thiefIndices.Add(randomIndex);
                boxes[randomIndex].SetThief(true);

                string clueName = clues[randCategory, thiefIndices.Count - 1];
                boxes[randomIndex].SetComputerPartName(clueName);
                usedClues.Add((randCategory, thiefIndices.Count - 1));

                Debug.Log($"Thief placed at box {randomIndex} ({clueName})");
            }
        }

        Debug.Log($"All {TOTAL_THIEVES} thieves placed!");

        // Fill remaining boxes with random clues (no duplicates)
        for (int i = 0; i < boxes.Length; i++)
        {
            if (!boxes[i].HasThief())
            {
                int cat = Random.Range(0, clues.GetLength(0));
                int ind = Random.Range(0, 5);

                // Make sure we don't reuse any clues
                while (usedClues.Contains((cat, ind)))
                {
                    cat = Random.Range(0, clues.GetLength(0));
                    ind = Random.Range(0, 5);
                }
                usedClues.Add((cat, ind));

                boxes[i].SetComputerPartName(clues[cat, ind]);

                Debug.Log($"Box {i} ({clues[cat, ind]}): DOES NOT HAVE THIEF");
            }
        }
    }
}