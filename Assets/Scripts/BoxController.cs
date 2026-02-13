using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections; // ADD THIS for FixedString

public class BoxController : NetworkBehaviour
{
   
    private NetworkVariable<FixedString64Bytes> computerPartName = new NetworkVariable<FixedString64Bytes>(
        "",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    
    private NetworkVariable<bool> hasThief = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    
    private NetworkVariable<bool> isRevealed = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material correctMaterial;
    [SerializeField] private Material wrongMaterial;

    void Start()
    {
        // Update to use .Value
        nameText.text = computerPartName.Value.ToString();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        isRevealed.OnValueChanged += OnRevealChanged;
        
        
        computerPartName.OnValueChanged += OnNameChanged;
        
        // Update text with current value
        nameText.text = computerPartName.Value.ToString();
    }

  
    void OnNameChanged(FixedString64Bytes oldValue, FixedString64Bytes newValue)
    {
        nameText.text = newValue.ToString();
        Debug.Log($"Name updated to: {newValue}");
    }

    void OnRevealChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            UpdateBoxVisual();
        }
    }

    public bool RevealBox()
    {
        Debug.Log($"   RevealBox on {gameObject.name}");
        Debug.Log($"   hasThief = {hasThief.Value}");
        Debug.Log($"   isRevealed = {isRevealed.Value}");
        
        if (isRevealed.Value)
        {
            Debug.Log("Already revealed!");
            return false;
        }
        
        isRevealed.Value = true;
        UpdateBoxVisual();
        
        Debug.Log($"   Returning: {hasThief.Value}");
        return hasThief.Value;
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
            renderer.material = hasThief.Value ? correctMaterial : wrongMaterial;
            Debug.Log($"Updated visual for {gameObject.name}: {(hasThief.Value ? "GREEN (thief)" : "RED (no thief)")}");
        }
    }

    // Server sets which boxes have thieves
    public void SetThief(bool hasThiefHere)
    {
        this.hasThief.Value = hasThiefHere;
        Debug.Log($" SetThief on {gameObject.name}: {hasThiefHere}");
    }

   
    public void SetComputerPartName(string name)
    {
        computerPartName.Value = name;
        nameText.text = name;
        Debug.Log($"SetComputerPartName on {gameObject.name}: {name}");
    }

    public bool HasThief()
    {
        return hasThief.Value;
    }

    public override void OnNetworkDespawn()
    {
        isRevealed.OnValueChanged -= OnRevealChanged;
        computerPartName.OnValueChanged -= OnNameChanged; 
        base.OnNetworkDespawn();
    }
}