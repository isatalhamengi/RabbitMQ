using RabbitMQ.Client;
using System.Text;

ConnectionFactory connectionFactory = new();
connectionFactory.Uri = new("your_url");

using IConnection connection = connectionFactory.CreateConnection();
using IModel channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "fanout-exchange-example", type: ExchangeType.Fanout);

for (int i = 0; i < 100; i++)
{
    await Task.Delay(200);
    byte[] message = Encoding.UTF8.GetBytes("Merhaba : " + i);
    channel.BasicPublish(exchange: "fanout-exchange-example", routingKey: string.Empty, body: message);
}

Console.Read();

// Burada dikkat edilmesi gereken husus BasicPublish fonksiyonunda routingKey parametresini boş olarak veriyoruz eğer routingKey vererek belirli bir route'a mesaj göndermek istiyorsak Fanout Exchange Kullanmamalıyız!