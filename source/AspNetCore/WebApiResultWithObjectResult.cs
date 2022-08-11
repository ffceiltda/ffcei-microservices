using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFCEI.Microservices.AspNetCore;

public class WebApiResultWithObjectResult : OkObjectResult
{
    public bool IsTextPlainResult { get; set; }

    public WebApiResultWithObjectResult(object? value)
        : base(value)
    {
    }
}
