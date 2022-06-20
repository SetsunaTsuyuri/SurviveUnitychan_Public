using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 装備品選択ボタン
    /// </summary>
    public class EquipmentSelectionButton : GameButton, IInitializable
    {
        /// <summary>
        /// 説明文
        /// </summary>
        TextMeshProUGUI _description = null;

        /// <summary>
        /// アイコン
        /// </summary>
        [SerializeField]
        Image _icon = null;

        /// <summary>
        /// 装備品
        /// </summary>
        IEquipment _equipment = null;

        /// <summary>
        /// 装備品
        /// </summary>
        public IEquipment Equipment
        {
            get => _equipment;
            set
            {
                _equipment = value;
                OnEquipmentSet();
            }
        }

        protected override void Awake()
        {
            base.Awake();

            // コンポーネント取得
            _description = GetComponentInChildren<TextMeshProUGUI>(true);
        }

        public void Initialize()
        {
            Equipment = null;
        }

        /// <summary>
        /// 装備品が設定されたときの処理
        /// </summary>
        private void OnEquipmentSet()
        {
            if (Equipment != null)
            {
                // データ
                EquipmentData<EquipmentLevelData> data = Equipment.GetEquipmentData();
                
                // アイコンを設定する
                _icon.sprite = data.Icon;

                // レベルの表示を設定する
                string level;
                if (Equipment.Level == MasterData.GameSettings.MinLevel)
                {
                    level = MasterData.GameSettings.NewEquipmentLevelDisplay;
                }
                else
                {
                    level = $"Lv.{Equipment.Level}";
                }

                // 説明文を設定する
                _description.text = 
                    $"【{data.Name}】{level}\n{data.Description}";
            }
            else
            {
                Hide();
            }
        }

        /// <summary>
        /// セットアップする
        /// </summary>
        /// <param name="player">プレイヤー</param>
        public void SetUp(Player player)
        {
            AddOnClickListener(() =>
            {
                switch (Equipment)
                {
                    // 武器獲得
                    case Weapon weapon:
                        player.ObtainEquipment(weapon);
                        break;

                    // 装飾品獲得
                    case Accessory accessory:
                        player.ObtainEquipment(accessory);
                        break;
                }
            });
        }

        /// <summary>
        /// 更新して表示する
        /// </summary>
        /// <param name="equipment">装備品</param>
        public void UpdateAndShow(IEquipment equipment)
        {
            Equipment = equipment;
            Show();
        }
    }
}
