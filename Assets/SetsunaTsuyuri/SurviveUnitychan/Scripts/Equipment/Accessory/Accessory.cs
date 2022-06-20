using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 装飾品
    /// </summary>
    public class Accessory : Equipment<AccessoryData, AccessoryLevelData>
    {
        /// <summary>
        /// 味方ユニットのオブジェクトプール
        /// </summary>
        SummonedAlly[] _allyPool = { };

        /// <summary>
        /// 味方ユニットのオブジェクトプールを作る
        /// </summary>
        /// <param name="units">ユニットの管理者</param>
        public void CreateAllyPool(UnitsManager units)
        {
            int max = Data.Levels.Max(x => x.Summonses);
            if (max <= 0)
            {
                return;
            }

            SummonedAlly prefab = Resources.Load<SummonedAlly>("Prefabs/SummonedAlly");
            _allyPool = new SummonedAlly[max];
            for (int i = 0; i < max; i++)
            {
                // プレハブインスタンス化
                SummonedAlly instance = Object.Instantiate(prefab);

                // 非アクティブ化
                instance.gameObject.SetActive(false);

                // 管理者設定
                instance.Units = units;

                // データ設定
                instance.Data = MasterData.Allies.GetValueOrDefault(Data.SummonsAllyId);

                // レイヤー設定
                instance.SetLayer(LayerName.Ally);

                // プールに追加
                _allyPool[i] = instance;

                // 味方ユニットリストに追加
                units.Allies.Add(instance);
            }
        }

        /// <summary>
        /// 味方ユニットのオブジェクトプールを消す
        /// </summary>
        public void DeleteAllyPool()
        {
            foreach (var ally in _allyPool)
            {
                Object.Destroy(ally);
            }
            _allyPool = null;
        }

        /// <summary>
        /// 召喚の更新処理を行う
        /// </summary>
        /// <param name="ownerPosition">所有者の位置</param>
        public void UpdateSummons(Vector3 ownerPosition)
        {
            AccessoryLevelData levelData = GetCurrentLevelData();
            SummonedAlly[] activeSummonedAllies = _allyPool
                .Where(x => x.isActiveAndEnabled)
                .ToArray();
            
            if (activeSummonedAllies.Length < levelData.Summonses)
            {
                // アクティブにすべき味方ユニットの数
                int toBeActivated = levelData.Summonses - activeSummonedAllies.Length;

                // 非アクティブの味方ユニット
                SummonedAlly[] nonActiveSummonedAllies = _allyPool
                    .Where(x => !x.isActiveAndEnabled)
                    .ToArray();

                // 出現させる
                for (int i = 0; i < toBeActivated && i < nonActiveSummonedAllies.Length; i++)
                {
                    nonActiveSummonedAllies[i].Spawn(ownerPosition + Vector3.forward);
                }
            }
            else if (activeSummonedAllies.Length > levelData.Summonses)
            {
                // 非アクティブにすべき味方ユニットの数
                int toBeDeactivated = activeSummonedAllies.Length - levelData.Summonses;

                // 非アクティブにする
                for (int i = 0; i < toBeDeactivated && i < activeSummonedAllies.Length; i++)
                {
                    activeSummonedAllies[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
