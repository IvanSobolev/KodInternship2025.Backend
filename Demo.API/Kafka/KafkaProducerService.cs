using System.Net.Http.Headers;

namespace Demo.Kafka;

using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;

public class KafkaProducerConfig
{
    public string BootstrapServers { get; set; }
    public string NewTaskTopic { get; set; }
}

public class NewTaskMessage
{
    public Guid TaskId { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public string Department { get; set; }
    public List<long> RecipientTelegramIds { get; set; }
}

public interface IKafkaProducerService
{
    Task ProduceNewTaskMessageAsync(NewTaskMessage message);
}

public class KafkaProducerService : IKafkaProducerService, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _topicName;

    public KafkaProducerService(IOptions<KafkaProducerConfig> config, ILogger<KafkaProducerService> logger)
    {
        var producerConfig = new ProducerConfig { BootstrapServers = config.Value.BootstrapServers };
        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        _topicName = config.Value.NewTaskTopic;
    }

    public async Task ProduceNewTaskMessageAsync(NewTaskMessage message)
    {
        if (message == null || message.RecipientTelegramIds == null || !message.RecipientTelegramIds.Any())
        {
            Console.WriteLine("Сообщение для Kafka не содержит получателей или является null. Отправка отменена.");
            return;
        }

        try
        {
            var serializedMessage = JsonSerializer.Serialize(message);
            var deliveryResult = await _producer.ProduceAsync(_topicName, new Message<Null, string> { Value = serializedMessage });
            Console.WriteLine($"Сообщение об уведомлении для задачи {message.TaskId} доставлено в Kafka: {deliveryResult.Status}");
        }
        catch (ProduceException<Null, string> e)
        {
            Console.WriteLine($"Ошибка доставки сообщения в Kafka: {e.Error.Reason}");
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
        GC.SuppressFinalize(this);
    }
}