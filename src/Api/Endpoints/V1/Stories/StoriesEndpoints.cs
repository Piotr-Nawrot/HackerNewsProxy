using System.Net;
using Contracts;
using Contracts.V1.Stories;
using Domain.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Endpoints.V1.Stories;

public sealed class StoriesEndpoints
{
    private StoriesEndpoints(){}

    private static readonly Action<ILogger, int, Exception?> LogBadRequest =
        LoggerMessage.Define<int>(
            LogLevel.Error,
            new EventId(2, "TopStoriesRequestOccurred"),
            "Bad request occurred with count {Count}");
    
    private static readonly Action<ILogger, int, Exception> LogError =
        LoggerMessage.Define<int>(LogLevel.Error, new EventId(500, "InternalServerError"),
            "An error occurred while processing request with count {Count}");
    
    public static void AddMapping(WebApplication app)
    {
        app.MapGet($"{ApiVersion.V1}/stories", async (
                    [FromQuery] int top,
                    [FromServices] IHackerNewsService service,
                    [FromServices] ILogger<StoriesEndpoints> logger) =>
                {
                    if (top is <= 0 or > 500)
                    {
                        LogBadRequest(logger, top, null);
                        return Results.BadRequest(
                            new ErrorResponse(
                                "The 'top' parameter must be greater than 0 and less than or equal to 500."));
                    }

                    try
                    {
                        var stories = await service.GetBestStoriesAsync(top)
                            .ConfigureAwait(false);
                        var response = stories.Adapt<IEnumerable<GetStoryResponse>>();

                        return Results.Ok(response);
                    }
                    catch (Exception e)
                    {
                        LogError(logger, top, e);
                        throw;
                    }
                }
            )
            .Produces<GetStoryResponse>()
            .Produces<ErrorResponse>(statusCode: (int)HttpStatusCode.BadRequest)
            .WithMetadata(new SwaggerOperationAttribute(
                summary: "Retrieves the top N best stories from the Hacker News API.",
                description:
                "This endpoint fetches the specified number of top-ranked stories from the Hacker News API."));
    }
}