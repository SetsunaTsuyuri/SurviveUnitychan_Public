using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    public partial class Player
    {
        /// <summary>
        /// 被ダメージステート
        /// </summary>
        private class Damaged : FiniteStateMachine<Player>.State
        {
            public override void Enter(Player context)
            {
                // SEを再生する
                AudioManager.PlaySE("ダメージ");

                // 弾の発射を中止する
                context.CancelFire();

                // 障害物を回避しなくなる
                context._agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

                // 点滅時間設定
                context.BlinkingTimeCount = MasterData.GameSettings.UnitBlinkingInterval;

                // 気絶時間設定
                context.RemainningStunnedTime = MasterData.GameSettings.PlayerStunnedTime;

                // ダメージアニメ再生
                context.SetAnimationId(UnitAnimationId.Damaged);
            }

            public override void Update(Player context)
            {
                // 一定間隔で点滅する
                context.BlinkAtRegularIntervals();

                // 気絶時間を減らす
                context.RemainningStunnedTime -= Time.deltaTime;
                
                // 気絶時間が終了した場合
                if (context.RemainningStunnedTime == 0.0f)
                {
                    // 無敵移動ステートに移行する
                    context.State.Change<InvincibleMove>();
                }
            }
        }
    }
}
