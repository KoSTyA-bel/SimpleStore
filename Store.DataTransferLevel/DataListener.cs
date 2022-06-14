using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Store.BLL.Entities;
using Store.BLL.Interfaces;
using Store.DataTransferLevel.Settings;
using System.Text;

namespace Store.DataTransferLevel;

public class DataListener : IDataListener, IDisposable
{
    private readonly RabbitSettings _settings;
    private readonly IServiceProvider _provider;
    private CancellationTokenSource _cancelTokenSource;
    private CancellationToken _token;

    public DataListener(IServiceProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _settings = provider.GetService(typeof(RabbitSettings)) as RabbitSettings ?? throw new ArgumentNullException("Can`t find RabbitSettings.");
        _cancelTokenSource = new CancellationTokenSource();
        _token = _cancelTokenSource.Token;
    }

    public CancellationTokenSource CancellationTokenSource { get => _cancelTokenSource; }

    public async void StartListen()
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
                    response = BuyProduct(message).GetAwaiter().GetResult();
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

    protected virtual async Task<string> BuyProduct(string id)
    {
        var repository = _provider.GetService(typeof(IRepository<Product>)) as IRepository<Product>;

        if (repository is null)
        {
            throw new ArgumentNullException(nameof(repository));
        }

        var product = await repository.GetById(id);

        if (product is null || product.Count <= 0)
        {
            return TransferStatus.Failure;
        }

        product.Count--;

        await repository.Update(product);

        return TransferStatus.Success;
    }
}
