# 🏪 VendingMachines — Система управления торговыми автоматами

VendingMachines — это десктопное приложение для управления сетью торговых автоматов, разработанное на Avalonia UI с использованием реактивного программирования. Приложение позволяет управлять автоматами, компаниями-владельцами, пользователями системы, а также отслеживать статистику продаж и обслуживания.

## ✨ Возможности

### Дашборд
- Круговая диаграмма эффективности работы автоматов
- Диаграмма состояния сети (исправен/неисправен/на обслуживании)
- Графики суммы и количества продаж по дням
- Сводная информация: деньги в автоматах, выручка, инкассация
- Лента новостей компании

### Управление торговыми автоматами
- Просмотр списка автоматов с пагинацией (таблица и плитка)
- Добавление, редактирование и удаление автоматов
- Фильтрация по названию локации
- Привязка к модели, компании, месту установки, режиму работы
- Настройка часового пояса и приоритета обслуживания
- Управление способами оплаты
- Добавление заметок к автомату
- Экспорт списка в CSV

### Управление компаниями
- Просмотр списка компаний с пагинацией
- Добавление, редактирование и удаление компаний
- Привязка к вышестоящей компании (холдингу)
- Указание адреса, контактов, даты начала сотрудничества
- Защита от удаления компании с привязанными автоматами
- Поиск по названию, адресу, контактам
- Экспорт списка в CSV

### Управление пользователями
- Просмотр списка пользователей с пагинацией
- Добавление, редактирование и удаление пользователей
- Назначение ролей (администратор, менеджер, инженер, оператор)
- Валидация email и пароля при создании
- Защита от удаления пользователей с активностями
- Поиск по ФИО, email, телефону
- Экспорт списка в CSV

### Аутентификация и регистрация
- Вход в систему по email и паролю
- Регистрация нового пользователя с подтверждением email
- CAPTCHA для защиты от ботов
- Валидация сложности пароля (минимум 8 символов, цифра, спецсимвол)
- Отображение/скрытие пароля

## 🛠 Технологический стек

- **Фреймворк**: Avalonia UI
- **Язык**: C# 12, .NET 8.0
- **Реактивность**: ReactiveUI
- **База данных**: PostgreSQL (Entity Framework Core)
- **Графики**: LiveChartsCore.SkiaSharpView
- **Диалоговые окна**: MessageBox.Avalonia
- **Тестирование**: MSTest + Entity Framework Core InMemory
- **Кроссплатформенность**: Windows, macOS, Linux

## 📁 Структура проекта

```
VendingMachines/
├── Models/
│   ├── _43pKobzarContext.cs          — Контекст базы данных (EF Core)
│   ├── VendingMachine.cs             — Модель торгового автомата
│   ├── Company.cs                    — Модель компании
│   ├── Highcompany.cs                — Модель вышестоящей компании
│   ├── User.cs                       — Модель пользователя
│   ├── UserRole.cs                   — Модель роли пользователя
│   ├── Maintenance.cs                — Модель обслуживания
│   ├── Product.cs                    — Модель продукта
│   ├── Sale.cs                       — Модель продажи
│   ├── PaymentType.cs                — Модель типа оплаты
│   └── ...                           — Другие модели
├── ViewModels/
│   ├── MainWindowViewModel.cs        — Главная ViewModel
│   ├── MainPageViewModel.cs          — Дашборд с графиками
│   ├── AdminPageViewModel.cs         — Управление автоматами
│   ├── CompanyPageViewModel.cs       — Управление компаниями
│   ├── UsersPageViewModel.cs         — Управление пользователями
│   ├── AddEditMachinePageViewModel.cs — Форма автомата
│   ├── AddEditCompanyPageViewModel.cs — Форма компании
│   ├── AddEditUserPageViewModel.cs   — Форма пользователя
│   ├── AuthPageViewModel.cs          — Авторизация
│   └── RegPageViewModel.cs           — Регистрация
├── Views/
│   ├── MainWindow.axaml              — Главное окно
│   ├── MainPage.axaml                — Дашборд
│   ├── AdminPage.axaml               — Список автоматов
│   ├── CompanyPage.axaml             — Список компаний
│   ├── UsersPage.axaml               — Список пользователей
│   ├── AddEditMachinePage.axaml      — Форма автомата
│   ├── AddEditCompanyPage.axaml      — Форма компании
│   ├── AddEditUserPage.axaml         — Форма пользователя
│   ├── AuthPage.axaml                — Страница входа
│   └── RegPage.axaml                 — Страница регистрации
└── Tests/
    └── AdminPageViewModelTests.cs    — Модульные тесты
```

