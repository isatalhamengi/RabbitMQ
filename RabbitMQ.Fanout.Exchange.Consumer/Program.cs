using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory connectionFactory = new();
connectionFactory.Uri = new("your_url");

using IConnection connection = connectionFactory.CreateConnection();
using IModel channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "fanout-exchange-example", type: ExchangeType.Fanout);

Console.Write("Kuyruk Adını Giriniz : ");
string _queueName = Console.ReadLine();

channel.QueueDeclare(queue: _queueName, exclusive: false);

channel.QueueBind(queue: _queueName, exchange: "fanout-exchange-example", routingKey: string.Empty);

EventingBasicConsumer consumer = new(channel);

channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

consumer.Received += (sender, e) =>
{
    string message = Encoding.UTF8.GetString(e.Body.Span);
    Console.WriteLine(message);
};

Console.Read();