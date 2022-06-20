using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// ユニット
    /// </summary>
    /// <typeparam name="TUnit">ユニット</typeparam>
    /// <typeparam name="TData">データ</typeparam>
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class Unit<TUnit, TData> : MonoBehaviour, IInitializable, IPausable
        where TUnit : Unit<TUnit, TData>
        where TData : UnitData
    {
        /// <summary>
        /// 最大生命力
        /// </summary>
        int _maxLife = 0;

        /// <summary>
        /// 最大生命力
        /// </summary>
        public virtual int MaxLife
        {
            get => _maxLife;
            set
            {
                int max = MasterData.GameSettings.UnitLifeLimit;
                _maxLife = Mathf.Clamp(value, 0, max);
            }
        }

        /// <summary>
        /// 生命力
        /// </summary>
        int _life = 0;

        /// <summary>
        /// 生命力
        /// </summary>
        public virtual int Life
        {
            get => _life;
            set => _life = Mathf.Clamp(value, 0, MaxLife);
        }

        /// <summary>
        /// 移動速度
        /// </summary>
        float _moveSpeed = 5.0f;

        /// <summary>
        /// 移動速度
        /// </summary>
        public float MoveSpeed
        {
            get => _moveSpeed;
            set
            {
                _moveSpeed = value;
                OnMoveSpeedSet();
            }
        }

        /// <summary>
        /// データ
        /// </summary>
        protected TData _data = null;

        /// <summary>
        /// データ
        /// </summary>
        public TData Data
        {
            get => _data;
            set
            {
                _data = value;
                OnDataSet();
            }
        }

        /// <summary>
        /// 点滅時間カウント
        /// </summary>
        float _blinkingTimeCount = 0.0f;

        /// <summary>
        /// 点滅カウント
        /// </summary>
        protected float BlinkingTimeCount
        {
            get => _blinkingTimeCount;
            set
            {
                // ゼロ未満にならない
                _blinkingTimeCount = Mathf.Max(value, 0.0f);
            }
        }

        /// <summary>
        /// 気絶時間
        /// </summary>
        float _remainingStunnedTime = 0.0f;

        /// <summary>
        /// 気絶時間
        /// </summary>
        protected float RemainningStunnedTime
        {
            get => _remainingStunnedTime;
            set
            {
                // ゼロ未満にならない
                _remainingStunnedTime = Mathf.Max(value, 0.0f);
            }
        }

        /// <summary>
        /// 無敵時間
        /// </summary>
        float _remainningInvincibleTime = 0.0f;

        /// <summary>
        /// 無敵時間
        /// </summary>
        protected float RemainningInvincibleTime
        {
            get => _remainningInvincibleTime;
            set
            {
                // ゼロ未満にならない
                _remainningInvincibleTime = Mathf.Max(value, 0.0f);
            }
        }

        /// <summary>
        /// ステートマシン
        /// </summary>
        public FiniteStateMachine<TUnit> State { get; protected set; } = null;

        /// <summary>
        /// モデル
        /// </summary>
        protected GameObject _model = null;

        /// <summary>
        /// レンダラー配列
        /// </summary>
        Renderer[] _renderers = { };

        /// <summary>
        /// レンダラーを有効にする
        /// </summary>
        protected bool _renderersEnabled = true;

        /// <summary>
        /// ユニットアニメーション
        /// </summary>
        public UnitAnimation UnitAnimation { get; protected set; } = null;

        /// <summary>
        /// ナビメッシュエージェント
        /// </summary>
        protected NavMeshAgent _agent = null;

        /// <summary>
        ///障害物回避の初期品質
        /// </summary>
        protected ObstacleAvoidanceType _initialQuality = ObstacleAvoidanceType.NoObstacleAvoidance;

        /// <summary>
        /// 一時停止直前の速度
        /// </summary>
        Vector3? _velocityBeforePause = null;

        /// <summary>
        /// 一時停止直前の加速度
        /// </summary>
        float? _accelerationBeforePause = null;

        protected virtual void Awake()
        {
            // コンポーネント取得
            _agent = GetComponent<NavMeshAgent>();

            // 障害物回避の初期品質をキャッシュする
            _initialQuality = _agent.obstacleAvoidanceType;

            // 一時停止時の処理を購読する
            PauseManager.OnPause
                .TakeUntilDestroy(this)
                .Subscribe(_ => Pause());

            // 再開時の処理を購読する
            PauseManager.OnResume
                .TakeUntilDestroy(this)
                .Subscribe((_) => Resume());
        }

        public virtual void Initialize()
        {
            // ステータスを更新する
            RefreshStatus();

            // 生命力を回復する
            Life = MaxLife;
        }

        public virtual void Pause()
        {
            // 現在の速度と加速度を保存する
            _velocityBeforePause = _agent.velocity;
            _accelerationBeforePause = _agent.acceleration;

            // 移動を止める
            _agent.velocity = Vector3.zero;
            _agent.acceleration = 0.0f;
            _agent.speed = 0.0f;

            // アニメーションを止める
            if (UnitAnimation)
            {
                UnitAnimation.Pause();
            }
        }

        public virtual void Resume()
        {
            // 保存した速度と加速度に戻す
            if (_velocityBeforePause.HasValue)
            {
                _agent.velocity = _velocityBeforePause.Value;
            }
            if (_accelerationBeforePause.HasValue)
            {
                _agent.acceleration = _accelerationBeforePause.Value;
            }

            // 移動を再開する
            _agent.speed = CalculateActualMoveSpeed();

            // アニメーションを再開する
            if (UnitAnimation)
            {
                UnitAnimation.Resume();
            }
        }

        /// <summary>
        /// アニメーションIDを設定する
        /// </summary>
        /// <param name="id">ID</param>
        public void SetAnimationId(UnitAnimationId id)
        {
            if (UnitAnimation)
            {
                UnitAnimation.Id = id;
            }
        }

        /// <summary>
        /// ステータスを更新する
        /// </summary>
        public virtual void RefreshStatus()
        {
            MaxLife = Data.MaxLife;
        }

        /// <summary>
        /// ダメージを受ける
        /// </summary>
        /// <param name="value">ダメージ値</param>
        protected virtual void TakeDamage(int value)
        {
            // ダメージを受けないなら中止する
            if (IsInvincible() || value <= 0)
            {
                return;
            }

            // 生命力を減らす
            Life -= value;

            // 生命力がゼロになった場合
            if (Life == 0)
            {
                // 死亡する
                BeKnockedOut();
            }
            else if (value > 0)
            {
                // ダメージを受けたときの処理を行う
                OnDamaged();
            }
        }

        /// <summary>
        /// 無敵状態である
        /// </summary>
        /// <returns></returns>
        private bool IsInvincible()
        {
            return _remainningInvincibleTime > 0.0f;
        }

        /// <summary>
        /// 一定間隔で点滅する
        /// </summary>
        protected void BlinkAtRegularIntervals()
        {
            // 点滅時間カウントを減らす
            BlinkingTimeCount -= Time.deltaTime;

            // カウントがゼロになった場合
            if (BlinkingTimeCount == 0.0f)
            {
                // レンダラーの有効、無効を切り替える
                _renderersEnabled = !_renderersEnabled;
                SetRenderersEnabled(_renderersEnabled);

                // 点滅時間カウントをリセットする
                BlinkingTimeCount = MasterData.GameSettings.UnitBlinkingInterval;
            }
        }

        /// <summary>
        /// レンダラーのenabledを設定する
        /// </summary>
        /// <param name="value">値</param>
        protected void SetRenderersEnabled(bool value)
        {
            foreach (var renderer in _renderers)
            {
                renderer.enabled = value;
            }
        }

        /// <summary>
        /// 倒れる
        /// </summary>
        public virtual void BeKnockedOut() { }

        /// <summary>
        /// ダメージを受けたときの処理
        /// </summary>
        protected virtual void OnDamaged() { }

        /// <summary>
        /// データが設定されたときの処理
        /// </summary>
        public virtual void OnDataSet()
        {
            // 既にモデルがキャッシュされている場合、そのモデルを破壊する
            if (_model)
            {
                Destroy(_model);
            }

            // データにモデルが存在する場合
            if (Data.Model)
            {
                // 新たにモデルを生成し、それをキャッシュする
                _model = Instantiate(Data.Model, transform);

                // レンダラーをキャッシュする
                _renderers = _model.GetComponentsInChildren<Renderer>();

                // アニメーションを取得し、それを初期化する
                UnitAnimation = GetComponentInChildren<UnitAnimation>(true);
                if (UnitAnimation)
                {
                    UnitAnimation.Initialize();
                }
            }
        }

        /// <summary>
        /// 素早さが設定されたときの処理
        /// </summary>
        protected void OnMoveSpeedSet()
        {
            if (!_agent)
            {
                return;
            }

            // ナビメッシュエージェントの最大速度を変える
            _agent.speed = CalculateActualMoveSpeed();
        }

        /// <summary>
        /// 実際の移動速度を計算する
        /// </summary>
        /// <returns></returns>
        public float CalculateActualMoveSpeed()
        {
            float correction = (float)MoveSpeed / 100.0f;
            float speed = MasterData.GameSettings.UnitMoveSpeed * correction;
            return speed;
        }

        /// <summary>
        /// 出現する
        /// </summary>
        /// <param name="position">出現する位置</param>
        public void Spawn(Vector3 position)
        {
            // 初期化
            Initialize();

            // 指定位置に移動
            Teleport(position);

            // アクティブ化
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 瞬間移動する
        /// </summary>
        /// <param name="position">移動先</param>
        public void Teleport(Vector3 position)
        {
            transform.position = position;
            _agent.Warp(position);
        }

        /// <summary>
        /// 目的地を更新する
        /// </summary>
        /// <param name="position">位置</param>
        public void UpdateDestination(Vector3 position)
        {
            _agent.destination = position;
        }

        /// <summary>
        /// レイヤーを設定する
        /// </summary>
        /// <param name="layer">レイヤー</param>
        public void SetLayer(int layer)
        {
            Transform[] children = GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            {
                child.gameObject.layer = layer;
            }
        }

        /// <summary>
        /// 生きている
        /// </summary>
        /// <returns></returns>
        public bool IsLiving()
        {
            return Life > 0;
        }
    }
}
