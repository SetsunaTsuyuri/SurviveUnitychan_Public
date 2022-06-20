using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    public partial class Player
    {
        /// <summary>
        /// 無敵移動ステート
        /// </summary>
        private class InvincibleMove : FiniteStateMachine<Player>.State
        {
            public override void Enter(Player context)
            {
                // 無敵時間設定
                context.RemainningInvincibleTime = MasterData.GameSettings.PlayerInvincibleTime;
            }

            public override void Update(Player context)
            {
                // 移動
                context.UpdateMove();

                // 一定間隔で点滅させる
                context.BlinkAtRegularIntervals();

                // 無敵時間を減らす
                context.RemainningInvincibleTime -= Time.deltaTime;

                // 無敵時間が終了した場合
                if (context.RemainningInvincibleTime <= 0.0f)
                {
                    // 通常ステートに移行する
                    context.State.Change<Normal>();
                }
            }

            public override void Exit(Player context)
            {
                // 障害物回避設定を元に戻す
                context._agent.obstacleAvoidanceType = context._initialQuality;
            }
        }
    }
}
