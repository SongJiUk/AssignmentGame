using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class PrisonRoom : BaseController
{
    [SerializeField] PurchaseZone upgradeZone;
    [SerializeField] RoomData currentData;
    [SerializeField] Transform Door;
    [SerializeField] Transform PrisonLevel1_DisableObject;
    [SerializeField] TextMeshProUGUI PrisonerCounttext;
    int currentCount = 0;
    int reserveCount = 0;
    public bool HasSpace => (currentCount + reserveCount) < currentData.maxCount;
    public bool isFull => currentCount < currentData.maxCount;
    bool hasWaitingPrisoner = false;
    public bool HasWaitingPrisoner => hasWaitingPrisoner;
    public void SetWaitingPrisoner(bool _value) => hasWaitingPrisoner = _value;
    
    public void Reserve()
    {
        reserveCount++;
    }
    public void Enter()
    {
        reserveCount--;
        currentCount++;
        PrisonerCounttext.text = $"{currentCount} / {currentData.maxCount}";
        if (!isFull)
        {
            PrisonerCounttext.color = Color.red;
            Door.gameObject.SetActive(true);
            Managers.GameM.cam.PlayCutScene(2f,1, new GameObject[] { upgradeZone.gameObject }).Forget();
        }
        else
            PrisonerCounttext.color = Color.white;
    }

    public async UniTaskVoid Upgrade(GameObject[] _targets)
    {
        PrisonLevel1_DisableObject.gameObject.SetActive(false);
        Door.gameObject.SetActive(false);
        currentData = currentData.nextLevel;
        PrisonerCounttext.text = $"{currentCount} / {currentData.maxCount}";
        PrisonerCounttext.color = Color.white;

        await Managers.GameM.cam.PlayCutScene(2f, 2, _targets, _isWide: true);

      
    }
}
