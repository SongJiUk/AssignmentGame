using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;

public class CameraController : BaseController
{
    PlayerController player;
    Vector3 Offset = new Vector3(6.5f, 7f, -10f);
    float smoothSpeed = 3f;
    bool isCutScene = false;
    private void Start()
    {
        Init();
    }

    public override bool Init()
    {
        if (!base.Init()) return false;
        player = Managers.GameM.player;
        return true;
    }

    public async UniTaskVoid PlayCutScene(PurchaseZone _zone, float _duration)
    {
        isCutScene = true;

        await transform.DOMove(_zone.transform.position + Offset, 0.8f)
            .SetEase(Ease.InOutBack)
            .AsyncWaitForCompletion();

        Vector3 originScale = _zone.transform.localScale;
        _zone.transform.DOScale(originScale, _duration/2)
            .From(Vector3.zero)
            .SetEase(Ease.OutBack);

        await UniTask.Delay(TimeSpan.FromSeconds(_duration / 2));

        await transform.DOMove(player.transform.position + Offset, 0.8f)
            .SetEase(Ease.InOutBack)
            .AsyncWaitForCompletion();

        isCutScene = false;
    }


    private void LateUpdate()
    {
        if (player == null || isCutScene) return;

        Vector3 target = player.transform.position + Offset;
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * smoothSpeed);
    }

}
