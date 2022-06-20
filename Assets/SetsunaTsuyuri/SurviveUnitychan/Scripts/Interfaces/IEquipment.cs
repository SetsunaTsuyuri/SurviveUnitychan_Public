namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 装備品
    /// </summary>
    public interface IEquipment
    {
        /// <summary>
        /// 装備品データを取得する
        /// </summary>
        /// <returns></returns>
        EquipmentData<EquipmentLevelData> GetEquipmentData();

        /// <summary>
        /// 装備品レベルデータを取得する
        /// </summary>
        /// <returns></returns>
        EquipmentLevelData GetCurrentEquipmentLevelData();

        /// <summary>
        /// レベル
        /// </summary>
        int Level { get; set; }

        /// <summary>
        /// 最大レベルである
        /// </summary>
        /// <returns></returns>
        public bool IsMaxLevel()
        {
            return Level == GetEquipmentData().Levels.Length;
        }
    }
}
