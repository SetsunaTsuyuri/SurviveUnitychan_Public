using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 召喚された味方ユニット
    /// </summary>
    public partial class SummonedAlly : Ally<SummonedAlly>
    {
        /// <summary>
        /// 追跡対象の敵ユニット
        /// </summary>
        public Enemy Target { get; set; } = null;

        protected override void Awake()
        {
            base.Awake();

            // ステートマシンを生成する
            State = new FiniteStateMachine<SummonedAlly>(this);
            State.Add<Normal>();
        }

        public override void Initialize()
        {
            base.Initialize();

            // 通常ステートへ移行する
            State.Change<Normal>();
        }

        /// <summary>
        /// 目的地を更新する
        /// </summary>
        public void UpdateDestination()
        {
            // 追跡対象のいる位置を目的地とする
            if (Target)
            {
                _agent.destination = Target.transform.position;
            }
        }

        /// <summary>
        /// 生きた追跡対象が存在する
        /// </summary>
        /// <returns></returns>
        public bool HasLivingTarget()
        {
            return Target && Target.IsLiving();
        }
    }
}
