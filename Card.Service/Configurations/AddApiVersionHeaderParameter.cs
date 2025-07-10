using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Card.Service.Configurations
{
    public class AddApiVersionHeaderParameter : IOperationFilter
    {
        private readonly string _apiVersion;
        public AddApiVersionHeaderParameter(string apiVersion)
        {
            _apiVersion = apiVersion;
        }
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-API-Version",
                In = ParameterLocation.Header,
                Required = true,
                Description = "ApiVersion",
                Example = new OpenApiString(_apiVersion)
                
            });   
        }
    }
}