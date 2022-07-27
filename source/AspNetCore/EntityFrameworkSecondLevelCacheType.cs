namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Entity Framework Second Level Cache Type
/// </summary>
public enum EntityFrameworkSecondLevelCacheType
{
    /// <summary>
    /// No caching
    /// </summary>
    NoCache,
    /// <summary>
    /// Use local in-memory cache
    /// </summary>
    MemoryCache,
    /// <summary>
    /// Use Redis based cache
    /// </summary>
    RedisCache
}
