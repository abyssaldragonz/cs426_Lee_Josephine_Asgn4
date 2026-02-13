using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class BoxController : NetworkBehaviour
{
    // Computer part name on this box
    public string computerPartName;
    
    private NetworkVariable<bool> hasThief = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    
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
        Debug.Log($"   hasThief = {hasThief.Value}"); // Add .Value
        Debug.Log($"   isRevealed = {isRevealed.Value}");
        
        if (isRevealed.Value)
        {
            Debug.Log("Already revealed!");
            return false;
        }
        
        isRevealed.Value = true;
        UpdateBoxVisual();
        
        Debug.Log($"   Returning: {hasThief.Value}"); // Add .Value
        return hasThief.Value; // Add .Value
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
            renderer.material = hasThief.Value ? correctMaterial : wrongMaterial; // Add .Value
            Debug.Log($"Updated visual for {gameObject.name}: {(hasThief.Value ? "GREEN (thief)" : "RED (no thief)")}"); // Add .Value
        }
    }

    // Server sets which boxes have thieves
    public void SetThief(bool hasThiefHere)
    {
        this.hasThief.Value = hasThiefHere; // Add .Value
        Debug.Log($" SetThief on {gameObject.name}: {hasThiefHere}");
        nameText.text = computerPartName;
    }

    public bool HasThief()
    {
        return hasThief.Value;
    }

    public override void OnNetworkDespawn()
    {
        isRevealed.OnValueChanged -= OnRevealChanged;
        base.OnNetworkDespawn();
    }
}