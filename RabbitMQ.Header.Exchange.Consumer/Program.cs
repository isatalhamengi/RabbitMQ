﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory connectionFactory = new();
connectionFactory.Uri = new("your_url");

using IConnection connection = connectionFactory.CreateConnection();
using IModel channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "header-exchange-example", type: ExchangeType.Headers);

Console.Write("Value : ");
string value = Console.ReadLine();

string queueName = channel.QueueDeclare().QueueName;

channel.QueueBind(queue: queueName, exchange: "header-exchange-example", routingKey: string.Empty, arguments: new Dictionary<string, object>
{
    ["no"] = value
});

EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(queue:queueName,autoAck:true,consumer:consumer);

consumer.Received += (sender, e) =>
{
    string message = Encoding.UTF8.GetString(e.Body.Span);
    Console.WriteLine(message);
};

Console.Read();