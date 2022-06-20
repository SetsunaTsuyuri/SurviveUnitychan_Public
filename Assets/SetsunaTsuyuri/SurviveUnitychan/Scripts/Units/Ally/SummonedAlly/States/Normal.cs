using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    public partial class SummonedAlly
    {
        /// <summary>
        /// 通常ステート
        /// </summary>
        private class Normal : FiniteStateMachine<SummonedAlly>.State
        {
            public override void Update(SummonedAlly context)
            {
                // 武器更新
                context.UpdateWeapons();
            }
        }
    }
}
