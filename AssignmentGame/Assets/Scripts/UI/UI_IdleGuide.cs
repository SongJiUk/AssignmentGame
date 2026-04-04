using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class UI_IdleGuide : MonoBehaviour
{
    [SerializeField] RectTransform fingerRect;

    const float idleThreshold = 4f;

    float lastMoveTime;
    Transform player;
    Tween InfinityTween;
    bool isPlaying = false;

    Vector2 offset = new Vector2(0, -100f);
    private void Start()
    {
        player = Managers.GameM.player.transform;
        lastMoveTime = Time.time;
        fingerRect.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(player.hasChanged)
        {
            lastMoveTime = Time.time;
            player.hasChanged = false;
            if (isPlaying) HideFinger();
        }

        if (!isPlaying && Time.time - lastMoveTime >= idleThreshold)
            ShowFinger();
    }

    void ShowFinger()
    {
        isPlaying = true;
        fingerRect.gameObject.SetActive(true);
        PlayInfinityLoop();
    }

    void HideFinger()
    {
        isPlaying = false;
        InfinityTween?.Kill();
        fingerRect.anchoredPosition = Vector2.zero;
        fingerRect.gameObject.SetActive(false);
    }

    void PlayInfinityLoop()
    {
        float width = 100f;
        float height = 60f;
        InfinityTween = DOTween.To(t =>
        {
            float angle = t * Mathf.PI * 2f;
            float x = width * Mathf.Sin(angle);
            float y = height * Mathf.Sin(angle * 2f);
            fingerRect.anchoredPosition = offset + new Vector2(x, y);
        }, 0f, 1f, 2f)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }

}
