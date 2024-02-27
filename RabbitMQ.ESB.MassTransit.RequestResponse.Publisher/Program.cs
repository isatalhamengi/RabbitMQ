using MassTransit;
using RabbitMQ.ESB.MassTransit.Shared.RequestResponseMessage;

string rabbitMQUri = "your_url";
string queueName = "request-queue";

IBusControl bus = Bus.Factory.CreateUsingRabbitMq(factory =>
{
    factory.Host(rabbitMQUri);
});

await bus.StartAsync();

IRequestClient<RequestMessage> requestClient =  bus.CreateRequestClient<RequestMessage>(new Uri($"{rabbitMQUri}/{queueName}"));

int i = 0;
while (true)
{
    await Task.Delay(200);
    var response = await requestClient.GetResponse<ResponseMessage>(new()
    {
        MessageNo = i,
        Text = $"{i}. request"
    });
    i++;
    Console.WriteLine($"Message Received : {response.Message.Text}");
}
Console.Read();