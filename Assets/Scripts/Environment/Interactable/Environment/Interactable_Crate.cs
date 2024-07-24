using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Crate : Interactable
{
    [SerializeField] private GameObject _originalObj, _shatterObj;
    [SerializeField] private bool _allowRandomItem;
    [SerializeField] private RandomItemDrop _randomItemDrop;
    [SerializeField] private Transform _itemTransform;
    public override void OnInteract(Enum_Weapons weapon)
    {
        if (IsWeaponInclude(weapon))
            Shatter();
    }

    private void Shatter()
    {
        if (_allowRandomItem)
        {
            float luck = Random.Range(0f, 1f);
            foreach (var item in _randomItemDrop.Items)
                if (luck <= item.Chance)
                {
                    ObjectPool.Instance.GetObject(transform.position, item.Item);
                    break;
                }
        }

        _originalObj.SetActive(false);
        _shatterObj.SetActive(true);
    }
}
//GameObject spawnedItem = ObjectPool.Instance.GetHealthPotion(_itemTransform.position);