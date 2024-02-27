using MassTransit;
using RabbitMQ.ESB.MassTransit.RequestResponse.Consumer.Consumers;

string rabbitMQUri = "your_url";
string queueName = "request-queue";

IBusControl bus = Bus.Factory.CreateUsingRabbitMq(factory =>
{
    factory.Host(rabbitMQUri);

    factory.ReceiveEndpoint(queueName:queueName,endpoint =>
    {
        endpoint.Consumer<RequestMessageConsumer>();
    });
});

await bus.StartAsync();

Console.Read();