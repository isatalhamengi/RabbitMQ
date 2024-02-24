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

byte[] message = Encoding.UTF8.GetBytes("Merhaba");
channel.BasicPublish(exchange: string.Empty, routingKey: queueName, body: message);
*/
#endregion
#region Publish/Subscribe (Pub/Sub) Tasarımı
/*
string exchangeName = "example-pub-sub-exchange";
channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

byte[] message = Encoding.UTF8.GetBytes("Merhaba");
channel.BasicPublish(exchange: exchangeName, routingKey: string.Empty, body: message);
*/
#endregion
#region Work Queue(İş Kuyruğu) Tasarımı
/*
string queueName = "example-work-queue";

channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false);

for (int i = 0; i < 100; i++)
{
    await Task.Delay(200);

    byte[] message = Encoding.UTF8.GetBytes($"Merhaba {i}");

    channel.BasicPublish(exchange: string.Empty, routingKey: queueName, body: message);
}
*/
#endregion
#region Request/Response Tasarımı
string queueName = "example-request-repsonse-queue";

channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false);

string replyQueueName = channel.QueueDeclare().QueueName;
string correlationID = Guid.NewGuid().ToString();

#region Request Mesajını Oluşturma ve Gönderme
IBasicProperties basicProperties = channel.CreateBasicProperties();
basicProperties.CorrelationId = correlationID;
basicProperties.ReplyTo = replyQueueName;

for (int i = 0; i < 100; i++)
{
    byte[] message = Encoding.UTF8.GetBytes("Merhaba " + i);
    channel.BasicPublish(exchange: string.Empty, routingKey: queueName, body: message, basicProperties: basicProperties);
}
#endregion

#region Response Kuyruğu Dinleme
EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(queue: replyQueueName, autoAck: true, consumer: consumer);
consumer.Received += (sender, e) =>
{
    //.......
    if (e.BasicProperties.CorrelationId == correlationID)
    {
        Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
    }
};
#endregion
#endregion

Console.Read();