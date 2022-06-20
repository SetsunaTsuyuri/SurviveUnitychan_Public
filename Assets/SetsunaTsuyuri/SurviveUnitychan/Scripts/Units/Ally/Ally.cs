using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 味方
    /// </summary>
    /// <typeparam name="TAlly">味方</typeparam>
    public abstract class Ally<TAlly> : Unit<TAlly, AllyData>
        where TAlly : Unit<TAlly, AllyData>
    {
        /// <summary>
        /// 攻撃力
        /// </summary>
        public int Attack { get; set; } = 0;

        /// <summary>
        /// 武器の再使用速度
        /// </summary>
        public int RechargeSpeed { get; set; } = 0;

        /// <summary>
        /// ユニットの管理者
        /// </summary>
        public UnitsManager Units { get; set; } = null;

        /// <summary>
        /// 武器
        /// </summary>
        public List<Weapon> Weapons { get; protected set; } = new List<Weapon>();

        /// <summary>
        /// 装飾品
        /// </summary>
        public List<Accessory> Accessories { get; protected set; } = new List<Accessory>();

        /// <summary>
        /// 発射装置
        /// </summary>
        public Shooter Shooter { get; protected set; } = null;

        protected override void Awake()
        {
            base.Awake();

            // コンポーネント生成
            Shooter = GetComponentInChildren<Shooter>(true);
        }

        public override void OnDataSet()
        {
            base.OnDataSet();

            // 弾のオブジェクトプールを消す
            foreach (var weapon in Weapons)
            {
                weapon.DeleteBulletPool();
            }
            Weapons.Clear();

            foreach (var id in Data.InitialWeaponIds)
            {
                // 武器データID
                WeaponData data = MasterData.Weapons.GetValueOrDefault(id);

                // 武器を作る
                Weapon weapon = new()
                {
                    Level = MasterData.GameSettings.MinLevel,
                    Data = data
                };

                // 弾のオブジェクトプールを作る
                weapon.CreateBulletPool();

                // 武器リストに武器を加える
                Weapons.Add(weapon);
            }


            // 味方ユニットのオブジェクトプールを消す
            foreach (var accessory in Accessories)
            {
                accessory.DeleteAllyPool();
            }
            Accessories.Clear();

            foreach (var id in Data.InitialAccessoryIds)
            {
                // 装飾品データID
                AccessoryData data = MasterData.Accessories.GetValueOrDefault(id);

                // 装飾品を作る
                Accessory accessory = new()
                {
                    Level = MasterData.GameSettings.MinLevel,
                    Data = data
                };

                // 味方ユニットのオブジェクトプールを作る
                accessory.CreateAllyPool(Units);

                // 装飾品リストに加える
                Accessories.Add(accessory);
            }

            // 装飾品更新
            UpdateSummonses();
        }

        public override void RefreshStatus()
        {
            base.RefreshStatus();

            // データに基づく基本値を代入する
            int attack = Data.Attack;
            int moveSpeed = Data.MoveSpeed;
            int rechargeSpeed = Data.RechargeSpeed;

            // 装飾品による補正を掛ける
            foreach (var accessory in Accessories)
            {
                AccessoryLevelData levelData = accessory.GetCurrentLevelData();
                attack += levelData.Attack;
                moveSpeed += levelData.MoveSpeed;
                rechargeSpeed += levelData.RechargeSpeed;
            }

            // ステータスに反映する
            Attack = attack;
            MoveSpeed = moveSpeed;
            RechargeSpeed = rechargeSpeed;
        }

        /// <summary>
        /// 弾の発射を中止する
        /// </summary>
        public void CancelFire()
        {
            foreach (var weapon in Weapons)
            {
                weapon.FireCancellationTokenSource?.Cancel();
            }
        }

        private void OnDestroy()
        {
            // 弾の発射を中止する
            CancelFire();
        }

        /// <summary>
        /// 武器の更新処理を行う
        /// </summary>
        public virtual void UpdateWeapons()
        {
            foreach (var weapon in Weapons)
            {
                weapon.Update<Ally<TAlly>, TAlly>(this, Units);
            }
        }

        /// <summary>
        /// 召喚の更新処理を行う
        /// </summary>
        protected void UpdateSummonses()
        {
            foreach (var accessory in Accessories)
            {
                accessory.UpdateSummons(transform.position);
            }
        }
    }
}
