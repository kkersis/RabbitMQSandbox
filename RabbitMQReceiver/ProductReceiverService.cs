using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQReceiver
{
    public class ProductReceiverService : BackgroundService
    {
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        // initialize the connection, channel and queue 
        // inside the constructor to persist them 
        // for until the service (or the application) runs
        public ProductReceiverService()
        {
            _factory = new ConnectionFactory() { HostName = "localhost" };

            _connection = _factory.CreateConnection();

            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: "heroes",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // when the service is stopping
            // dispose these references
            // to prevent leaks
            if (stoppingToken.IsCancellationRequested)
            {
                _channel.Dispose();
                _connection.Dispose();
                return Task.CompletedTask;
            }

            // create a consumer that listens on the channel (queue)
            var consumer = new EventingBasicConsumer(_channel);

            // handle the Received event on the consumer
            // this is triggered whenever a new message
            // is added to the queue by the producer
            consumer.Received += (model, ea) =>
            {
                // read the message bytes
                var body = ea.Body.ToArray();

                // convert back to the original string
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine("Received " + message);

                Task.Run(() =>
                {
                    // splitting message, maching it to some class
                    // then saving the data or doing something
                    // with it should be done here
                });
            };

            _channel.BasicConsume(queue: "products", autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