## 🧪 Тестирование

Проект содержит обширный набор модульных тестов, написанных с использованием **MSTest** и **Entity Framework Core InMemory Database**.

### Архитектура тестов

#### Базовый класс TestBase

Абстрактный класс `TestBase` предоставляет общую функциональность для всех тестов:

- **CreateInMemoryContext()** — создаёт новый экземпляр контекста БД в памяти с уникальным именем для каждого теста
- **SeedVendingMachines()** — заполняет БД тестовым торговым автоматом
- **SeedCompanies()** — заполняет БД тестовыми компаниями
- **SeedHighCompanies()** — заполняет БД тестовыми холдингами
- **SeedUsers()** — заполняет БД тестовыми пользователями

```csharp
protected _43pKobzarContext CreateInMemoryContext()
{
    var options = new DbContextOptionsBuilder<_43pKobzarContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;
    return new _43pKobzarContext(options);
}
```

Использование `Guid.NewGuid().ToString()` гарантирует изоляцию тестов друг от друга.

### Тесты AdminPageViewModel

Тесты для страницы управления торговыми автоматами:

| Тест | Описание |
|------|----------|
| `LoadVendingMachines_ShouldPopulatePagedMachines` | Проверяет загрузку автоматов после инициализации |
| `FilterText_FiltersByName` | Проверяет фильтрацию по названию локации |
| `PageSize_Change_ResetsCurrentPage` | Проверяет сброс страницы при изменении размера |
| `DeleteMachine_RemovesMachine` | Проверяет удаление автомата из БД |

### Тесты CompanyPageViewModel

Тесты для страницы управления компаниями:

| Тест | Описание |
|------|----------|
| `LoadCompanies_IncludesHighCompany` | Проверяет загрузку компаний с вышестоящими |
| `SearchText_FiltersByName` | Проверяет поиск по названию компании |
| `DeleteCompany_WithNoVendingMachines_RemovesCompany` | Проверяет успешное удаление компании без автоматов |
| `DeleteCompany_WithVendingMachines_DoesNotRemove` | Проверяет защиту от удаления компании с автоматами |

### Тесты UsersPageViewModel

Тесты для страницы управления пользователями:

| Тест | Описание |
|------|----------|
| `LoadUsers_IncludesRole` | Проверяет загрузку пользователей с ролями |
| `SearchText_FiltersByName` | Проверяет поиск по имени пользователя |
| `DeleteUser_WithNoRelated_RemovesUser` | Проверяет успешное удаление пользователя без активностей |
| `DeleteUser_WithMaintenances_DoesNotRemove` | Проверяет защиту от удаления с обслуживаниями |

### Тесты AddEditUserPageViewModel

Тесты для формы создания/редактирования пользователя:

| Тест | Описание |
|------|----------|
| `Constructor_LoadsRoles` | Проверяет загрузку списка ролей |
| `Save_WhenEmailEmpty_DoesNotAddUser` | Проверяет валидацию пустого email |
| `Save_WhenAllValid_AddsUser` | Проверяет успешное создание пользователя |
| `Save_EditUser_UpdatesDatabase` | Проверяет обновление существующего пользователя |

### Тесты AddEditCompanyPageViewModel

Тесты для формы создания/редактирования компании:

| Тест | Описание |
|------|----------|
| `Constructor_LoadsHighCompanies` | Проверяет загрузку вышестоящих компаний |
| `Save_WhenNameEmpty_DoesNotAddCompany` | Проверяет валидацию пустого названия |
| `Save_NewCompany_AddsToDatabase` | Проверяет успешное создание компании |

### Тесты AddEditMachinePageViewModel

Тесты для формы создания/редактирования автомата:

| Тест | Описание |
|------|----------|
| `LoadLookups_PopulatesAllCollections` | Проверяет загрузку всех справочников |
| `Save_WhenAllRequiredSelected_AddsMachine` | Проверяет создание автомата со всеми обязательными полями |

### Особенности тестирования

