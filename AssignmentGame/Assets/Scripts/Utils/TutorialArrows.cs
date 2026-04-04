using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TutorialArrows : MonoBehaviour
{
    [SerializeField] GameObject spinArrowPivot;
    [SerializeField] GameObject spinArrow;
    [SerializeField] GameObject dirArrowPivot;
    [SerializeField] GameObject dirArrow;

    Transform target;
    Transform player;
    Camera cam;
    Tween spinTween;


    public void Init()
    {
        cam = Camera.main;
        player = Managers.GameM.player.transform;
        dirArrow.transform.localPosition = new Vector3(1.5f, 0, 0f);
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
        gameObject.SetActive(true);

        spinArrowPivot.transform.localRotation = Quaternion.identity;
        spinTween?.Kill();
        spinTween = spinArrowPivot.transform
            .DORotate(new Vector3(0, 360, 0), 1.5f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }


    public void Hide()
    {
        spinTween?.Kill();
        target = null;
        spinArrow.SetActive(false);
        dirArrow.SetActive(false);
    }
    public void EndTutorial()
    {
        spinTween?.Kill();
        target = null;
        spinArrow.SetActive(false);
        dirArrow.SetActive(false);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (target == null) return;

        bool isVisible = IsTargetVisible();
        spinArrow.SetActive(isVisible);
        dirArrow.SetActive(!isVisible);

        if(isVisible)
        {
            spinArrowPivot.transform.position = target.position + Vector3.up * 1.5f;
        }
        else
        {
            dirArrowPivot.transform.position = player.position + Vector3.up * 1.5f;

            Vector3 dir = (target.position - player.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero)
                dirArrowPivot.transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(0, -90, 0);
        }
    } 


    bool IsTargetVisible()
    {
        Vector3 screenPos = cam.WorldToViewportPoint(target.position);
        return screenPos.z > 0 && screenPos.x > 0 && screenPos.x < 1 && screenPos.y > 0 && screenPos.y < 1;
    }
}
