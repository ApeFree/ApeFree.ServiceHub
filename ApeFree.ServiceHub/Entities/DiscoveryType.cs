namespace ApeFree.ServiceHub.Entities
{
    /// <summary>
    /// 发现匹配类型
    /// </summary>
    public enum DiscoveryType
    {
        /// <summary>
        /// 按服务ID检索
        /// </summary>
        Id,
        /// <summary>
        /// 按服务名称检索
        /// </summary>
        Name,
        /// <summary>
        /// 按关键词检索
        /// </summary>
        Keywords,
    }
}
