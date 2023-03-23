using FluentValidation.AspNetCore;
using FluentValidation;
using System.Reflection;

namespace TatBlog.WebApi.Validations;

public static class FluentValidationDependencyInjection
{
	public static WebApplicationBuilder ConfigureFluentValidation(this WebApplicationBuilder builder)
	{
		// Enable client-side integration
		//builder.Services.AddFluentValidationClientsideAdapters();

		// Scan and register all validators in given assembly
		builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

		return builder;
	}
}
