using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UI_Max : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI maxText;
    
    public void Play(Vector3 _screenPos, System.Action _onComplete)
    {
        transform.position = _screenPos;
        maxText.color = Color.red;

        Sequence seq = DOTween.Sequence();
        seq.Join(maxText.transform.DOMoveY(_screenPos.y + 80f, 1.2f).SetEase(Ease.OutCubic));
        seq.Join(maxText.DOFade(0f, 1.2f).SetEase(Ease.InCubic));
        seq.OnComplete(() => _onComplete?.Invoke());
    }
}
