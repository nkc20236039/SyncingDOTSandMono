using System;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace SyncingDOTSandMono.SyncPlayerLoop
{
    // Monoアップデートの前に実行する
    public struct SyncBeforeMonoUpdate { }
    // Monoアップデートの後に実行する
    public struct SyncAfterMonoUpdate { }

    public static class DOTSMonoSynchronizer
    {
        public static event Action SyncBeforeUpdate;
        public static event Action SyncAfterUpdate;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initalize()
        {
            InjectBeforeUpdate();
        }

        private static void InjectBeforeUpdate()
        {
            // アップデート前に行う処理を登録
            var syncBefore = new PlayerLoopSystem
            {
                type = typeof(SyncBeforeMonoUpdate),
                updateDelegate = () => SyncBeforeUpdate?.Invoke()
            };

            var syncAfter = new PlayerLoopSystem
            {
                type = typeof(SyncAfterMonoUpdate),
                updateDelegate = () => SyncAfterUpdate?.Invoke()
            };

            // PlayerLoopを取得してアップデートのタイミングを探索
            var playerLoop = PlayerLoop.GetDefaultPlayerLoop();
            for (int i = 0; i < playerLoop.subSystemList.Length; i++)
            {
                if (playerLoop.subSystemList[i].type == typeof(Update))
                {
                    // アップデートの状態を保持しつつ同期処理する場所を追加する
                    playerLoop.subSystemList[i] = new PlayerLoopSystem
                    {
                        type = playerLoop.subSystemList[i].type,
                        updateDelegate = playerLoop.subSystemList[i].updateDelegate,
                        subSystemList = playerLoop.subSystemList[i].subSystemList
                            .Prepend(syncBefore)    // アップデートの先頭に追加
                            .Append(syncAfter)      // アップデートの最後に追加
                            .ToArray(),
                        updateFunction = playerLoop.subSystemList[i].updateFunction,
                        loopConditionFunction = playerLoop.subSystemList[i].loopConditionFunction
                    };
                    break;  // 見つけたので終了
                }
            }
            // PlayerLoopに作成したループを追加
            PlayerLoop.SetPlayerLoop(playerLoop);
        }
    }
}