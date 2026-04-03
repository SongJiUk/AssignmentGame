using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{

    static Managers instance;

    UIManager uiManager = null;
    readonly GameManager gameManager = new();
    readonly ResourceManager resourceManager = new();
    readonly ObjectManager objectManager = new();
    readonly PoolManager poolManager = new();

    public static GameManager GameM { get { return Instance?.gameManager; } }
    public static ResourceManager ResourceM { get { return Instance?.resourceManager; } }
    public static ObjectManager ObjectM { get { return Instance?.objectManager; } }
    public static PoolManager PoolM { get { return Instance?.poolManager; } }
    public static UIManager UIM { get { return Instance?.uiManager; } }


    public static Managers Instance
    {
        get
        {
            if (instance != null) return instance;

            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject() { name = "@Managers" };
                go.AddComponent<Managers>();
            }
            DontDestroyOnLoad(go);
            instance = go.GetComponent<Managers>();
            instance.uiManager = go.GetComponent<UIManager>();

            return instance;

        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        uiManager = GetComponent<UIManager>();

        Managers.GameM.Init();
    }


    private void OnDestroy()
    {
        instance = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            GameM.player.OnChangeHoldHandCuff(true);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            GameM.player.OnChangeHoldHandCuff(false);
        }
    }
}
