using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using Cysharp.Threading.Tasks;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// インゲームの管理者
    /// </summary>
    public partial class InGameManager : MonoBehaviour
    {
        /// <summary>
        /// インゲームUIの管理者
        /// </summary>
        [SerializeField]
        InGameUIManager _ui = null;

        /// <summary>
        /// 残り時間
        /// </summary>
        readonly ReactiveProperty<float> _remainningTime = new();

        /// <summary>
        /// 残り時間
        /// </summary>
        public float RemainningTime
        {
            get => _remainningTime.Value;
            private set
            {
                // ゼロ未満にならない
                _remainningTime.Value = Mathf.Max(value, 0.0f);
            }
        }

        /// <summary>
        /// 残り時間が変わったとき
        /// </summary>
        public IReadOnlyReactiveProperty<float> RemainningTimeChanged
        {
            get => _remainningTime;
        }

        /// <summary>
        /// 敵撃破数
        /// </summary>
        readonly ReactiveProperty<int> _defeatedEnemiesCount = new();

        /// <summary>
        /// 敵撃破数
        /// </summary>
        public int DefeatedEnemiesCount
        {
            get => _defeatedEnemiesCount.Value;
            private set => _defeatedEnemiesCount.Value = value;
        }

        /// <summary>
        /// 撃破数が変わったとき
        /// </summary>
        public IReadOnlyReactiveProperty<int> DefeatedEnemiesCountChanged
        {
            get => _defeatedEnemiesCount;
        }

        /// <summary>
        /// ユニットの管理者
        /// </summary>
        public UnitsManager Units { get; private set; } = null;

        /// <summary>
        /// アイテムの管理者
        /// </summary>
        ItemsManager _items = null;

        /// <summary>
        /// ステージのデータ
        /// </summary>
        public StageData StageData { get; private set; } = null;

        /// <summary>
        /// ステートマシン
        /// </summary>
        FiniteStateMachine<InGameManager> _state = null;

        private void Awake()
        {
            // コンポーネント取得
            Units = GetComponentInChildren<UnitsManager>(true);
            _items = GetComponentInChildren<ItemsManager>(true);

            // ステートマシン生成
            _state = new(this);
            _state.Add<BattleStart>();
            _state.Add<InBattle>();
            _state.Add<BattleEnd>();
        }

        private void Start()
        {
            // ステージデータ取得
            StageData = MasterData.Stages.GetValueOrDefault(0);

            // 戦闘開始
            _state.Change<BattleStart>();
        }

        private void Update()
        {
            // 一時停止中なら更新しない
            if (PauseManager.IsPausing)
            {
                return;
            }

            // ステートに応じて更新処理を行う
            _state.Update();
        }

        /// <summary>
        /// プレイヤーがレベルアップしたときの処理
        /// </summary>
        /// <param name="levelUps">レベルの上昇値</param>
        private void OnPlayerLevelUp(int levelUps)
        {
            // ポーズ開始
            PauseManager.Pause();

            // レベルアップSEを再生する
            AudioManager.PlaySE("レベルアップ");

            // 経験値ゲージを満タンにする
            _ui.Experience.FillUp();

            CancellationToken token = this.GetCancellationTokenOnDestroy();
            OnPlayerLevelUpAsync(levelUps, token).Forget();
        }

        /// <summary>
        /// プレイヤーがレベルアップしたときの非同期処理
        /// </summary>
        /// <param name="levelUps">レベルの上昇値</param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async UniTask OnPlayerLevelUpAsync(int levelUps, CancellationToken token)
        {
            // プレイヤー
            Player player = Units.Player;

            // 装備選択UI
            EquipmentSelectionButtonsManager equipmentSelection = _ui.EquipmentSelection;

            // レベルアップした回数分繰り返す
            for (int i = 0; i < levelUps; i++)
            {
                // 更新
                equipmentSelection.UpdateButtons(player);

                // 選択
                equipmentSelection.Select();

                // ボタン押下待機
                await equipmentSelection.WaitForAnyButtonPressed(token);
            }

            // 装備選択UI非表示
            equipmentSelection.Hide();

            // 経験値バーの表示更新
            _ui.Experience.UpdateDisplay(player);

            // ポーズ解除
            PauseManager.Resume();
        }

        /// <summary>
        /// リザルトシーンがロードされたときの処理
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="loadSceneMode"></param>
        private void OnResultSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            // リザルトシーンの管理者
            ResultManager result = FindObjectOfType<ResultManager>();

            // プレイヤーの生存時間
            float survivalTime = StageData.TimeToClear - RemainningTime;

            // 生存時間を設定する
            result.SurvivalTime = survivalTime;

            // 敵撃破数を設定する
            result.DefeatedEnemiesCount = DefeatedEnemiesCount;

            // プレイヤー
            Player player = FindObjectOfType<Player>();

            // シーン変更後に破壊されるようにする
            SceneManager.MoveGameObjectToScene(player.gameObject, SceneManager.GetActiveScene());

            // 通常ステートに移行させる
            player.State.Change<Player.Normal>();

            // 中心地点に移動させる
            player.Teleport(Vector3.zero);

            // 180度回転状態にする
            player.transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);

            // プレイヤーを登録する
            result.Player = player;

            // この関数の登録を解除する
            SceneManager.sceneLoaded -= OnResultSceneLoaded;
        }
    }
}
