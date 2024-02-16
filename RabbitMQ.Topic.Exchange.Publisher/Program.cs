using RabbitMQ.Client;
using System.Text;

ConnectionFactory connectionFactory = new();
connectionFactory.Uri = new("your_url");

using IConnection connection = connectionFactory.CreateConnection();
using IModel channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "topic-exchange-example", type: ExchangeType.Topic);

for (int i = 0; i < 100; i++)
{
    await Task.Delay(200);
    byte[] message = Encoding.UTF8.GetBytes("Merhaba : " + i);
    Console.Write("Topic Belirtiniz : ");
    string topic = Console.ReadLine();
    channel.BasicPublish(exchange: "topic-exchange-example", routingKey: topic, body: message);
}

Console.Read();