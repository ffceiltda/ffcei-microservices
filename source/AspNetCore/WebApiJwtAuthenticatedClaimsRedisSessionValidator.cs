using FFCEI.Microservices.Configuration;
using StackExchange.Redis;
using System.Diagnostics;
using System.Globalization;

namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Redis Based Web Api Jwt Authenticated Claims Session Validator
/// </summary>
public sealed class WebApiJwtAuthenticatedClaimsRedisSessionValidator : IWebApiJwtAuthenticatedClaimsSessionValidator
{
    private readonly ConnectionMultiplexer _redisConnection;
    private readonly IDatabase _redisDatabase;

    /// <summary>
    /// Default constrcutor
    /// </summary>
    /// <param name="configuration">Redis configuration</param>
    /// <exception cref="ArgumentNullException">throw if Redis configuration is null</exception>
    public WebApiJwtAuthenticatedClaimsRedisSessionValidator(RedisConnectionConfiguration configuration)
    {
        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        _redisConnection = ConnectionMultiplexer.Connect(configuration.ConnectionString, Debugger.IsAttached ? Console.Error : null);
        _redisDatabase = _redisConnection.GetDatabase(configuration.Database);
    }

    public async Task SaveSessionAsync(Guid claimer, Guid session, string resource, string bearerToken, long expirationInSeconds, int maxNumberOfSimultaneousSessions = 1)
    {
        if (claimer == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(claimer));
        }

        if (session == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(session));
        }

        if (string.IsNullOrEmpty(resource))
        {
            throw new ArgumentNullException(nameof(resource));
        }

        var now = DateTime.UtcNow;

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        var scriptResult = await _redisDatabase.ScriptEvaluateAsync(
            LuaScript.Prepare(@"
                local existingSpecificTokens = redis.call('KEYS', 'claim:*:'..@Session..':'..@Claimer..':'..@Resource)
                
                if #existingSpecificTokens > 0 then
                    redis.call('DEL', existingSpecificTokens[1])
                else
                    
                    local existingTokens = redis.call('KEYS', 'claim:*:*:'..@Claimer..':*')
                
                    if #existingTokens == tonumber(@MaxNumberOfSimultaneousSessions) then
                        redis.call('DEL', existingTokens[1])
                    end

                end

                redis.call('SET', 'claim:'..@Timestamp..':'..@Session..':'..@Claimer..':'..@Resource, @BearerToken, 'EX', tonumber(@ExpirationInSeconds))

                return 'OK'
                "),
            new
            {
                Timestamp = now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                Session = session.ToString(),
                Claimer = claimer.ToString(),
                Resource = resource,
                BearerToken = bearerToken,
                ExpirationInSeconds = expirationInSeconds,
                MaxNumberOfSimultaneousSessions = maxNumberOfSimultaneousSessions
            });
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        if (scriptResult is null)
        {
            throw new InvalidOperationException("Redis call failed");
        }

        string value = (string)scriptResult;

        if (string.IsNullOrEmpty(value) || (value != "OK"))
        {
            throw new InvalidOperationException($"Redis call failed, expected 'OK' but received '{value}'");
        }
    }

    public async Task<string> GetSessionAsync(Guid claimer, Guid session, string resource)
    {
        if (claimer == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(claimer));
        }

        if (session == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(session));
        }

        if (string.IsNullOrEmpty(resource))
        {
            throw new ArgumentNullException(nameof(resource));
        }

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        var scriptResult = await _redisDatabase.ScriptEvaluateAsync(
            LuaScript.Prepare(@"
                local existingTokens = redis.call('KEYS', 'claim:*:'..@Session..':'..@Claimer..':'..@Resource)

                if #existingTokens > 0 then
                    return redis.call('GET', existingTokens[1])
                end

                return 'NOTFOUND'
                "),
            new
            {
                Session = session.ToString(),
                Claimer = claimer.ToString(),
                Resource = resource
            });
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        if (scriptResult is null)
        {
            throw new InvalidOperationException("Redis call failed");
        }

        string value = (string)scriptResult;

        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidOperationException($"Redis call failed, expected 'OK' or 'NOTFOUND' but received '{value}'");
        }

        return value;
    }

    public async Task ExpireSessionAsync(Guid claimer, Guid session, string resource)
    {
        if (claimer == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(claimer));
        }

        if (session == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(session));
        }

        if (string.IsNullOrEmpty(resource))
        {
            throw new ArgumentNullException(nameof(resource));
        }

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        var scriptResult = await _redisDatabase.ScriptEvaluateAsync(
            LuaScript.Prepare(@"
                local existingTokens = redis.call('KEYS', 'claim:*:'..@Session..':'..@Claimer..':'..@Resource)

                for _, existingToken in ipairs(existingTokens) do
                    redis.call('DEL', existingToken)
                end

                return 'OK'
                "),
            new
            {
                Session = session.ToString(),
                Claimer = claimer.ToString(),
                Resource = resource
            });
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        if (scriptResult is null)
        {
            throw new InvalidOperationException("Redis call failed");
        }

        string value = (string)scriptResult;

        if (string.IsNullOrEmpty(value) || (value != "OK"))
        {
            throw new InvalidOperationException($"Redis call failed, expected 'OK' but received '{value}'");
        }
    }

    public async Task ExpireAllSessionsAsync(Guid claimer)
    {
        if (claimer == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(claimer));
        }

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        var scriptResult = await _redisDatabase.ScriptEvaluateAsync(
            LuaScript.Prepare(@"
                local existingTokens = redis.call('KEYS', 'claim:*:*:'..@Claimer..':*')

                for _, existingToken in ipairs(existingTokens) do
                    redis.call('DEL', existingToken)
                end

                return 'OK'
                "),
            new
            {
                Claimer = claimer.ToString(),
            });
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        if (scriptResult is null)
        {
            throw new InvalidOperationException("Redis call failed");
        }

        string value = (string)scriptResult;

        if (string.IsNullOrEmpty(value) || (value != "OK"))
        {
            throw new InvalidOperationException($"Redis call failed, expected 'OK' but received '{value}'");
        }
    }
}
