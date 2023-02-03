using UnityEngine;

public class GameScene : MonoBehaviour
{
    private void Awake()
    {
        PrefabManager.Instance.LoadNecessaryResources();
        Configurator.Instance.SyncLoading("Test");
    }
}