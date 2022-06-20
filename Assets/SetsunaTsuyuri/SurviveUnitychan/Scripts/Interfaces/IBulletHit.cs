namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// 弾が命中するオブジェクト
    /// </summary>
    public interface IBulletHit
    {
        /// <summary>
        /// 弾が命中したときの処理
        /// </summary>
        /// <param name="bullet">弾</param>
        void OnHit(Bullet bullet);
    }
}
