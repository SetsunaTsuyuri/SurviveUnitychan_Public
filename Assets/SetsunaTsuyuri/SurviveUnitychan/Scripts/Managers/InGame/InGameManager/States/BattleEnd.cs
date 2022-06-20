using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    public partial class InGameManager
    {
        /// <summary>
        /// 戦闘終了
        /// </summary>
        private class BattleEnd : FiniteStateMachine<InGameManager>.State
        {
            public override void Enter(InGameManager context)
            {
                // プレイヤー
                Player player = context.Units.Player;

                // 弾の発射を中止する
                player.CancelFire();

                // シーン変更後に破壊されないようにする
                player.transform.SetParent(null);
                DontDestroyOnLoad(player);

                // BGMを停止する
                AudioManager.StopBGM();

                // リザルトシーンに移行する
                SceneChangeManager.ChangeScene(SceneName.Result, context.OnResultSceneLoaded);
            }
        }
    }
}
