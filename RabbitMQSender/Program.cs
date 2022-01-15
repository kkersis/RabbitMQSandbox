using RabbitMQ.Client;
using RabbitMQReceiver;
using System;
using System.Text;
using System.Threading;

namespace RabbitMQSender
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            // 1. create connection
            using var connection = factory.CreateConnection();

            // 2. create channel
            using var channel = connection.CreateModel();
            // 3. connect to the queue
            channel.QueueDeclare(queue: "products",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            // start the receiver:
            // (normally this would be done using DI and
            // background worker would start itself in the background)
            var receiver = new ProductReceiverService().StartAsync(CancellationToken.None);

            int index = 1;
            while (index <= 99999)
            {
                string message = $"{index}|Product{10000 + index}|Potato|3|{DateTime.UtcNow.ToLongDateString()}|1|Lithuania";
                var body = Encoding.UTF8.GetBytes(message);

                // push content into the queue
                channel.BasicPublish(exchange: "",
                                     routingKey: "products",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine("Sent " + message);
                index++;
                Thread.Sleep(1000);
            }
        }
    }
}
