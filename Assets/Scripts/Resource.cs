using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] private string resourceName;
    [SerializeField] private float health;
    [SerializeField] private int requiredTier;
    [SerializeField] private GameObject drop;
    [SerializeField] private int minDropAmount = 1;
    [SerializeField] private int maxDropAmount = 5;
    [SerializeField] private bool combineDrops = true;

    private bool isHarvested = false;

    public string ResourceName => resourceName;
    public float Health => health;
    public int RequiredTier => requiredTier;

    public void Harvest(Resource resource, float damage)
    {
        if (damage >= 0)
        {
            health -= damage;

            if (health <= 0 && !isHarvested)
            {
                isHarvested = true;
                Harvested();
            }
        }
        else
        {
            Debug.LogWarning("Damage value is invalid.");
        }
    }

    private void Harvested()
    {
        Debug.Log($"{resourceName} has been harvested!");

        int totalAmount = Random.Range(minDropAmount, maxDropAmount + 1);

        if (drop != null)
        {
            if (combineDrops)
            {
                GameObject droppedItem = Instantiate(drop, transform.position, Quaternion.identity);
                Item itemComponent = droppedItem.GetComponent<Item>();
                if (itemComponent != null)
                {
                    itemComponent.Amount = totalAmount;
                }
            }
            else
            {
                for (int i = 0; i < totalAmount; i++)
                {
                    GameObject droppedItem = Instantiate(drop, GetRandomDropPosition(), Quaternion.identity);
                    Item itemComponent = droppedItem.GetComponent<Item>();
                    if (itemComponent != null)
                    {
                        itemComponent.Amount = 1;
                    }
                }
            }
        }
        Destroy(gameObject);
    }

    private Vector3 GetRandomDropPosition()
    {
        float range = 2f;
        return new Vector3(
            transform.position.x + Random.Range(-range, range),
            transform.position.y,
            transform.position.z + Random.Range(-range, range)
        );
    }
}
