using Google.Protobuf.Protocol;

namespace ServerContents.Data
{
    internal class Data
    {
        #region Stat
        [Serializable]
        public class StatData : ILoader<int, StatInfo>
        {
            public List<StatInfo> stats = new List<StatInfo>();

            public Dictionary<int, StatInfo> MakeDict()
            {
                Dictionary<int, StatInfo> dict = new Dictionary<int, StatInfo>();
                foreach (StatInfo stat in stats)
                {
                    dict.Add(stat.Level, stat);
                }
                return dict;
            }
        }
        #endregion
    }
}
