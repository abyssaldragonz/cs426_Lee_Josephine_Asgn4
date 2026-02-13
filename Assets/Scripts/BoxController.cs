using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class BoxController : NetworkBehaviour
{
    // Computer part name on this box
    public string computerPartName;
    
    // Is there a thief hidden here? (regular bool, NOT NetworkVariable)
    [HideInInspector] public bool hasThief = false;
    
    // Has this box been revealed?
    private NetworkVariable<bool> isRevealed = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    
    // UI elements
    [SerializeField] private TMP_Text nameText;
    
    // Materials for visual feedback
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material correctMaterial;
    [SerializeField] private Material wrongMaterial;

    void Start()
    {
        nameText.text = computerPartName;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        isRevealed.OnValueChanged += OnRevealChanged;
        nameText.text = computerPartName;
    }

    void OnRevealChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            UpdateBoxVisual();
        }
    }

    // Called when Firewall guesses this box
    public bool RevealBox()
    {
        Debug.Log($"   RevealBox on {gameObject.name}");
        Debug.Log($"   hasThief = {hasThief}");
        Debug.Log($"   isRevealed = {isRevealed.Value}");
        
        if (isRevealed.Value)
        {
            Debug.Log("Already revealed!");
            return false;
        }
        
        isRevealed.Value = true;
        UpdateBoxVisual();
        
        Debug.Log($"   Returning: {hasThief}");
        return hasThief;
    }

    public bool IsRevealed()
    {
        return isRevealed.Value;
    }

    void UpdateBoxVisual()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null && isRevealed.Value)
        {
            renderer.material = hasThief ? correctMaterial : wrongMaterial;
            Debug.Log($"Updated visual for {gameObject.name}: {(hasThief ? "GREEN (thief)" : "RED (no thief)")}");
        }
    }

    // Server sets which boxes have thieves
    public void SetThief(bool hasThiefHere)
    {
        this.hasThief = hasThiefHere;
        Debug.Log($" SetThief on {gameObject.name}: {hasThiefHere}");
        nameText.text = computerPartName;
    }

    public bool HasThief()
    {
        return hasThief;
    }

    public override void OnNetworkDespawn()
    {
        isRevealed.OnValueChanged -= OnRevealChanged;
        base.OnNetworkDespawn();
    }
}