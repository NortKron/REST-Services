ASP.NET Core API сервис

Два взаимодейтвующих между собой REST-сервиса, реализующие приём заявки на кредит в формате JSON и получение статуса раасмотрения заявки

1-й сервис принимает заявку на кредит в формате JSON, сохраняет её в БД на SQL Server (подключение через ConnectionString). Далее обращается ко второму сервису, который возвращает
true / false, после чего 1-й сервис меняет статус заявки в БД
Присутствует подобие Unit-тестов