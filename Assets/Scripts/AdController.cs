using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Monetization;

public class AdController : MonoBehaviour
{
    public static AdController Instance;
    string googlePlayStoreId = "3473129";
    string appleStoreId = "3473128";
    public int timesPlayed = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        Monetization.Initialize(googlePlayStoreId, false);
    }

    // Update is called once per frame
    public void LoadAd()
    {
        timesPlayed++;
        if(timesPlayed == 3)
        {
            timesPlayed = 0;
            if(Monetization.IsReady("video"))
            {
                ShowAdPlacementContent ad = null;
                ad = Monetization.GetPlacementContent("video") as ShowAdPlacementContent;
                if(ad != null)
                {
                    ad.Show();
                }
            }
        }
    }
}
