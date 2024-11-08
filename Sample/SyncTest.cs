using SyncingDOTSandMono.SyncPlayerLoop;
using UnityEngine;

public class SyncTest : MonoBehaviour
{
    private void Start()
    {
        DOTSMonoSynchronizer.SyncBeforeUpdate += () => Debug.Log($"Before Update");
        DOTSMonoSynchronizer.SyncAfterUpdate += () => Debug.Log($"After Update");
    }

    private void Update()
    {
        Debug.Log($"Unity Update");
    }
}