using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BusinessLayer.Email;

namespace BusinessLayer.RabbitMQ
{
    public class Consumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IModel _channel;
        private readonly string _queueName = "ForgotPasswordQueue";

        public Consumer(IConnection connection, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _channel = connection.CreateModel();

            // Declare the queue
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var jsonMessage = Encoding.UTF8.GetString(body);
                var message = JsonConvert.DeserializeObject<ResetPasswordMessage>(jsonMessage);

                // Create a scope to resolve EmailHelper
                using (var scope = _scopeFactory.CreateScope())
                {
                    var emailService = scope.ServiceProvider.GetRequiredService<EmailHelper>();
                    await emailService.SendPasswordResetEmailAsync(message.Email, message.ResetToken);
                }

                // Acknowledge message
                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            base.Dispose();
        }

        public class ResetPasswordMessage
        {
            public string Email { get; set; }
            public string ResetToken { get; set; }
        }
    }
}
