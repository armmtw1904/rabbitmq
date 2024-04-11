using System.Text;
using RabbitMQ.Client;

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

var basicprops = channel.CreateBasicProperties();
basicprops.Persistent = true;

SendMessage();

void SendMessage()
{
    //Console.WriteLine("enter your secret message !!");

    for(int i = 0; i < 100; i++)
    {
        string message = $"secret message {i}!!";
        var enc = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "test-exchange",
            routingKey: "secret-message",
            body: enc,
            basicProperties: basicprops);

        Console.WriteLine($" [{i}] Sent :: {message}");
        Thread.Sleep(2000);
    }

    Console.ReadLine();
    //Console.WriteLine("do you want to continue sending messages, if yes enter 'Y' !!");
    //var inp = Console.ReadLine();
    //if (inp.Equals("Y", StringComparison.OrdinalIgnoreCase))
    //{
    //    SendMessage();
    //}
}