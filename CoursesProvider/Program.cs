using CoursesProvider.Infrastructure.Data.Contexts;
using CoursesProvider.Infrastructure.GraphQL;
using CoursesProvider.Infrastructure.GraphQL.Mutations;
using CoursesProvider.Infrastructure.GraphQL.ObjectTypes;
using CoursesProvider.Infrastructure.Handlers;
using CoursesProvider.Infrastructure.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
	.ConfigureFunctionsWebApplication()
	.ConfigureServices(services =>
	{
		services.AddApplicationInsightsTelemetryWorkerService();
		services.ConfigureFunctionsApplicationInsights();

		services.AddPooledDbContextFactory<DataContext>(x =>
		{
			x.UseCosmos(Environment.GetEnvironmentVariable("COSMOS_URI")!, Environment.GetEnvironmentVariable("COSMOS_DBNAME")!)
			.UseLazyLoadingProxies();
		});

		services.AddScoped<ICourseService, CourseService>();
        services.AddSingleton<ServiceBusHandler>(sp =>
		new ServiceBusHandler(
         sp.GetRequiredService<ILogger<ServiceBusHandler>>(),
         "Endpoint=sb://courseprovider.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Q/rrVPV452ftyNuC9dEJDV84IoWUbvz8l+ASbDvjztg=",
         "courseprovider",
         "BackofficeApp",
         "FrontEndApp"
     )
 );


        services.AddGraphQLFunction()
		.AddQueryType<CourseQuery>()
		.AddMutationType<CourseMutation>()
		.AddType<CourseType>();

		var sp = services.BuildServiceProvider();
		using var scope = sp.CreateScope();
		var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<DataContext>>();
		using var context = dbContextFactory.CreateDbContext();
		context.Database.EnsureCreated();
	})
	.Build();

var cancellationTokenSource = new CancellationTokenSource();
var serviceBusHandler = host.Services.GetRequiredService<ServiceBusHandler>();
await serviceBusHandler.StartAsync(cancellationTokenSource.Token);



host.Run();

await serviceBusHandler.StopAsync(cancellationTokenSource.Token);
