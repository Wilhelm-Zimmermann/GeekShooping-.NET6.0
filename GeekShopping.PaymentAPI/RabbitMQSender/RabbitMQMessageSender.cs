using GeekShopping.PaymentAPI.Messages;
using GeekShopping.MessageBus;
using RabbitMQ.Client;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;

namespace GeekShopping.PaymentAPI.RabbitMQSender
{
    public class RabbitMQMessageSender : IRabbitMQMessageSender
    {
        private readonly string _hostName;
        private readonly string _password;
        private readonly string _userName;
        private IConnection _connection;
        private const string exchangeName = "DirectPaymentUpdateExchange";
        private const string paymentEmailUpdateQueueName = "paymentEmailUpdateQueueName";
        private const string paymentOrderUpdateQueueName = "paymentOrderUpdateQueueName";

        public RabbitMQMessageSender()
        {
            _hostName = "localhost";
            _password = "guest";
            _userName = "guest";
        }

        public void SendMessage(BaseMessage message)
        {
            if(ConnectionExists())
            { 
                using var channel = _connection.CreateModel();
                channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: false);
                channel.QueueDeclare(paymentEmailUpdateQueueName, false, false, false, null);
                channel.QueueDeclare(paymentOrderUpdateQueueName, false, false, false, null);

                channel.QueueBind(paymentEmailUpdateQueueName, exchangeName, "PaymentEmail");
                channel.QueueBind(paymentOrderUpdateQueueName, exchangeName, "PaymentOrder");

                byte[] body = GetMessageAsByteArray(message);
                channel.BasicPublish(exchange: exchangeName, "PaymentEmail", basicProperties: null, body);
                channel.BasicPublish(exchange: exchangeName, "PaymentOrder", basicProperties: null, body);
            }
        }

        private byte[] GetMessageAsByteArray(BaseMessage message)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize<UpdatePaymentResultMessage>((UpdatePaymentResultMessage)message, options);
            var body = Encoding.UTF8.GetBytes(json);
            return body;
        }

        private bool ConnectionExists()
        {
            if (_connection != null) return true;
            CreateConnection();
            return _connection != null;
        }

        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostName,
                    Password = _password,
                    UserName = _userName,
                };
                _connection = factory.CreateConnection();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
