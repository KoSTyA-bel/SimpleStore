using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Store.BLL.Interfaces;
using Store.DataTransferLevel.Settings;
using System.Text;

namespace Store.DataTransferLevel;

public class DataListener : IDataListener, IDisposable
{
    private readonly RabbitSettings _settings;
    private CancellationTokenSource _cancelTokenSource;
    private CancellationToken _token;

    public DataListener(RabbitSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _cancelTokenSource = new CancellationTokenSource();
        _token = _cancelTokenSource.Token;
    }

    public void StartListen()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password
        };

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: _settings.RoutingKey, durable: false,
              exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: _settings.RoutingKey,
                                 autoAck: false, consumer: consumer);

            consumer.Received += (model, ea) =>
            {
                string response = TransferStatus.Failure;

                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                try
                {
                    var message = Encoding.UTF8.GetString(body);
                    // TODO
                    // Implement the purchase of a single item.
                    response = TransferStatus.Success;
                }
                catch (Exception e)
                {
                    response = TransferStatus.Failure;
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                      basicProperties: replyProps, body: responseBytes);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag,
                      multiple: false);
                }
            };

            while (!_token.IsCancellationRequested)
            {

            }
        }
    }

    public void StopListen()
    {
        _cancelTokenSource.Cancel();
    }

    public void Dispose()
    {
        StopListen();
    }
}
