using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;


public class ObjectManager : MonoBehaviour
{

    public PlayerController player { get; private set; }


    public PlayerController SpawnPlayer(Vector3 _pos)
    {
        GameObject go = Managers.ResourceM.Instantiate("Player", _pooling: false);
        if (go == null) return null;

        player = go.GetOrAddComponent<PlayerController>();
        player.Init();
        Managers.GameM.player = player;
        player.transform.position = _pos;
        return player;
    }

    public Mineral SpawnMineral(Vector3 _pos)
    {
        GameObject go = Managers.ResourceM.Instantiate("Mineral", _pooling: true);
        if (go == null) return null;

        Mineral mineral = go.GetOrAddComponent<Mineral>();
        mineral.transform.position = _pos;
        return mineral;
    }

    public Transform SpawnBackMineral(Vector3 _pos)
    {
        GameObject go = Managers.ResourceM.Instantiate("BackMineral", _pooling: true);
        if (go == null) return null;

        go.transform.position = _pos;
        Vector3 originalScale = go.transform.localScale;
        go.transform.DOScale(originalScale, 0.3f)
        .From(Vector3.zero)
        .SetEase(Ease.OutBack);

        return go.transform;
    }

    public Transform SpawnHandCuff(Vector3 _pos)
    {
        GameObject go = Managers.ResourceM.Instantiate("HandCuffs", _pooling: true);
        if (go == null) return null;

        go.transform.position = _pos;
        Vector3 originScale = go.transform.localScale;
        go.transform.DOScale(originScale, 0.3f)
            .From(Vector3.zero)
            .SetEase(Ease.OutBack);

        return go.transform;
    }

    public PrisonerController SpawnPrisoner(Vector3 _pos)
    {
        GameObject go = Managers.ResourceM.Instantiate("Prisoner", _pooling: true);
        if (go == null) return null;

        PrisonerController prisoner =  go.GetOrAddComponent<PrisonerController>();
        prisoner.transform.position = _pos;
        return prisoner;
    }

    public Transform SpawnMoney(Vector3 _pos)
    {
        GameObject go = Managers.ResourceM.Instantiate("Money", _pooling: true);
        if (go == null) return null;

        go.transform.position = _pos;
        return go.transform;
    }

    public void DeSpawn<T>(T _obj) where T : Component
    {
        if (_obj == null) return;

        Managers.ResourceM.Destroy(_obj.gameObject);

    }
}
