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

    public event Action onUpdateAll;

    private void Start()
    {
        onUpdateAll += UpdateAddonCounts;
        onUpdateAll += UpdateScrapCounts;
    }

    public void UpdateScrapCounts()
    {
        onUpdateScrapCounts?.Invoke();
    }
    public void UpdateAddonCounts()
    {
        onUpdateAddonCounts?.Invoke();
    }
    public void UpdateAll()
    {
        onUpdateAll?.Invoke();
    }


}
