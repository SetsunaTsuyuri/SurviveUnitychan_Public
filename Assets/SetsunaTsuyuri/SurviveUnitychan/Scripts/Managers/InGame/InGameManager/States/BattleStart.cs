using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    public partial class InGameManager
    {
        /// <summary>
        /// 戦闘開始
        /// </summary>
        private class BattleStart : FiniteStateMachine<InGameManager>.State
        {
            public override void Enter(InGameManager context)
            {
                // ユニットのセットアップ
                context.Units.SetUp(context);

                // 敵が倒されたとき、敵撃破数を増やす
                foreach (var enemy in context.Units.Enemies)
                {
                    enemy.Killed
                        .TakeUntilDestroy(context)
                        .Subscribe((_) =>context.DefeatedEnemiesCount++);
                }

                // アイテムのセットアップ
                context._items.SetUp(context);

                // UIのセットアップ
                context._ui.SetUp(context);

                // 残り時間セット
                context.RemainningTime = context.StageData.TimeToClear;

                // プレイヤーがレベルアップしたときの処理購読
                context.Units.Player.LevelUpped
                    .TakeUntilDestroy(context)
                    .Subscribe(x => context.OnPlayerLevelUp(x));

                // BGMを再生する
                AudioManager.PlayBGM(context.StageData.BgmName);

                // 戦闘中ステートへ移行する
                context._state.Change<InBattle>();
            }

            public override void Update(InGameManager context)
            {
                // ユニット更新
                context.Units.UpdateUnits();

                // 残り時間を減らす
                context.RemainningTime -= Time.deltaTime;

                // 残り時間がゼロになったらゲームクリア
                if (context.RemainningTime > 0.0f)
                {
                    context._state.Change<BattleEnd>();
                }
            }
        }
    }
}
