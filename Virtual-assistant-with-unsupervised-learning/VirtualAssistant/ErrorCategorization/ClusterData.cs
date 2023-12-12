namespace ErrorCategorization;

public static class ClusterData
{
    private static Dictionary<uint, IList<Input>> _clusters = [];

    public static IList<Input> GetItemsInCluster(uint clusterId)
    {
        return _clusters[clusterId];
    }

    public static void AddItemToCluster(uint clusterId, Input item)
    {
        if (!_clusters.ContainsKey(clusterId))
            _clusters[clusterId] = new List<Input>();

        _clusters[clusterId].Add(item);
    }
}
