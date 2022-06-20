namespace SetsunaTsuyuri.SurviveUnitychan
{
    /// <summary>
    /// アイテムに接触したとき、何かが起きるオブジェクト
    /// </summary>
    public interface IItemHit
    {
        /// <summary>
        /// アイテムに接触したときの処理
        /// </summary>
        /// <param name="item">アイテム</param>
        void OnHit(Item item);
    }
}
