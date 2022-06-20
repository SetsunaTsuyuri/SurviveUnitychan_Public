namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 敵ユニットに接触したとき、何かが起きるオブジェクト
    /// </summary>
    public interface IEnemyHit
    {
        /// <summary>
        /// 敵ユニットに接触したときの処理
        /// </summary>
        /// <param name="enemy">敵ユニット</param>
        void OnHit(Enemy enemy);
    }
}
