using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace BusinessLayer.RabbitMQ
{
    public class Producer
    {
        private readonly IModel _channel;
        private readonly string _queueName = "ForgotPasswordQueue";

        public Producer(IConnection connection)
        {
            _channel = connection.CreateModel();

            // Declare the queue
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void PublishMessage(object message)
        {
            var jsonMessage = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            _channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
        }
    }
}
