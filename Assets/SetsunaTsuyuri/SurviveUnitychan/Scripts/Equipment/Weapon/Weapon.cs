using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 射撃武器
    /// </summary>
    public class Weapon : Equipment<WeaponData, WeaponLevelData>
    {
        /// <summary>
        /// 弾のオブジェクトプール
        /// </summary>
        public Bullet[] BulletPool { get; private set; } = { };

        /// <summary>
        /// 使用までの時間カウント
        /// </summary>
        float _rechargeTime = 0.0f;

        /// <summary>
        /// 発射のキャンセレーショントークンソース
        /// </summary>
        public CancellationTokenSource FireCancellationTokenSource { get; private set; } = null;

        /// <summary>
        /// 弾のオブジェクトプールを作る
        /// </summary>
        public void CreateBulletPool()
        {
            if (Data.NumberOfBulletPool <= 0)
            {
                return;
            }

            Bullet prefab = Resources.Load<Bullet>("Prefabs/Bullet");
            BulletPool = new Bullet[Data.NumberOfBulletPool];
            for (int i = 0; i < Data.NumberOfBulletPool; i++)
            {
                // プレハブインスタンス化
                Bullet instance = Object.Instantiate(prefab);

                // ステータス設定
                instance.MoveType = Data.MoveType;
                instance.Penetrates = Data.Penetrates;

                // モデルインスタンス化
                Object.Instantiate(Data.BulletModel, instance.transform);

                // セットアップ
                instance.SetUp();

                // 非アクティブ化
                instance.gameObject.SetActive(false);

                // プールに追加する
                BulletPool[i] = instance;
            }
        }

        /// <summary>
        /// 弾のオブジェクトプールを消す
        /// </summary>
        /// <param name="shooter">発射装置</param>
        public void DeleteBulletPool()
        {
            foreach (var bullet in BulletPool)
            {
                Object.Destroy(bullet);
            }
            BulletPool = null;
        }

        /// <summary>
        /// 更新する
        /// </summary>
        /// <typeparam name="TOwner">所有者</typeparam>
        /// <typeparam name="TAlly">味方</typeparam>
        /// <param name="owner">所有者</param>
        /// <param name="units">ユニットの管理者</param>
        public void Update<TOwner, TAlly>(TOwner owner, UnitsManager units)
            where TOwner : Ally<TAlly>
            where TAlly : Unit<TAlly, AllyData>
        {
            if (CanShot())
            {
                // 発射装置に弾を発射させる
                FireCancellationTokenSource = new();
                CancellationToken token = FireCancellationTokenSource.Token;
                owner.Shooter.FireAsync(this, owner.Attack, units, token).Forget();

                // 再使用までの時間をリセットする
                _rechargeTime = 0.0f;
            }
            else
            {
                // 再使用までの時間を増やす
                float time = Time.deltaTime * (owner.RechargeSpeed / 100.0f);
                _rechargeTime += time;
            }
        }

        /// <summary>
        /// 発射可能である
        /// </summary>
        /// <returns></returns>
        private bool CanShot()
        {
            float recharge = GetCurrentLevelData().Recharge;
            return _rechargeTime >= recharge;
        }
    }
}
