using RabbitMQ.Client;

namespace RabbitMqCreateExcel.Web.Services
{
    public class RabbitMqClientService /*: IDisposable*/
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private readonly ILogger<RabbitMqClientService> _logger;
        public static readonly string ExchangeName = "ExcelDirectExchange";
        public static readonly string RoutingExcel = "excel-route-file";
        public static readonly string QueueName = "queue-excel-file";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public RabbitMqClientService(ConnectionFactory connectionFactory, ILogger<RabbitMqClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();
            if (_channel is { IsOpen: true })
            {
                return _channel;
            }
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(ExchangeName, type: ExchangeType.Direct, true, false);
            _channel.QueueDeclare(QueueName, true, false, false);
            _channel.QueueBind(exchange: ExchangeName, queue: QueueName, routingKey: RoutingExcel);
            _logger.LogInformation("RabbitMq ile bağlantı kuruldu...");
            return _channel;
        }

        //public void Dispose()
        //{
        //    _channel?.Close();
        //    _channel?.Dispose();
        //    _connection?.Close();
        //    _connection?.Dispose();
        //    _logger.LogInformation("RabbitMq ile bağlantı koptu...");
        //}
    }
}
