using FFCEI.Microservices.Json;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Text.Json;

namespace FFCEI.Microservices.AspNetCore;

/// <summary>
/// Web Api Client object
/// </summary>
public class WebApiClient
{
    /// <summary>
    /// HTTP Api Uri
    /// </summary>
    public Uri ApiHttpUrl { get; set; } = new Uri("http://localhost");

    /// <summary>
    /// HTTP Authentication Token
    /// </summary>
    public string? AuthenticationToken { get; set; }

    /// <summary>
    /// Ignore HTTPS SSL Errors
    /// </summary>
    public bool IgnoreHTTPSSLErrors { get; set; }

    /// <summary>
    /// Create a Rest Client
    /// </summary>
    /// <returns>a HTTP Rest Client instance</returns>
    protected RestClient CreateRestClient()
    {
        var httpOptions = new RestClientOptions
        {
            BaseUrl = ApiHttpUrl,
            Authenticator = string.IsNullOrEmpty(AuthenticationToken) ? null : new JwtAuthenticator(AuthenticationToken)
        };

        if (IgnoreHTTPSSLErrors)
        {
            httpOptions.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => { return IgnoreHTTPSSLErrors; };
        }

        var httpClient = new RestClient(httpOptions);

        return httpClient;
    }

    protected async Task<RestResponse> ExecuteHTTPMethodAsync(string httpApiPath, Method requestMethod)
    {
        using var httpClient = CreateRestClient();

        var httpRequest = new RestRequest(httpApiPath)
        {
            Method = requestMethod
        };

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        var httpResponse = await httpClient.ExecuteAsync(httpRequest);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        return httpResponse;
    }

    protected async Task<(RestResponse, TOut?)> ExecuteHTTPMethodWithJsonResponseAsync<TOut>(string httpApiPath, Method requestMethod)
        where TOut : class, new()
    {
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        var httpResponse = await ExecuteHTTPMethodAsync(httpApiPath, requestMethod);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        if ((httpResponse.StatusCode != HttpStatusCode.OK) || (httpResponse.Content == null))
        {
            return (httpResponse, null);
        }

        var jsonResponse = JsonSerializer.Deserialize<TOut>(httpResponse.Content, JsonSerializerOptionsExtensionMethods.WebApiOptions);

        return (httpResponse, jsonResponse);
    }

    protected async Task<RestResponse> ExecuteHTTPMethodWithBodyAsync(string httpApiPath, Method requestMethod, DataFormat requestFormat, object requestBody)
    {
        using var httpClient = CreateRestClient();

        var httpRequest = new RestRequest(httpApiPath)
        {
            Method = requestMethod,
            RequestFormat = requestFormat
        }
        .AddBody(requestBody);

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        var httpResponse = await httpClient.ExecuteAsync(httpRequest);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        return httpResponse;
    }

    protected async Task<RestResponse> ExecuteHTTPMethodWithJsonBodyAsync<TIn>(string httpApiPath, Method requestMethod,
        TIn jsonRequest) where TIn : class
    {
        using var httpClient = CreateRestClient();

        var httpRequest = new RestRequest(httpApiPath)
        {
            Method = requestMethod,
            RequestFormat = DataFormat.Json
        }
        .AddJsonBody(jsonRequest);

#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        var httpResponse = await httpClient.ExecuteAsync(httpRequest);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        return httpResponse;
    }

    protected async Task<(RestResponse, TOut?)> ExecuteHTTPMethodWithJsonBodyAndResponseAsync<TIn, TOut>(string httpApiPath, Method requestMethod,
        TIn jsonRequest) where TIn : class where TOut : class, new()
    {
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
        var httpResponse = await ExecuteHTTPMethodWithJsonBodyAsync<TIn>(httpApiPath, requestMethod, jsonRequest);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

        if ((httpResponse.StatusCode != HttpStatusCode.OK) || (httpResponse.Content == null))
        {
            return (httpResponse, null);
        }

        var jsonResponse = JsonSerializer.Deserialize<TOut>(httpResponse.Content, JsonSerializerOptionsExtensionMethods.WebApiOptions);

        return (httpResponse, jsonResponse);
    }
}
