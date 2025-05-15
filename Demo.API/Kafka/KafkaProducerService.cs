using System.Net.Http.Headers;

namespace Demo.Kafka;

using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;

public class KafkaProducerConfig
{
    public string BootstrapServers { get; set; }
    public string NewTaskTopic { get; set; } // Для сообщений о новых задачах
    public string SimpleNotificationTopic { get; set; } 
}

public class SimpleNotificationMessage
{
    public Guid CorrelationId { get; set; } // Для отслеживания или связывания с каким-либо событием
    public string MessageType { get; set; } // Тип уведомления, например, "Info", "Warning", "SystemAlert"
    public string Content { get; set; }     // Само текстовое сообщение
    public DateTime Timestamp { get; set; } // Время создания уведомления
    public List<long>? TargetUserTelegramIds { get; set; } // Опционально: кому предназначено, если это важно для консьюмера

    public SimpleNotificationMessage(string content, string messageType = "Info", List<long>? targetUserTelegramIds = null)
    {
        CorrelationId = Guid.NewGuid();
        MessageType = messageType;
        Content = content;
        Timestamp = DateTime.UtcNow;
        TargetUserTelegramIds = targetUserTelegramIds;
    }
}

public class NewTaskMessage
{
    public Guid TaskId { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public int Department { get; set; }
    public List<long> RecipientTelegramIds { get; set; }
}

public interface IKafkaProducerService
{
    Task ProduceNewTaskMessageAsync(NewTaskMessage message);
    Task ProduceSimpleNotificationAsync(SimpleNotificationMessage message);
}

public class KafkaProducerService : IKafkaProducerService, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _newTaskTopicName;
    private readonly string _simpleNotificationTopicName; // Для нового топика
    private readonly ILogger<KafkaProducerService> _logger; // Добавим логгер

    public KafkaProducerService(IOptions<KafkaProducerConfig> config, ILogger<KafkaProducerService> logger)
    {
        _logger = logger; // Сохраняем логгер
        var producerConfig = new ProducerConfig { BootstrapServers = config.Value.BootstrapServers };
        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        _newTaskTopicName = config.Value.NewTaskTopic;
        _simpleNotificationTopicName = config.Value.SimpleNotificationTopic; // Получаем имя нового топика

        if (string.IsNullOrEmpty(_newTaskTopicName))
        {
            _logger.LogError("NewTaskTopic не сконфигурирован.");
            // throw new ArgumentNullException(nameof(config.Value.NewTaskTopic), "NewTaskTopic не может быть пустым.");
        }
        if (string.IsNullOrEmpty(_simpleNotificationTopicName))
        {
            _logger.LogError("SimpleNotificationTopic не сконфигурирован.");
            // throw new ArgumentNullException(nameof(config.Value.SimpleNotificationTopic), "SimpleNotificationTopic не может быть пустым.");
        }
    }

    public async Task ProduceNewTaskMessageAsync(NewTaskMessage message)
    {
        if (string.IsNullOrEmpty(_newTaskTopicName))
        {
            _logger.LogError("Топик для новых задач не задан. Отправка NewTaskMessage отменена.");
            return;
        }

        if (message == null || message.RecipientTelegramIds == null || !message.RecipientTelegramIds.Any())
        {
            _logger.LogWarning("Сообщение NewTaskMessage для Kafka не содержит получателей или является null. Отправка отменена.");
            return;
        }

        try
        {
            var serializedMessage = JsonSerializer.Serialize(message);
            var deliveryResult = await _producer.ProduceAsync(_newTaskTopicName, new Message<Null, string> { Value = serializedMessage });
            _logger.LogInformation("Сообщение NewTaskMessage (Задача {TaskId}) доставлено в Kafka топик '{Topic}': {Status}",
                message.TaskId, _newTaskTopicName, deliveryResult.Status);
        }
        catch (ProduceException<Null, string> e)
        {
            _logger.LogError(e, "Ошибка доставки NewTaskMessage (Задача {TaskId}) в Kafka топик '{Topic}'",
                message.TaskId, _newTaskTopicName);
            throw; // Перебрасываем, чтобы вызывающий код знал об ошибке
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Непредвиденная ошибка при отправке NewTaskMessage (Задача {TaskId}) в Kafka топик '{Topic}'",
                message.TaskId, _newTaskTopicName);
            throw;
        }
    }

    // НОВЫЙ МЕТОД для отправки простых уведомлений
    public async Task ProduceSimpleNotificationAsync(SimpleNotificationMessage message)
    {
        if (string.IsNullOrEmpty(_simpleNotificationTopicName))
        {
            _logger.LogError("Топик для простых уведомлений не задан. Отправка SimpleNotificationMessage отменена.");
            return;
        }

        if (message == null || string.IsNullOrWhiteSpace(message.Content))
        {
            _logger.LogWarning("Сообщение SimpleNotificationMessage для Kafka не содержит контента или является null. Отправка отменена.");
            return;
        }

        try
        {
            var serializedMessage = JsonSerializer.Serialize(message);
            var deliveryResult = await _producer.ProduceAsync(_simpleNotificationTopicName, new Message<Null, string> { Value = serializedMessage });
            _logger.LogInformation("Сообщение SimpleNotificationMessage (ID: {CorrelationId}, Тип: {MessageType}) доставлено в Kafka топик '{Topic}': {Status}",
                message.CorrelationId, message.MessageType, _simpleNotificationTopicName, deliveryResult.Status);
        }
        catch (ProduceException<Null, string> e)
        {
            _logger.LogError(e, "Ошибка доставки SimpleNotificationMessage (ID: {CorrelationId}) в Kafka топик '{Topic}'",
                message.CorrelationId, _simpleNotificationTopicName);
            throw; // Перебрасываем, чтобы вызывающий код знал об ошибке
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Непредвиденная ошибка при отправке SimpleNotificationMessage (ID: {CorrelationId}) в Kafka топик '{Topic}'",
                message.CorrelationId, _simpleNotificationTopicName);
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Flush(TimeSpan.FromSeconds(10)); // Даем время на отправку оставшихся сообщений перед Dispose
        _producer?.Dispose();
        GC.SuppressFinalize(this);
    }
}