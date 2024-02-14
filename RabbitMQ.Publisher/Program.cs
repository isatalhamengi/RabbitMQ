using RabbitMQ.Client;
using System.Text;


//Bağlantı Oluşturma
ConnectionFactory factory = new();
factory.Uri = new("your_url");

//Bağlantı Aktifleştirme ve Kanal Açma
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

//Queue oluşturma
//durable : mesajın kalıcı olup olmadığını işaretler
//exclusive : yalnızda oluşturulan kuyruğa özel olup olmadığını ifader eder
channel.QueueDeclare(queue: "example-queue", exclusive: false, durable: true);

//burada durable parametresini destekleyerek kalıcı olmasını destekleyen bir özellik tanımlarız
IBasicProperties properties = channel.CreateBasicProperties();
properties.Persistent = true;

//Queue'ya mesaj gönderme
//RabbitMQ kuyrupa atacağı mesakjları byte türünden kabul etmektedir. Bizim gönderdiğimiz mesajlar byte'a dönüştürmemiz gerekmektedir.

//byte[] message = Encoding.UTF8.GetBytes("Merhaba");
//channel.BasicPublish(exchange: "", routingKey: "example-queue", body: message);

for (int i = 0; i < 100; i++)
{
    Thread.Sleep(1000);
    byte[] message = Encoding.UTF8.GetBytes("Merhaba " + i);
    //basicProperties : yukarda kalıcı olması için yaptığımız ayarları mesajı yayımlarken kullanmamız için bize yardımcı olan parametredir.
    channel.BasicPublish(exchange: "", routingKey: "example-queue", body: message, basicProperties: properties);
}


Console.Read();
