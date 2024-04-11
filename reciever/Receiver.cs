using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory
{
    HostName = "localhost",
    UserName = "guest",
    Password = "mystrongpassword"
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "test-exchange",
    type: ExchangeType.Direct,
    durable: true,
    autoDelete: false);

var sys_queue = channel.QueueDeclare().QueueName;
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
channel.QueueBind(queue: sys_queue,
    exchange: "test-exchange",
    routingKey: "secret-message");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, args) =>
{
    var enc = args.Body.ToArray();
    string message = Encoding.UTF8.GetString(enc);
    Console.WriteLine($" [x] Received :: {message}");
    channel.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
    Thread.Sleep(3000);
};

channel.BasicConsume(queue: sys_queue,
    autoAck: false,
    consumer: consumer);

Console.ReadLine();