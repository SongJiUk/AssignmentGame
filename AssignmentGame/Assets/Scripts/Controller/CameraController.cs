using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;

public class CameraController : BaseController
{
    [SerializeField] Transform[] CutScenePoint;

    PlayerController player;
    Vector3 Offset = new Vector3(6.5f, 7f, -10f);
    float smoothSpeed = 3f;
    bool isCutScene = false;
    Camera cam;
    private void Start()
    {
        Init();
    }

    public override bool Init()
    {
        if (!base.Init()) return false;
        player = Managers.GameM.player;
        cam = GetComponent<Camera>();
        return true;
    }

    public async UniTask PlayCutScene(float _duration, int _cutSceneNumber, GameObject[] _targets = null, bool _isWide = false)
    {
        isCutScene = true;
        if (_isWide) cam.orthographicSize = 9;
        await transform.DOMove(CutScenePoint[_cutSceneNumber].position + Offset, 0.8f)
            .SetEase(Ease.InOutBack)
            .AsyncWaitForCompletion();

        

        if (_targets != null)
        {
            foreach (var target in _targets)
            {
                target.SetActive(true);
                Vector3 originScale = target.transform.localScale;
                target.transform.localScale = Vector3.zero;

                var purchaseZone = target.GetComponent<PurchaseZone>();
                if (purchaseZone != null) purchaseZone.isReady = false;

                await DOTween.Sequence()
                            .Append(target.transform.DOScale(originScale * 1.5f, 0.4f).SetEase(Ease.OutBack))
                            .Append(target.transform.DOScale(originScale, 0.3f).SetEase(Ease.InBack))
                            .AsyncWaitForCompletion();

                if (purchaseZone != null) purchaseZone.isReady = true;
            }
        }

        await UniTask.Delay(TimeSpan.FromSeconds(_duration));

        

        await transform.DOMove(player.transform.position + Offset, 0.8f)
            .SetEase(Ease.InOutBack)
            .AsyncWaitForCompletion();

        if (_isWide) cam.orthographicSize = 7;
        isCutScene = false;
    }


    private void LateUpdate()
    {
        if (player == null || isCutScene) return;

        Vector3 target = player.transform.position + Offset;
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * smoothSpeed);
    }

}
