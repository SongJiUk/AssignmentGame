using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class UIManager : MonoBehaviour
{
    Queue<UI_Max> uiMaxPool = new();

    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] UI_Max maxPrefab;
    [SerializeField] Canvas canvas;

    public void MoneyTextCheck()
    {
        moneyText.text = Managers.GameM.Money.ToString();
    }

    public void ShowMax(Vector3 _wolrdPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(_wolrdPos);
        UI_Max uiMax = GetMaxUI();
        uiMax.Play(screenPos, () => ReturnUiMax(uiMax));
    }

    UI_Max GetMaxUI()
    {
        if(uiMaxPool.Count > 0)
        {
            var ui = uiMaxPool.Dequeue();
            ui.gameObject.SetActive(true);
            return ui;
        }

        return Instantiate(maxPrefab, canvas.transform);
    }

    void ReturnUiMax(UI_Max _ui)
    {
        _ui.gameObject.SetActive(false);
        uiMaxPool.Enqueue(_ui);
    }

    public bool TryShowMax(Vector3 _worldPos, ref float _lastTime, float _cooldown = 2f)
    {
        if (Time.time - _lastTime < _cooldown) return false;
        _lastTime = Time.time;
        ShowMax(_worldPos);
        return true;
    }
}
