using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    public partial class InGameManager
    {
        /// <summary>
        /// 戦闘中
        /// </summary>
        private class InBattle : FiniteStateMachine<InGameManager>.State
        {
            public override void Update(InGameManager context)
            {
                // ユニット更新
                context.Units.UpdateUnits();
                context.Units.UpdateEnemyReinforcements(context.StageData);

                // アイテム更新
                Vector3 playerPosition = context.Units.Player.transform.position;
                context._items.UpdateItems(playerPosition);

                // 残り時間を減らす
                context.RemainningTime -= Time.deltaTime;

                // 残り時間がゼロになるかプレイヤーがやられたら終了
                if (context.RemainningTime == 0.0f ||
                    !context.Units.Player.IsLiving())
                {
                    // 戦闘終了ステートへ移行する
                    context._state.Change<BattleEnd>();
                }
            }
        }
    }
}
