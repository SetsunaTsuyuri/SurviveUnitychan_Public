using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UniRx;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// ユニットの管理者
    /// </summary>
    public class UnitsManager : MonoBehaviour
    {
        /// <summary>
        /// プレイヤー
        /// </summary>
        [field: SerializeField]
        public Player Player { get; private set; } = null;

        /// <summary>
        /// 味方ユニット
        /// </summary>
        public List<SummonedAlly> Allies { get; private set; } = new();

        /// <summary>
        /// 敵ユニット
        /// </summary>
        public Enemy[] Enemies { get; private set; } = null;

        /// <summary>
        /// 敵のプレファブ
        /// </summary>
        [SerializeField]
        Enemy _enemyPrefab = null;

        /// <summary>
        /// 味方のデータ集
        /// </summary>
        [SerializeField]
        AllyDataCollection _allyDataCollection = null;

        /// <summary>
        /// 敵のデータ集
        /// </summary>
        [SerializeField]
        EnemyDataCollection _enemyDataCollection = null;

        /// <summary>
        /// 目的地の更新間隔
        /// </summary>
        [SerializeField]
        float _destinationsUpdateInterval = 0.5f;

        /// <summary>
        /// 目的地の更新カウント
        /// </summary>
        float _destinationsUpdateCount = 0.0f;

        /// <summary>
        /// 敵が出現する間隔
        /// </summary>
        [SerializeField]
        float _enemiesSpawningInterval = 0.5f;

        /// <summary>
        /// 敵出現の更新カウント
        /// </summary>
        float _enemiesSpawningCount = 0.0f;

        /// <summary>
        /// 敵が出現する位置の数
        /// </summary>
        [SerializeField]
        int _enemiesSpawningPoints = 32;

        /// <summary>
        /// 敵が出現する地点の円の数
        /// </summary>
        [SerializeField]
        int _enemiesSpawningpointsCircles = 3;

        /// <summary>
        /// 最初の敵出現地点の円とプレイヤーとの距離
        /// </summary>
        [SerializeField]
        float _firstCircleDistance = 30.0f;

        /// <summary>
        /// 2つ目以降の敵出現の円とその前の円との距離
        /// </summary>
        [SerializeField]
        float _secondAndSubsequentCircleDistance = 10.0f;

        /// <summary>
        /// 敵の出現地点から最寄りのNavMeshを探す距離
        /// </summary>
        [SerializeField]
        float _samplePositionMaxDistance = 0.01f;

        /// <summary>
        /// 敵の最大出現数(現在値)
        /// </summary>
        int _currentMaxEnemies = 0;

        /// <summary>
        /// 敵の増援時間カウント
        /// </summary>
        float _enemyReinforcementTimeCount = 0.0f;

        /// <summary>
        /// セットアップする
        /// </summary>
        /// <param name="inGame">インゲームの管理者</param>
        public void SetUp(InGameManager inGame)
        {
            // 敵の最大出現数設定
            _currentMaxEnemies = inGame.StageData.InitialEnemies;

            // プレイヤーのユニット管理者設定
            Player.Units = this;

            // プレイヤーのデータ設定
            Player.Data = _allyDataCollection.GetValueOrDefault(0);

            // プレイヤーのレイヤー設定
            Player.SetLayer(LayerName.Ally);

            // プレイヤー初期化
            Player.Initialize();

            // 敵の出現地点生成
            Player.CreateEnemySpawnedPoints(_enemiesSpawningPoints, _enemiesSpawningpointsCircles, _firstCircleDistance, _secondAndSubsequentCircleDistance);

            // 敵ユニット生成
            CreateEnemies(inGame);
        }

        /// <summary>
        /// 敵ユニットを生成する
        /// </summary>
        /// <param name="inGame">インゲームの管理者</param>
        private void CreateEnemies(InGameManager inGame)
        {
            // ステージデータ
            StageData stageData = inGame.StageData;

            // 配列の長さを敵の最大出現数と同じにする
            Enemies = new Enemy[stageData.MaxEnemies];

            for (int i = 0; i < Enemies.Length; i++)
            {
                // インスタンス化する
                Enemy instance = Instantiate(_enemyPrefab, transform);

                // データを設定する
                int dataId = stageData.EnemyIds[i % stageData.EnemyIds.Length];
                instance.Data = _enemyDataCollection.GetValueOrDefault(dataId);

                // レイヤーを変更する
                instance.SetLayer(LayerName.Enemy);

                // 非アクティブにする
                instance.gameObject.SetActive(false);

                // 配列に代入する
                Enemies[i] = instance;
            }
        }

        /// <summary>
        /// 各ユニットの更新処理を行う
        /// </summary>
        public void UpdateUnits()
        {
            Player.State.Update();
            UpdateAllies();

            UpdateEnemiesSpawning();
            UpdateUnitsDestination();
        }

        /// <summary>
        /// 味方ユニットの更新処理を行う
        /// </summary>
        private void UpdateAllies()
        {
            // アクティブな味方の配列
            SummonedAlly[] activeSummonedAllies = Allies
                 .Where(x => x.isActiveAndEnabled)
                 .ToArray();

            // 更新する
            foreach (var ally in activeSummonedAllies)
            {
                ally.State.Update();
            }
        }

        /// <summary>
        /// 一定間隔で敵を出現させる
        /// </summary>
        private void UpdateEnemiesSpawning()
        {
            if (_enemiesSpawningCount > _enemiesSpawningInterval)
            {
                _enemiesSpawningCount = 0.0f;

                SpawnEnemies();
            }
            else
            {
                _enemiesSpawningCount += Time.deltaTime;
            }
        }

        /// <summary>
        /// 敵を出現させる
        /// </summary>
        private void SpawnEnemies()
        {
            // 非アクティブの敵配列
            Enemy[] nonActiveEnemies = Enemies
                .Where(x => !x.isActiveAndEnabled)
                .ToArray();

            // 非アクティブな敵がいない場合は中止する
            if (nonActiveEnemies.Length == 0)
            {
                return;
            }

            // 出現可能な地点を探す
            List<Vector3> positions = new();
            for (int i = 0; i < Player.EnemySpawnedPoints.Length; i++)
            {
                Vector3 position = Player.EnemySpawnedPoints[i].position;
                if (NavMesh.SamplePosition(position, out NavMeshHit hit, _samplePositionMaxDistance, NavMesh.AllAreas))
                {
                    positions.Add(hit.position);
                }
            }

            // 出現可能な地点が1つ以上あれば、それらの内いずれかの地点に敵を出現させる
            if (positions.Any())
            {
                int activeEnemies = Enemies.Length - nonActiveEnemies.Length;
                int spawnableEnemies = _currentMaxEnemies - activeEnemies;

                for (int i = 0; i < spawnableEnemies; i++)
                {
                    int index = Random.Range(0, positions.Count - 1);
                    nonActiveEnemies[i].Spawn(positions[index]);
                }
            }
        }

        /// <summary>
        /// 一定間隔でユニットの目的地を更新する
        /// </summary>
        private void UpdateUnitsDestination()
        {
            if (_destinationsUpdateCount > _destinationsUpdateInterval)
            {
                _destinationsUpdateCount = 0.0f;

                UpdateAlliesDestination();
                UpdateEnemiesDestination();
            }
            else
            {
                _destinationsUpdateCount += Time.deltaTime;
            }
        }

        /// <summary>
        /// 味方の目的地を更新する
        /// </summary>
        private void UpdateAlliesDestination()
        {
            // アクティブな味方の配列
            SummonedAlly[] activeSummonedAllies = Allies
                .Where(x => x.isActiveAndEnabled)
                .ToArray();

            foreach (var ally in activeSummonedAllies)
            {
                // 追跡対象が存在しないする場合、新たな追跡対象を探す
                if (!ally.HasLivingTarget())
                {
                    // ランダムに選ぶ
                    ally.Target = GetRandomLivingEnemy();   
                }

                // 目的地を更新する
                ally.UpdateDestination();
            }
        }

        /// <summary>
        /// 生きた敵ユニットをランダムに取得する
        /// </summary>
        /// <returns></returns>
        public Enemy GetRandomLivingEnemy()
        {
            return Enemies
                .Where(x => isActiveAndEnabled)
                .Shuffle()
                .FirstOrDefault();
        }

        /// <summary>
        /// 最も近い敵ユニットのトランスフォームを探す
        /// </summary>
        /// <param name="subject">主体</param>
        /// <returns></returns>
        public Transform SerchForTheNearestEnemyTrandform(Transform subject)
        {
            return Enemies
               .Where(x => x.isActiveAndEnabled)
               .Select(x => x.transform)
               .OrderBy(x => (subject.position - x.position).sqrMagnitude)
               .FirstOrDefault();
        }

        /// <summary>
        /// 敵の目的地を更新する
        /// </summary>
        private void UpdateEnemiesDestination()
        {
            // アクティブな敵の配列
            Enemy[] activeEnemies = Enemies
                .Where(x => x.isActiveAndEnabled)
                .ToArray();

            // プレイヤーのいる位置を目的地とする
            foreach (var enemy in activeEnemies)
            {
                enemy.UpdateDestination(Player.transform.position);
            }
        }

        /// <summary>
        /// 敵の最大出現数を更新する
        /// </summary>
        /// <param name="stageData">ステージデータ</param>
        public void UpdateEnemyReinforcements(StageData stageData)
        {
            // 一定間隔で更新する
            _enemyReinforcementTimeCount += Time.deltaTime;
            if (_enemyReinforcementTimeCount >= stageData.EnemyReinforcementsInterval)
            {
                _enemyReinforcementTimeCount = 0.0f;

                // 敵の最大数を増やす
                _currentMaxEnemies += stageData.EnemyReinforcements;
            }
        }
    }
}
