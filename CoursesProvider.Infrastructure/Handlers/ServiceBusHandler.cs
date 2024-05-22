using Azure.Messaging.ServiceBus;
using Castle.Core.Logging;
using HotChocolate.Subscriptions;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CoursesProvider.Infrastructure.Handlers;

public class ServiceBusHandler
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;
    private readonly ServiceBusProcessor _processorBackofficeApp;
    private readonly ServiceBusProcessor _processorFrontEndApp;

    private readonly ILogger<ServiceBusHandler> _logger;


    public ServiceBusHandler(ILogger<ServiceBusHandler> logger, string connectionString, string courseprovider, string BackofficeApp, string FrontEndApp)
    {
        _logger = logger;
        _client = new ServiceBusClient(connectionString);
        _sender = _client.CreateSender(courseprovider);
        _processorBackofficeApp = _client.CreateProcessor(courseprovider, BackofficeApp);
        _processorFrontEndApp = _client.CreateProcessor(courseprovider, FrontEndApp);

        _processorBackofficeApp.ProcessMessageAsync += MessageHandler;
        _processorBackofficeApp.ProcessErrorAsync += ErrorHandler;

        _processorFrontEndApp.ProcessMessageAsync += MessageHandler;
        _processorFrontEndApp.ProcessErrorAsync += ErrorHandler;
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "An error occurred while processing a Service Bus message.");
        return Task.CompletedTask;
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        try
        {
            string message = args.Message.Body.ToString();
            _logger.LogInformation("Received message: {Message}");

            await PublishAsync(message);

            await args.CompleteMessageAsync(args.Message);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while handling a Service Bus message.");

        }
    }

    public async Task PublishAsync(string message, string messageType = null!)
    {

        try
        {
            var payload = new ServiceBusMessage(message);
            if (messageType != null)
                payload.ApplicationProperties.Add("messageType", messageType);

            await _sender.SendMessageAsync(payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while handling a Service Bus message.");
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _processorBackofficeApp.StartProcessingAsync(cancellationToken);
        await _processorFrontEndApp.StartProcessingAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processorBackofficeApp.StopProcessingAsync(cancellationToken);
        await _processorFrontEndApp.StopProcessingAsync(cancellationToken);
    }



}


