using System;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace SyncingDOTSandMono.SyncPlayerLoop
{
    // Mono�A�b�v�f�[�g�̑O�Ɏ��s����
    public struct SyncBeforeMonoUpdate { }
    // Mono�A�b�v�f�[�g�̌�Ɏ��s����
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
            // �A�b�v�f�[�g�O�ɍs��������o�^
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

            // PlayerLoop���擾���ăA�b�v�f�[�g�̃^�C�~���O��T��
            var playerLoop = PlayerLoop.GetDefaultPlayerLoop();
            for (int i = 0; i < playerLoop.subSystemList.Length; i++)
            {
                if (playerLoop.subSystemList[i].type == typeof(Update))
                {
                    // �A�b�v�f�[�g�̏�Ԃ�ێ���������������ꏊ��ǉ�����
                    playerLoop.subSystemList[i] = new PlayerLoopSystem
                    {
                        type = playerLoop.subSystemList[i].type,
                        updateDelegate = playerLoop.subSystemList[i].updateDelegate,
                        subSystemList = playerLoop.subSystemList[i].subSystemList
                            .Prepend(syncBefore)    // �A�b�v�f�[�g�̐擪�ɒǉ�
                            .Append(syncAfter)      // �A�b�v�f�[�g�̍Ō�ɒǉ�
                            .ToArray(),
                        updateFunction = playerLoop.subSystemList[i].updateFunction,
                        loopConditionFunction = playerLoop.subSystemList[i].loopConditionFunction
                    };
                    break;  // �������̂ŏI��
                }
            }
            // PlayerLoop�ɍ쐬�������[�v��ǉ�
            PlayerLoop.SetPlayerLoop(playerLoop);
        }
    }
}