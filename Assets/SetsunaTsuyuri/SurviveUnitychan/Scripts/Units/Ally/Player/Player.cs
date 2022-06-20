using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// プレイヤー
    /// </summary>
    public partial class Player : Ally<Player>, IEnemyHit, IItemHit
    {
        public override int Life
        {
            get => base.Life;
            set
            {
                base.Life = value;
                _lifeSet.OnNext(base.Life);
            }
        }

        public override int MaxLife
        {
            get => base.MaxLife;
            set
            {
                base.MaxLife = value;
                _maxLifeSet.OnNext(base.MaxLife);
            }
        }

        /// <summary>
        /// 生命力が設定されたとき
        /// </summary>
        readonly Subject<int> _lifeSet = new();

        /// <summary>
        /// 生命力が設定されたとき
        /// </summary>
        public IObservable<int> LifeSet
        {
            get => _lifeSet;
        }

        /// <summary>
        /// 最大生命力が設定されたとき
        /// </summary>
        readonly Subject<int> _maxLifeSet = new();

        /// <summary>
        /// 最大生命力が設定されたとき
        /// </summary>
        public IObservable<int> MaxLifeSet
        {
            get => _maxLifeSet;
        }

        /// <summary>
        /// レベルが上がったとき
        /// </summary>
        readonly Subject<int> _levelUpped = new();

        /// <summary>
        /// レベルが上がったとき
        /// </summary>
        public IObservable<int> LevelUpped
        {
            get => _levelUpped;
        }

        /// <summary>
        /// 武器が更新されたとき
        /// </summary>
        readonly Subject<Weapon[]> _weaponsUpdated = new();

        /// <summary>
        /// 武器が更新されたとき
        /// </summary>
        public IObservable<Weapon[]> WeaponsUpdated
        {
            get => _weaponsUpdated;
        }

        /// <summary>
        /// 装飾品が更新されたとき
        /// </summary>
        readonly Subject<Accessory[]> _accessoriesUpdated = new();

        /// <summary>
        /// 装飾品が更新されたとき
        /// </summary>
        public IObservable<Accessory[]> AccessoriesUpdated
        {
            get => _accessoriesUpdated;
        }

        /// <summary>
        /// 敵の出現地点の親オブジェクト
        /// </summary>
        [SerializeField]
        Transform _enemySpawnedPositionsParent = null;

        /// <summary>
        /// 回転の速さ
        /// </summary>
        [SerializeField]
        float _rotationSpeed = 10.0f;

        /// <summary>
        /// 敵の出現地点
        /// </summary>
        public Transform[] EnemySpawnedPoints { get; private set; } = { };

        /// <summary>
        /// レベル
        /// </summary>
        readonly ReactiveProperty<int> _level = new();

        /// <summary>
        /// レベル
        /// </summary>
        public int Level
        {
            get => _level.Value;
            set
            {
                int min = MasterData.GameSettings.MinLevel;
                int max = MasterData.GameSettings.MaxLevel;
                _level.Value = Mathf.Clamp(value, min, max);
            }
        }

        /// <summary>
        /// レベルが変わったとき
        /// </summary>
        public IReadOnlyReactiveProperty<int> LevelChanged
        {
            get => _level;
        }

        /// <summary>
        /// 経験値
        /// </summary>
        int _experience = 0;

        /// <summary>
        /// 経験値
        /// </summary>
        public int Experience
        {
            get => _experience;
            set
            {
                _experience = value;

                // 経験値の設定を通知する
                _experienceSet.OnNext(this);

                // レベルを更新する
                UpdateLevel();
            }
        }

        /// <summary>
        /// 経験値が設定されたとき
        /// </summary>
        readonly Subject<Player> _experienceSet = new();

        /// <summary>
        /// 経験値が設定されたとき
        /// </summary>
        public IObservable<Player> ExperienceSet
        {
            get => _experienceSet;
        }

        protected override void Awake()
        {
            base.Awake();

            // ステートマシンを生成する
            State = new FiniteStateMachine<Player>(this);
            State.Add<Normal>();
            State.Add<Damaged>();
            State.Add<InvincibleMove>();
            State.Add<KnockedOut>();
        }

        public override void Initialize()
        {
            base.Initialize();

            // レベルと経験値を初期化する
            Level = MasterData.GameSettings.MinLevel;
            Experience = 0;

            // 通常ステートに移行する
            State.Change<Normal>();
        }

        public void OnHit(Enemy enemy)
        {
            // 通常ステートでなければ中止する
            if (State.Current is not Normal)
            {
                return;
            }

            // ダメージを受ける
            TakeDamage(enemy.Data.TouchDamage);
        }

        public override void BeKnockedOut()
        {
            // 戦闘不能ステートに移行する
            State.Change<KnockedOut>();
        }

        protected override void OnDamaged()
        {
            // ダメージステートに移行する
            State.Change<Damaged>();
        }

        public void OnHit(Item item)
        {
            // SEを再生する
            AudioManager.PlaySE("アイテム入手");

            // 経験値を増やす
            Experience += item.Experience;

            // 非アクティブにする
            item.gameObject.SetActive(false);
        }

        /// <summary>
        /// 移動の更新処理を行う
        /// </summary>
        public void UpdateMove()
        {
            // 入力
            Vector3 input = InputUtility.GetMoveDirection();

            // 回転する
            Rotate(input);

            // 移動する
            Move(input);
        }

        /// <summary>
        /// 移動する
        /// </summary>
        private void Move(Vector3 direction)
        {
            if (direction == Vector3.zero)
            {
                // 待機をアニメ再生する
                SetAnimationId(UnitAnimationId.Idle);
                return;
            }

            // 移動アニメを再生する
            SetAnimationId(UnitAnimationId.Move);

            // 速度
            float speed = CalculateActualMoveSpeed();
            Vector3 velocity = speed * Time.deltaTime * direction;

            // 移動する
            _agent.Move(velocity);
        }

        /// <summary>
        /// 回転する
        /// </summary>
        /// <param name="direction">方向</param>
        private void Rotate(Vector3 direction)
        {
            if (direction == Vector3.zero)
            {
                return;
            }

            // 回転角度
            Quaternion rotation = Quaternion.LookRotation(direction);

            // 回転する
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                rotation,
                _rotationSpeed * Time.deltaTime);
        }

        /// <summary>
        /// レベルを更新する
        /// </summary>
        private void UpdateLevel()
        {
            // 直前のレベル
            int previousLevel = Level;

            // 経験値をレベルに換算する
            Level = ToLevel(Experience);

            // レベルの増加量
            int levelUps = Level - previousLevel;
            if (levelUps > 0)
            {
                // レベル上昇を通知する
                _levelUpped.OnNext(levelUps);
            }
        }

        /// <summary>
        /// 経験値からレベルを求める
        /// </summary>
        /// <param name="experience">経験値</param>
        /// <returns></returns>
        private int ToLevel(int experience)
        {
            int result = MasterData.GameSettings.MinLevel;

            // レベル計算
            while (experience >= ToMinExperience(result + 1))
            {
                result++;
            }

            return result;
        }

        /// <summary>
        /// レベルから最低経験値を求める
        /// </summary>
        /// <param name="level">レベル</param>
        /// <returns></returns>
        public int ToMinExperience(int level)
        {
            int result = 0;

            // プレイヤー設定
            GameSettings settings = MasterData.GameSettings;
            int minLevel = settings.MinLevel;
            int baseValue = settings.LevelUpExperience;
            float increase = settings.LevelUpExperienceRate;

            // 経験値計算
            for (int i = minLevel; i < level; i++)
            {
                float multiplier = 1.0f + (i - 1) * increase;
                int experience = Mathf.FloorToInt(baseValue * multiplier);
                result += experience;
            }

            return result;
        }

        /// <summary>
        /// 入手可能な装備品を作る
        /// </summary>
        /// <returns></returns>
        public IEquipment[] CreateAvailableEquipments()
        {
            // 装備品リスト
            List<IEquipment> availables = new();

            // 武器
            foreach (var id in Data.AvailableWeaponIds)
            {
                // レベル
                int level = MasterData.GameSettings.MinLevel;

                // 既にIDと同じ武器を得ている場合
                Weapon alreadyAcquired = Weapons.FirstOrDefault(x => x.Data.Id == id);
                if (alreadyAcquired != null)
                {
                    // 最大レベルは候補としない
                    if (alreadyAcquired.IsMaxLevel())
                    {
                        continue;
                    }

                    // 現在のレベル+1をレベルとする
                    level = alreadyAcquired.Level + 1;
                }

                // 武器データ
                WeaponData data = MasterData.Weapons.GetValueOrDefault(id);

                // 武器インスタンス
                Weapon weapon = new()
                {
                    Level = level,
                    Data = data
                };

                // リストに加える
                availables.Add(weapon);
            }

            // 装飾品
            foreach (var id in Data.AvailableAccesoryIds)
            {
                // レベル
                int level = MasterData.GameSettings.MinLevel;

                // 既にIDと同じ武器を得ている場合
                Accessory alreadyAcquired = Accessories.FirstOrDefault(x => x.Data.Id == id);
                if (alreadyAcquired != null)
                {
                    // 最大レベルは候補としない
                    if (alreadyAcquired.IsMaxLevel())
                    {
                        continue;
                    }

                    // 現在のレベル+1をレベルとする
                    level = alreadyAcquired.Level + 1;
                }


                // 装飾品データ
                AccessoryData data = MasterData.Accessories.GetValueOrDefault(id);

                // 装飾品インスタンス
                Accessory accessory = new()
                {
                    Level = level,
                    Data = data
                };

                // リストに加える
                availables.Add(accessory);
            }

            // リストを配列にして返す
            IEquipment[] result = availables.ToArray();
            return result;
        }

        /// <summary>
        /// 装備品を入手する
        /// </summary>
        /// <param name="equipment">装備品</param>
        public void ObtainEquipment(IEquipment equipment)
        {
            // 入手済みの装備品
            IEquipment[] acquiredEquipments = equipment switch
            {
                Weapon => Weapons.ToArray(),
                Accessory => Accessories.ToArray(),
                _ => null
            };

            // nullなら中止する
            if (acquiredEquipments is null)
            {
                return;
            }

            // 既に入手している場合
            IEquipment alreadyAcquried = acquiredEquipments
                .FirstOrDefault(x => x.GetEquipmentData().Id == equipment.GetEquipmentData().Id);
            if (alreadyAcquried is not null)
            {
                // レベルを1上げる
                alreadyAcquried.Level++;

                switch (alreadyAcquried)
                {
                    // 武器
                    case Weapon weapon:
                        _weaponsUpdated.OnNext(Weapons.ToArray());
                        break;

                    // 装飾品
                    case Accessory accessory:
                        UpdateSummonses();
                        RefreshStatus();
                        _accessoriesUpdated.OnNext(Accessories.ToArray());
                        break;
                }
            }
            else
            {
                switch (equipment)
                {
                    // 武器
                    case Weapon weapon:
                        weapon.CreateBulletPool();
                        Weapons.Add(weapon);
                        _weaponsUpdated.OnNext(Weapons.ToArray());
                        break;

                    // 装飾品
                    case Accessory accessory:
                        accessory.CreateAllyPool(Units);
                        Accessories.Add(accessory);
                        UpdateSummonses();
                        RefreshStatus();
                        _accessoriesUpdated.OnNext(Accessories.ToArray());
                        break;
                }
            }
        }

        /// <summary>
        /// 敵の出現地点を生成する
        /// </summary>
        /// <param name="points">1つの円を描く出現地点の数</param>
        /// <param name="circles">出現地点の円の数</param>
        /// <param name="firstDistance">最初の円の距離</param>
        /// <param name="secondAndSubsequentDistance">2つ目以降の円の距離</param>
        public void CreateEnemySpawnedPoints(int points, int circles, float firstDistance, float secondAndSubsequentDistance)
        {
            // 出現地点それぞれの角度の差
            float angleDifference = 360.0f / points;

            List<Transform> pointList = new();
            for (int i = 0; i < circles; i++)
            {
                // 出現地点とプレイヤーとの距離
                float distance = firstDistance + i * secondAndSubsequentDistance;

                for (int j = 0; j < points; j++)
                {
                    // 出現地点を求める
                    Vector3 position = Vector3.zero;
                    float angle = (90.0f - angleDifference * j) * Mathf.Deg2Rad;
                    position.x = distance * Mathf.Cos(angle);
                    position.z = distance * Mathf.Sin(angle);

                    // 出現地点を持つ空オブジェクトを作り、それを出現地点親オブジェクトの子とする
                    GameObject point = new($"EnemySpawnedPoint_{i}");
                    point.transform.SetParent(_enemySpawnedPositionsParent);
                    point.transform.localPosition = position;

                    // 出現地点のTransformをキャッシュする
                    pointList.Add(point.transform);
                }
            }
            EnemySpawnedPoints = pointList.ToArray();
        }

        /// <summary>
        /// 武器の最大数を取得する
        /// </summary>
        /// <returns></returns>
        public int GetMaxWeapons()
        {
            int result = Data.InitialWeaponIds
                .Union(Data.AvailableWeaponIds)
                .Count();

            return result;
        }

        /// <summary>
        /// 装飾品の最大数を取得する
        /// </summary>
        /// <returns></returns>
        public int GetMaxAccessories()
        {
            int result = Data.InitialAccessoryIds
                .Union(Data.AvailableAccesoryIds)
                .Count();

            return result;
        }
    }
}
