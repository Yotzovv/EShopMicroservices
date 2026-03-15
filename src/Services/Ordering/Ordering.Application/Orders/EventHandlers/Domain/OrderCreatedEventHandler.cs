using MassTransit;

namespace Ordering.Application.Orders.EventHandlers.Domain
{
    public class OrderCreatedEventHandler
        (IPublishEndpoint publish, ILogger<OrderCreatedEventHandler> logger) 
        : INotificationHandler<OrderCreatedEvent>
    {
        public async Task Handle(OrderCreatedEvent domainEvent, CancellationToken cancellationToken)
        {
            logger.LogInformation("Domain event handled: {DomainEvent}", domainEvent.GetType().Name);

            var orderCreatedIntegrationEvent = domainEvent.Order.ToOrderDto();

            await publish.Publish(orderCreatedIntegrationEvent, cancellationToken);

            return Task.CompletedTask;
        }
    }
}
