using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 装備品
    /// </summary>
    /// <typeparam name="TEquipmentData">基本データ</typeparam>
    /// <typeparam name="TEquipmentLevelData">レベルデータ</typeparam>
    public abstract class Equipment<TEquipmentData, TEquipmentLevelData> : IEquipment
        where TEquipmentData : EquipmentData<TEquipmentLevelData>
        where TEquipmentLevelData : EquipmentLevelData
    {
        /// <summary>
        /// レベル
        /// </summary>
        public int Level { get; set; } = 0;

        /// <summary>
        /// データ
        /// </summary>
        public TEquipmentData Data { get; set; } = null;

        /// <summary>
        /// 現在のレベルデータを取得する
        /// </summary>
        /// <returns></returns>
        public virtual TEquipmentLevelData GetCurrentLevelData()
        {
            int max = Data.Levels.Length;

            int index = Mathf.Clamp(Level - 1, 0, max);
            return Data.Levels[index];
        }

        /// <summary>
        /// 装備品データを取得する
        /// </summary>
        /// <returns></returns>
        public EquipmentData<EquipmentLevelData> GetEquipmentData()
        {
            EquipmentData<EquipmentLevelData> data = new()
            {
                Id = Data.Id,
                Name = Data.Name,
                Icon = Data.Icon,
                Description = Data.Description,
                Levels = Data.Levels

            };

            return data;
        }

        /// <summary>
        /// 装備品レベルデータを取得する
        /// </summary>
        /// <returns></returns>
        public EquipmentLevelData GetCurrentEquipmentLevelData()
        {
            return GetCurrentLevelData();
        }

        /// <summary>
        /// 最大レベルである
        /// </summary>
        /// <returns></returns>
        public bool IsMaxLevel()
        {
            return Level >= Data.Levels.Length;
        }
    }
}
