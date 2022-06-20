using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 装備選択ボタンの管理者
    /// </summary>
    public class EquipmentSelectionButtonsManager : SelectableGameUI<EquipmentSelectionButton>
    {
        /// <summary>
        /// ボタンプレハブ
        /// </summary>
        [SerializeField]
        EquipmentSelectionButton prefab = null;

        private void Start()
        {
            Hide();
        }

        /// <summary>
        /// セットアップする
        /// </summary>
        /// <param name="player">プレイヤー</param>
        public void SetUp(Player player)
        {
            // ボタン生成
            int number = MasterData.GameSettings.AvailableEquipmentsOnLevelUp;
            for (int i = 0; i < number; i++)
            {
                // インスタンス生成
                Instantiate(prefab, transform);
            }

            SetUp();

            // ボタンセットアップ
            foreach (var button in buttons)
            {
                button.SetUp(player);
            }
        }

        /// <summary>
        /// ボタンを更新する
        /// </summary>
        /// <param name="player">プレイヤー</param>
        public void UpdateButtons(Player player)
        {
            // 各ボタンを初期化する
            foreach (var button in buttons)
            {
                button.Initialize();
            }

            // 入手可能な装備品
            IEquipment[] availables = player.CreateAvailableEquipments();

            // 入手可能な数
            int max = MasterData.GameSettings.AvailableEquipmentsOnLevelUp;
            int number = Mathf.Min(availables.Length, max);

            // ボタン更新
            if (number > 0)
            {
                // 入手可能な数だけランダムに選ぶ
                IEquipment[] selected = availables
                    .Shuffle()
                    .ToArray();

                // ボタンを更新して表示する
                for (int i = 0; i < number; i++)
                {
                    buttons[i].UpdateAndShow(selected[i]);
                }

                // ボタンのナビゲーションを更新する
                UpdateButtonNavigationsToLoop();
            }
        }
    }
}
