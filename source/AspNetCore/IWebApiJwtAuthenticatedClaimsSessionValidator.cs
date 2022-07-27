namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Web Api Jwt Authenticated Claims Session Validator
/// </summary>
public interface IWebApiJwtAuthenticatedClaimsSessionValidator
{
    /// <summary>
    /// Save a session
    /// </summary>
    /// <param name="claimer">Claimer</param>
    /// <param name="session">Session</param>
    /// <param name="resource">Resource</param>
    /// <param name="bearerToken">JWT bearer token</param>
    /// <param name="expirationInSeconds">Expiration in seconds</param>
    /// <param name="maxNumberOfSimultaneousSessions">Maximum number of simultaneous sessions</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">throw if claimer, session or resource is empty</exception>
    /// <exception cref="InvalidOperationException">throw if Redis call failed</exception>
    Task SaveSessionAsync(Guid claimer, Guid session, string resource, string bearerToken, long expirationInSeconds, int maxNumberOfSimultaneousSessions = 1);

    /// <summary>
    /// Find a session if valid
    /// </summary>
    /// <param name="claimer">Claimer</param>
    /// <param name="session">Session</param>
    /// <param name="resource">Resource</param>
    /// <returns>Bearer token</returns>
    /// <exception cref="ArgumentNullException">throw if claimer, session or resource is empty</exception>
    /// <exception cref="InvalidOperationException">throw if Redis call failed</exception>
    Task<string> GetSessionAsync(Guid claimer, Guid session, string resource);

    /// <summary>
    /// Expires a session
    /// </summary>
    /// <param name="claimer">Claimer</param>
    /// <param name="session">Session</param>
    /// <param name="resource">Resource</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">throw if claimer, session or resource is empty</exception>
    /// <exception cref="InvalidOperationException">throw if Redis call failed</exception>
    Task ExpireSessionAsync(Guid claimer, Guid session, string resource);

    /// <summary>
    /// Expire all sessions from a claimer
    /// </summary>
    /// <param name="claimer">Claimer</param>
    /// <returns>nothing</returns>
    /// <exception cref="ArgumentNullException">throw if claimer is empty</exception>
    /// <exception cref="InvalidOperationException">throw if Redis call failed</exception>
    Task ExpireAllSessionsAsync(Guid claimer);
}
