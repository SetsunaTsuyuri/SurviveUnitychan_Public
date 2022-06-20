using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// マスターデータ
    /// </summary>
    public class MasterData : Singleton<MasterData>, IInitializable
    {
        /// <summary>
        /// 味方ユニットデータ
        /// </summary>
        AllyDataCollection _allies = null;

        /// <summary>
        /// 味方ユニットデータ
        /// </summary>
        public static AllyDataCollection Allies
        {
            get => Instance._allies;
        }

        /// <summary>
        /// 武器データ
        /// </summary>
        WeaponDataCollection _weapons = null;

        /// <summary>
        /// 武器データ
        /// </summary>
        public static WeaponDataCollection Weapons
        {
            get => Instance._weapons;
        }

        /// <summary>
        /// 装飾品データ
        /// </summary>
        AccessoryDataCollection _accessories = null;

        /// <summary>
        /// 装飾品データ
        /// </summary>
        public static AccessoryDataCollection Accessories
        {
            get => Instance._accessories;
        }

        /// <summary>
        /// ステージデータ
        /// </summary>
        StageDataCollection _stages = null;

        /// <summary>
        /// ステージデータ
        /// </summary>
        public static StageDataCollection Stages
        {
            get => Instance._stages;
        }

        /// <summary>
        /// ゲーム設定
        /// </summary>
        GameSettings _gameSettings = null;

        /// <summary>
        /// ゲーム設定
        /// </summary>
        public static GameSettings GameSettings
        {
            get => Instance._gameSettings;
        }

        public override void Initialize()
        {
            _allies = Resources.Load<AllyDataCollection>("MasterData/Units/Allies");
            _weapons = Resources.Load<WeaponDataCollection>("MasterData/Equipments/Weapons");
            _accessories = Resources.Load<AccessoryDataCollection>("MasterData/Equipments/Accessories");
            _stages = Resources.Load<StageDataCollection>("MasterData/Stages");
            _gameSettings = Resources.Load<GameSettings>("MasterData/GameSettings");
        }
    }
}
