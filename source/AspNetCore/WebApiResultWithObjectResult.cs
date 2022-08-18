using Microsoft.AspNetCore.Mvc;

namespace FFCEI.Microservices.AspNetCore;

public class WebApiResultWithObjectResult : OkObjectResult
{
    public bool IsTextPlainResult { get; set; }

    public WebApiResultWithObjectResult(object? value)
        : base(value)
    {
    }
}
