using RabbitMQ.Client;
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

            int index = 1;
            while (index <= 99999)
            {
                string message = $"{index}|Product{10000 + index}|Potato|3|{DateTime.UtcNow.ToLongDateString()}|1|Poland";
                var body = Encoding.UTF8.GetBytes(message);

                // push content into the queue
                channel.BasicPublish(exchange: "",
                                     routingKey: "products",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", message);
                index++;
                Thread.Sleep(10000);
            }
        }
    }
}
