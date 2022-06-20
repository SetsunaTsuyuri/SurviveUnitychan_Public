using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 装飾品のデータ
    /// </summary>
    [System.Serializable]
    public class AccessoryData : EquipmentData<AccessoryLevelData>
    {
        /// <summary>
        /// 召喚する味方ユニットのID
        /// </summary>
        public int SummonsAllyId = 0;
    }
}
