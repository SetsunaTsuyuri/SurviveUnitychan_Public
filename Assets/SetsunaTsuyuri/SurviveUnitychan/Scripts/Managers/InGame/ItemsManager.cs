using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// アイテムの管理者
    /// </summary>
    public class ItemsManager : MonoBehaviour
    {
        /// <summary>
        /// アイテムの最大数
        /// </summary>
        [SerializeField]
        int _maxItems = 0;

        /// <summary>
        /// アイテムがプレイヤーの追跡を開始する距離
        /// </summary>
        [SerializeField]
        float _trackingStartDistance = 0.0f;

        /// <summary>
        /// アイテムプレハブ
        /// </summary>
        [SerializeField]
        Item _itemPrefab = null;

        /// <summary>
        /// アイテム配列
        /// </summary>
        Item[] _items = null;

        /// <summary>
        /// 追跡フラグの更新間隔
        /// </summary>
        [SerializeField]
        float _trackingFlagsUpdateInterval = 0.5f;

        /// <summary>
        /// 追跡フラグの更新カウント
        /// </summary>
        float _trackingFlagsUpdateCount = 0.0f;

        /// <summary>
        /// セットアップする
        /// </summary>
        /// <param name="inGame">インゲームの管理者</param>
        public void SetUp(InGameManager inGame)
        {
            // アイテムを作る
            CreateItems(_maxItems);

            // 敵が倒されたとき、アイテムを落とすようにする
            foreach (var enemy in inGame.Units.Enemies)
            {
                enemy.Killed
                    .TakeUntilDestroy(this)
                    .Subscribe(OnEnemyKilled);
            }
        }

        /// <summary>
        /// 敵ユニットが倒されたときの処理
        /// </summary>
        /// <param name="enemy">敵ユニット</param>
        private void OnEnemyKilled(Enemy enemy)
        {
            // アイテムを出現させる
            Item nonActiveItem = _items.FirstOrDefault(x => !x.isActiveAndEnabled);
            if (nonActiveItem)
            {
                nonActiveItem.Spawn(enemy);
            }
            else // これ以上出現させられない場合は、最も古いアイテムを新しいアイテムとして出現させる
            {
                Item theOldestItem = _items
                    .Where(x => x.isActiveAndEnabled)
                    .OrderBy(x => x.SpawnedTime)
                    .FirstOrDefault();

                if (theOldestItem)
                {
                    theOldestItem.Spawn(enemy);
                }
            }
        }

        /// <summary>
        /// アイテムを作る
        /// </summary>
        /// <param name="number">アイテムの数</param>
        private void CreateItems(int number)
        {
            _items = new Item[number];
            for (int i = 0; i < number; i++)
            {
                // インスタンス化する
                Item instance = Instantiate(_itemPrefab, transform);

                // 非アクティブ化する
                instance.gameObject.SetActive(false);

                // 配列に代入する
                _items[i] = instance;
            }
        }

        /// <summary>
        /// アイテムの更新を行う
        /// </summary>
        /// <param name="playerPosition"></param>
        public void UpdateItems(Vector3 playerPosition)
        {
            // アイテムの追跡処理を行う
            UpdateItemsTracking(playerPosition);

            // 一定間隔でアイテムの追跡フラグを更新する
            if (_trackingFlagsUpdateCount >= _trackingFlagsUpdateInterval)
            {
                // 追跡フラグ更新
                UpdateItemsTrackingFlag(playerPosition);

                // カウントリセット
                _trackingFlagsUpdateCount = 0.0f;
            }
            else
            {
                // カウント進行
                _trackingFlagsUpdateCount += Time.deltaTime;
            }
        }

        /// <summary>
        /// アイテムの追跡フラグを更新する
        /// </summary>
        /// <param name="playerPosition">プレイヤーの位置</param>
        private void UpdateItemsTrackingFlag(Vector3 playerPosition)
        {
            // 追跡可能にすべきアイテム
            Item[] items = _items
                .Where(x => x.isActiveAndEnabled)
                .Where(x => !x.CanTrackTarget)
                .Where(x => IsInRangeToStartTracking(x.transform.position, playerPosition))
                .ToArray();

            foreach (var item in items)
            {
                item.CanTrackTarget = true;
            }
        }

        /// <summary>
        /// アイテムの追跡処理を更新する
        /// </summary>
        /// <param name="playerPosition">プレイヤーの位置</param>
        private void UpdateItemsTracking(Vector3 playerPosition)
        {
            // 追跡可能なアイテム
            Item[] items = _items
                .Where(x => x.isActiveAndEnabled)
                .Where(x => x.CanTrackTarget)
                .ToArray();

            foreach (var item in items)
            {
                item.Track(playerPosition);
            }
        }

        /// <summary>
        /// 追跡範囲内に存在する
        /// </summary>
        /// <param name="trackerPosition">追跡者の位置</param>
        /// <param name="targetPosition">追跡対象の位置</param>
        /// <returns></returns>
        private bool IsInRangeToStartTracking(Vector3 trackerPosition, Vector3 targetPosition)
        {
            float sqrMagnitude = (targetPosition - trackerPosition).sqrMagnitude;
            float sqrDistance = _trackingStartDistance * _trackingStartDistance;

            return sqrMagnitude <= sqrDistance;
        }
    }
}
