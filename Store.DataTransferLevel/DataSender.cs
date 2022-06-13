using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Store.BLL.Interfaces;
using Store.DataTransferLevel.Settings;
using System.Collections.Concurrent;
using System.Text;

namespace Store.DataTransferLevel;

public class DataSender : IDataSender, IDisposable
{
    private IConnection connection;
    private IModel channel;
    private string replyQueueName;
    private EventingBasicConsumer consumer;
    private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
    private IBasicProperties props;
    private readonly RabbitSettings _settings;

    public DataSender(RabbitSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        Connect();
    }

    protected virtual void Connect()
    {
        var factory = new ConnectionFactory() { HostName = _settings.HostName, Port = _settings.Port, UserName = _settings.UserName, Password = _settings.Password };

        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        replyQueueName = channel.QueueDeclare().QueueName;
        consumer = new EventingBasicConsumer(channel);

        props = channel.CreateBasicProperties();
        var correlationId = Guid.NewGuid().ToString();
        props.CorrelationId = correlationId;
        props.ReplyTo = replyQueueName;

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var response = Encoding.UTF8.GetString(body);
            if (ea.BasicProperties.CorrelationId == correlationId)
            {
                respQueue.Add(response);
            }
        };

        channel.BasicConsume(
            consumer: consumer,
            queue: replyQueueName,
            autoAck: true);
    }

    public string Call(string data)
    {
        var messageBytes = Encoding.UTF8.GetBytes(data);
        channel.BasicPublish(
            exchange: "",
            routingKey: _settings.RoutingKey,
            basicProperties: props,
            body: messageBytes);

        return respQueue.Take();
    }

    public void Dispose()
    {
        connection.Close();
    }
}
