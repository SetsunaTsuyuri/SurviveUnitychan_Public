using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    public partial class Player
    {
        /// <summary>
        /// 通常ステート
        /// </summary>
        public class Normal : FiniteStateMachine<Player>.State
        {
            public override void Enter(Player context)
            {
                // レンダラーが無効になっていれば有効化する
                if (!context._renderersEnabled)
                {
                    context._renderersEnabled = true;
                    context.SetRenderersEnabled(true);
                }

                // 待機アニメ再生
                context.UnitAnimation.Id = UnitAnimationId.Idle;
            }

            public override void Update(Player context)
            {
                // 移動
                context.UpdateMove();

                // 武器
                context.UpdateWeapons();
            }
        }
    }
}
