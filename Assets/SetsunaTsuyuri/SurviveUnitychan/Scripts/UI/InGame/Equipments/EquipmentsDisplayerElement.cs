using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 装備表示UIの要素
    /// </summary>
    public class EquipmentsDisplayerElement : GameUI
    {
        /// <summary>
        /// アイコン
        /// </summary>
        [SerializeField]
        Image _icon = null;

        /// <summary>
        /// レベル
        /// </summary>
        TextMeshProUGUI level = null;

        protected override void Awake()
        {
            base.Awake();

            level = GetComponentInChildren<TextMeshProUGUI>(true);
        }

        /// <summary>
        /// 表示をクリアする
        /// </summary>
        public void Clear()
        {
            _icon.sprite = null;
            _icon.enabled = false;
            level.text = string.Empty;
        }

        /// <summary>
        /// 表示を更新する
        /// </summary>
        /// <param name="equipment">装備</param>
        public void UpdateDisplay(IEquipment equipment)
        {
            // アイコン
            _icon.sprite = equipment.GetEquipmentData().Icon;
            _icon.enabled = _icon.sprite != null;

            // テキスト
            level.text = $"Lv.{equipment.Level}";
        }
    }
}