1. **Изоляция тестов**: Каждый тест использует собственную InMemory базу данных
2. **SkipConfirmation**: ViewModel поддерживают флаг `skipConfirmation` для пропуска диалоговых окон в тестах
3. **Асинхронные тесты**: Использование `ToTask()` для конвертации ReactiveUI команд в Task
4. **Reflection в тестах**: Доступ к приватным методам через `GetMethod().Invoke()` для тестирования фильтрации
5. **Проверка граничных условий**: Тесты покрывают как успешные сценарии, так и ошибки валидации

### Запуск тестов

```
dotnet test
```

Или через Test Explorer в Visual Studio / Rider.

## 🚀 Начало работы

### Предварительные требования

- .NET 8.0 SDK или выше
- PostgreSQL (для production) или автоматическая InMemory БД для тестов
- Avalonia UI

### Установка

1. Клонируйте репозиторий

```
git clone https://github.com/your-username/VendingMachines.git
cd VendingMachines
```

2. Восстановите зависимости

```
dotnet restore
```

3. Настройте подключение к базе данных

Отредактируйте строку подключения в `Models/_43pKobzarContext.cs`:

```csharp
optionsBuilder.UseNpgsql("Host=ваш_хост;Port=5432;Database=ваша_бд;Username=ваш_пользователь;Password=ваш_пароль");
```

4. Запустите приложение

```
dotnet run
```

## 🗄️ Схема базы данных

База данных содержит таблицы в схеме `EducationalPractice`:

| Таблица | Описание |
|---------|----------|
| vending_machines | Торговые автоматы |
| company | Компании-владельцы |
| highcompany | Вышестоящие компании (холдинги) |
| company_location | Локации размещения |
| model | Модели автоматов |
| users | Пользователи системы |
| user_role | Роли пользователей |
| maintenance | Обслуживание автоматов |
| products | Продукты в автоматах |
| sales | Продажи |
| payment_type | Типы оплаты |
| payment_method | Способы оплаты |
| vending_machine_payment | Связь автоматов с типами оплаты |
| work_mode | Режимы работы |
| service_priority | Приоритеты обслуживания |
| timezone | Часовые пояса |
| place | Места установки |
| status | Статусы автоматов |
| notes_vending | Заметки к автоматам |

## 📊 Дашборд

Главная страница отображает:

- **Эффективность**: Круговая диаграмма "Работают / Не работают"
- **Состояние сети**: Диаграмма "Исправен / Неисправен / На обслуживании"
- **Графики продаж**: Сумма и количество продаж по дням (линейные графики)
- **Сводка**: Деньги в автоматах, выручка, инкассация
- **Новости**: Лента новостей компании

## 🔐 Безопасность

- Пароли хешируются перед сохранением
- Валидация сложности пароля при регистрации (минимум 8 символов, цифра, спецсимвол)
- CAPTCHA для защиты от автоматической регистрации
- Подтверждение email через код

## 📦 Зависимости

Основные NuGet пакеты:

```xml
<PackageReference Include="Avalonia" Version="11.0.0" />
<PackageReference Include="Avalonia.Desktop" Version="11.0.0" />
<PackageReference Include="Avalonia.ReactiveUI" Version="11.0.0" />
<PackageReference Include="ReactiveUI" Version="19.5.0" />
<PackageReference Include="LiveChartsCore.SkiaSharpView.Avalonia" Version="2.0.0-rc2" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />
<PackageReference Include="MessageBox.Avalonia" Version="3.1.5" />
<PackageReference Include="MSTest.TestFramework" Version="3.1.0" />
```

## 🤝 Участие в разработке

Мы приветствуем ваш вклад в проект!

1. Сделайте форк репозитория
2. Создайте ветку для вашей функции (`git checkout -b feature/новая-фича`)
3. Зафиксируйте изменения (`git commit -m 'Добавил новую фичу'`)
4. Убедитесь, что все тесты проходят (`dotnet test`)
5. Отправьте изменения в ветку (`git push origin feature/новая-фича`)
6. Откройте Pull Request

### Требования к тестам

- Все новые ViewModel должны иметь модульные тесты
- Тесты должны покрывать успешные и ошибочные сценарии
- Используйте InMemory Database для изоляции тестов
- Наследуйте тестовые классы от `TestBase` для переиспользования хелперов
