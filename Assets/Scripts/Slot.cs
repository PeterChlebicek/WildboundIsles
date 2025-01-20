using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour
{
    public InventoryItemData Item;
    public int Amount;
    public int Durability;

    [SerializeField] FloatingSlot _floatingSlot;
    [SerializeField] GameObject _durabilityBarObject;

    [Header("UI")]
    [SerializeField] Image _icon;
    [SerializeField] TextMeshProUGUI _amountText;
    [SerializeField] GameObject _highlight;
    [SerializeField] Image _durabilityBar;

    Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnDisable()
    {
        HighlightSlot(false);
    }

    public void Select()
    {
        _anim.Play("Select");
    }
    public void Deselect()
    {
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Select"))
        {
            _anim.Play("Deselect");
        }
    }
    public void HighlightSlot(bool highlight)
    {
        _highlight.SetActive(highlight);
    }
    void UpdateSlot()
    {
        if(Item != null) 
        {
            _icon.sprite = Item.Icon;
            _icon.color = Color.white;
            if(Item.Durability == 0)
            {
                _amountText.text = Amount.ToString();
                _durabilityBarObject.SetActive(false);
            }
            else
            {
                _amountText.text = "";
                _durabilityBarObject.SetActive(true);
                _durabilityBar.fillAmount = (float)Durability / (float)Item.Durability;
            }
        }
        else
        {
            _icon.color = Color.clear;
            _amountText.text = "";
            _durabilityBarObject.SetActive(false);
        }
    }

    public void AddItem(InventoryItemData item, int amount, int durability)
    {
        Item = item;
        Amount += amount;
        Durability = durability;
        if (_anim != null)
        {
            _anim.Play("AddItem", 1, 0);
        }
        UpdateSlot();
    } 

    public void ReplaceItem(InventoryItemData item, int amount, int durability) 
    {
        Item = item;
        Amount = amount;
        Durability = durability;
        if (_anim != null)
        {
            _anim.Play("AddItem", 1, 0);
        }
        UpdateSlot();
    }

    public void RemoveItem()
    {
        Item = null;
        Amount = 0;
        UpdateSlot();
    }
    public void RemoveItem(int amount)
    {
        Amount -= amount;
        if(Amount == 0)
        {
            Item = null;
        }
        UpdateSlot();
    }
    public void SpawnFloatingSlot()
    {
        if (Item != null)
        {
            if (Input.GetMouseButton(0))
            {
                _floatingSlot.SpawnMe(this, Amount);
                RemoveItem();
            }
            else
            {
                int myAmount = Amount / 2;
                int floatingSlotAmount = Amount - myAmount;
                _floatingSlot.SpawnMe(this, floatingSlotAmount);
                RemoveItem(floatingSlotAmount);
            }
        }
    }
    public void DecreaseDurability()
    {
        Durability -= 10;
        UpdateSlot();
    }
}
