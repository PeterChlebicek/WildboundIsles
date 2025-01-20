using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class FloatingSlot : MonoBehaviour
{
    [SerializeField] InventoryItemData _item;
    [SerializeField] int _amount;
    [SerializeField] int _durability;
    [SerializeField] Inventory _inventory;
    [SerializeField] GameObject _durabilityBarObject;

    [Header("UI")]
    [SerializeField] Image _icon;
    [SerializeField] TextMeshProUGUI _textAmount;
    [SerializeField] Image _durabilityBar;

    Slot _ogSlot;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;

        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
            };

            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if(results.Count > 0 && results[0].gameObject.GetComponent<Slot>() != null)
            {
                Slot secondSlot = results[0].gameObject.GetComponent<Slot>();
                if(secondSlot.Item == _item || secondSlot.Item == null)
                {
                    AddSlot(secondSlot);
                }
                else
                {
                    SwapSlots(secondSlot);
                }
            }
            else
            {
                _inventory.ThrowItems(_item, _amount, _durability);
            }
            _inventory.UpdateSlots();
            gameObject.SetActive(false);

        }
    }

    void SwapSlots(Slot secondSlot)
    {
        _ogSlot.ReplaceItem(secondSlot.Item, secondSlot.Amount, secondSlot.Durability);
        secondSlot.ReplaceItem(_item, _amount, _durability);
    }
    void AddSlot(Slot secondSlot)
    {
        if(secondSlot.Item == null || secondSlot.Amount + _amount <= _item.StackSize)
        {
            secondSlot.AddItem(_item, _amount, _durability);
        }
        else
        {
            if(_amount == _item.StackSize || secondSlot.Amount == _item.StackSize)
            {
                SwapSlots(secondSlot);
            }
            else
            {
                _amount += secondSlot.Amount - _item.StackSize;
                secondSlot.AddItem(_item, _item.StackSize - secondSlot.Amount, _durability);
                _ogSlot.AddItem(_item, _amount, _durability);
            }
        }
    }

    public void SpawnMe(Slot ogSlot, int amount)
    {
        gameObject.SetActive(true);
        _item = ogSlot.Item;
        _amount = amount;
        _durability = ogSlot.Durability;
        _ogSlot = ogSlot;
        _icon.sprite = ogSlot.Item.Icon;
        _textAmount.text = _amount.ToString();

        if(_durability == 0)
        {
            _textAmount.text = _amount.ToString();
            _durabilityBarObject.SetActive(false);
        }
        else
        {
            _textAmount.text = "";
            _durabilityBarObject.SetActive(true);
            _durabilityBar.fillAmount = (float)_durability / _item.Durability;
        }
    }
}
