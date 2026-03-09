namespace Travel_agency.Core.Enums
{
    public enum Status
    {
        Pending,    // Очікує підтвердження
        Confirmed,  // Підтверджено
        Cancelled,  // Скасовано користувачем
        Rejected,   // Відхилено оператором
        Paid,       // Оплачено
        Completed   // Завершено
    }
}
