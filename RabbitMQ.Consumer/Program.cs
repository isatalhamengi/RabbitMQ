using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;



//Bağlantı oluşturma
ConnectionFactory factory = new();
factory.Uri = new("your_url");
//Bağlantyı aktifleştirme ve Kanal Açma
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();
//Queue oluşturma
//durable : mesajın kalıcı olup olmadığını işaretler
//exclusive : yalnızda oluşturulan kuyruğa özel olup olmadığını ifader eder
channel.QueueDeclare(queue: "example-queue", exclusive: false, durable: true);
//Mesaj Okuma
//autoack : mesajın otomatik olarak onaylanıp onaylanmayacağını ifade eder eğer true olursa otomatik olarak işlendi bilgisi verir eğer Consumer tarafında hata olursa yine de mesaj işleneceği için bu değeri true yaparken dikkatli olmak gerekir. False yaptığımızda BasicAck fonksiyonu ile kullanılır.
EventingBasicConsumer consumer = new(channel);
channel.BasicConsume(queue: "example-queue", autoAck: false, consumer);

//Burada adil bir dağılım olmasını sağlıyoruz Fair Dispatch
//prefetchSize : maksimum alabileceği dosya boyutunu temsil eder 0 sınırsız demektir.
//prefetchCount : alabileceği maksimum mesajı ifader eder,
//global : sadece ilgili Consumerda mı yoksa tüm consumerlarda mı olacaüını ifader eder.
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
consumer.Received += async (sender, e) =>
{
    //Kuyruğa gelen mesajın işlendiği yer
    //e.Body : Kuyruktaki mesajın verisini bütünsel olarak getirecektir
    //e.Body.Span veya e.Body.ToArray() : Kuyruktaki mesajın byte verisinbi getirecektir.
    Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
    //Burada Message Acknowledge konusunu ele alırız yani bir mesajın başarılı işlendiğine dair kuyruk tarafına bir geri bilidim yaparız.
    //multiple : sadece ilgili mesajda mı geçerli bir işlem yapıp yapmadığımızı ifade eder.
    //Eğer başarılı olarak işlem olmadığını haber vereceksek BasicNack fonksiyonunu kullanırız.
    channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
};

Console.Read();