using Chess.Web.Validation;
using FluentValidation;
using Microsoft.AspNetCore.ResponseCompression;
using MudBlazor.Services;

namespace Chess.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebApp(this IServiceCollection collection)
    {
        collection.AddRazorPages();
        collection.AddServerSideBlazor();
        //To improve performacne, binary data is compressed for the response.
        collection.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
        });

        collection.AddMudServices();
        collection.AddValidatorsFromAssemblyContaining<SetupModelValidator>();

        return collection;
    }
}