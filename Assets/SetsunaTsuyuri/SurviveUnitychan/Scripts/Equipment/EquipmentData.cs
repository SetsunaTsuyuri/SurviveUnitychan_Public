using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 装備品のデータ
    /// </summary>
    public class EquipmentData<TEquipmentLevelData>
        where TEquipmentLevelData : EquipmentLevelData
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id = 0;

        /// <summary>
        /// 名前
        /// </summary>
        public string Name = string.Empty;

        /// <summary>
        /// アイコン
        /// </summary>
        public Sprite Icon = null;

        /// <summary>
        /// 説明文
        /// </summary>
        public string Description = string.Empty;

        /// <summary>
        /// レベル毎のデータ
        /// </summary>
        public TEquipmentLevelData[] Levels = { };
    }
}
