﻿namespace Demo.DAL.Enums;

public enum TaskStatus
{
    ToDo,             // Нужно сделать
    InProgress,       // В работе
    PendingReview,    // Ожидает проверки (если работник завершил, но менеджер должен подтвердить)
    Completed         // Выполнено
}