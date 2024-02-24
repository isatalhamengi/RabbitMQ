using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory factory = new();
factory.Uri = new("your_url");

using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

#region P2P (Point to Point) Tasarımı
/*
string queueName = "example-p2p-queue";

channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false);

EventingBasicConsumer consumer = new(channel);

channel.BasicConsume(queue: queueName,autoAck:false,consumer:consumer);

consumer.Received += (sender, e) =>
{
    string message = Encoding.UTF8.GetString(e.Body.Span);
    Console.WriteLine(message);
};
*/
#endregion
#region Publish/Subscribe (Pub/Sub) Tasarımı
/*
string exchangeName = "example-pub-sub-exchange";
channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

string queueName = channel.QueueDeclare().QueueName;

channel.QueueBind(
    queue:queueName,
    exchange: exchangeName,
    routingKey:string.Empty
    );

EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(
    queue:queueName,
    autoAck:false,
    consumer:consumer
    );

consumer.Received += (sender, e) =>
{
    string message = Encoding.UTF8.GetString(e.Body.Span);
    Console.WriteLine(message);
};
*/
#endregion
#region Work Queue(İş Kuyruğu) Tasarımı
/*
string queueName = "example-work-queue";

channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false);

EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

channel.BasicQos(
    prefetchCount: 1,
    prefetchSize: 0,
    global: false
    );

consumer.Received += (sender, e) =>
{
    string message = Encoding.UTF8.GetString(e.Body.Span);
    Console.WriteLine(message);
};
*/
#endregion
#region Request/Response Tasarımı
string queueName = "example-request-repsonse-queue";

channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false);

EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(
    queue: queueName,
    autoAck: true,
    consumer: consumer
    );

consumer.Received += (sender, e) =>
{
    string message = Encoding.UTF8.GetString(e.Body.Span);
    Console.WriteLine(message);
    //..........
    byte[] responseMessage = Encoding.UTF8.GetBytes($"İşlem Tamamlandı : {message}");

    IBasicProperties basicProperties = channel.CreateBasicProperties();
    basicProperties.CorrelationId = e.BasicProperties.CorrelationId;
    channel.BasicPublish(
        exchange: string.Empty,
        routingKey: e.BasicProperties.ReplyTo,
        basicProperties: basicProperties,
        body: responseMessage
        );
};
#endregion

Console.Read();