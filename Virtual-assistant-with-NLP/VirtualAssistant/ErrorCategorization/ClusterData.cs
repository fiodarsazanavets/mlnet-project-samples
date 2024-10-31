namespace ErrorCategorization;

public static class ClusterData
{
    private static readonly Dictionary<uint, IList<ErrorData>> _clusters = [];

    public static IList<ErrorData> GetItemsInCluster(uint clusterId)
    {
        return _clusters[clusterId];
    }

    public static void AddItemToCluster(uint clusterId, Input item)
    {
        if (!_clusters.ContainsKey(clusterId))
            _clusters[clusterId] = new List<ErrorData>();

        _clusters[clusterId].Add(new ErrorData
        {
            ErrorCode = item.ErrorCode,
            ErrorMessage = item.ErrorMessage
        });
    }
}