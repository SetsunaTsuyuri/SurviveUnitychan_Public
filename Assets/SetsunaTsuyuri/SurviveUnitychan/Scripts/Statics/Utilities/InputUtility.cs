using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 入力関連の便利な関数
    /// </summary>
    public static class InputUtility
    {
        /// <summary>
        /// 移動方向の入力を取得する
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetMoveDirection()
        {
            // 方向
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");
            Vector3 direction = new(x, 0.0f, z);

            return direction;
        }
    }
}
