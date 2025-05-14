using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace  Demo.DAL.Models;

public enum Department // Отделы
{
    Frontend,
    Backend,
    Design

}

public enum ProjecTaskStatus // Стасус задания
{
    ToDo,             //  Нужно сделать
    InProgress,       // В работе
    PendingReview,    // Ожидает проверки (если работник завершил, но менеджер должен подтвердить)
    Completed         // Выполнено
}



