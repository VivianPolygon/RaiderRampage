using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEvents : MonoBehaviour
{
    public static UIEvents instance;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public event Action onUpdateScrapCounts;
    public event Action onUpdateAddonCounts;
    public event Action onCheckCosts;
    public event Action onCheckBarrelInventorySpace;
    public event Action onUpdateBarricade;

    
    public event Action onUpdateAll;

    private void OnEnable()
    {
        onUpdateAll += UpdateAddonCounts;
        onUpdateAll += UpdateScrapCounts;
        onUpdateAll += CheckCosts;
        onUpdateAll += CheckBarrelInventorySpace;
        onUpdateAll += UpdateBarricade;
    }

    private void OnDisable()
    {
        onUpdateAll -= UpdateAddonCounts;
        onUpdateAll -= UpdateScrapCounts;
        onUpdateAll -= CheckCosts;
        onUpdateAll -= CheckBarrelInventorySpace;
        onUpdateAll -= UpdateBarricade;
    }

    public void UpdateScrapCounts()
    {
        onUpdateScrapCounts?.Invoke();
    }
    public void UpdateAddonCounts()
    {
        onUpdateAddonCounts?.Invoke();
    }
    public void CheckCosts()
    {
        onCheckCosts?.Invoke();
    }
    public void CheckBarrelInventorySpace()
    {
        onCheckBarrelInventorySpace?.Invoke();
    }

    public void UpdateBarricade()
    {
        onUpdateBarricade?.Invoke();
    }

    public void UpdateAll()
    {
        onUpdateAll?.Invoke();
    }


}
