using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 装飾品のレベルデータ
    /// </summary>
    [System.Serializable]
    public class AccessoryLevelData : EquipmentLevelData
    {
        /// <summary>
        /// 攻撃力
        /// </summary>
        public int Attack = 0;

        /// <summary>
        /// 武器の再使用速度
        /// </summary>
        public int RechargeSpeed = 0;

        /// <summary>
        /// 移動速度
        /// </summary>
        public int MoveSpeed = 0;

        /// <summary>
        /// 召喚数
        /// </summary>
        public int Summonses = 0;
    }
}
