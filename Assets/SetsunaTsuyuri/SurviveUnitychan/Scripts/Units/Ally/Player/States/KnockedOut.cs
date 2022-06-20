using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    public partial class Player
    {
        /// <summary>
        /// 戦闘不能ステート
        /// </summary>
        public class KnockedOut : FiniteStateMachine<Player>.State
        {
            public override void Enter(Player context)
            {
                // SEを再生する
                AudioManager.PlaySE("ダメージ");

                // 弾の発射を中止する
                context.CancelFire();

                // 戦闘不能アニメ再生
                context.SetAnimationId(UnitAnimationId.KnockedOut);
            }
        }
    }
}
