# RecipeFinder

**Факултетен номер:** 2301321020  
**Име на проекта:** RecipeFinder  

## Описание:
RecipeFinder е ASP.NET Core проект за управление на потребителски профили и рецепти. Състои се от:
- ASP.NET Core Web API за работа с рецепти, съставки, категории и потребители.
- ASP.NET Core MVC клиент, който консумира API-то и предоставя потребителски интерфейс.

### Роли:
- 👤 **Администратор:** Управлява потребители, категории и съставки.
- 🙋‍♂️ **Обикновен потребител:** Може да:
  - Добавя, редактира и изтрива свои рецепти.
  - Търси рецепти по категории и съставки.
  - Добавя рецепти в "Любими".

---

## Съдържание:
- `RecipeFinder.API` – ASP.NET Core Web API.
- `RecipeFinder.MVC` – ASP.NET Core MVC приложение, което използва API-то.

---
## Инсталация и стартиране

### Изисквания:
- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download)

### Стъпки:
1. Клониране на repository:
  ```bash
   git clone https://github.com/TedyIsBack/RecipeFinder_distributed-applications-se.git
   cd RecipeFinder_distributed-applications-se/course-work/implementations/RecipeFinderAPI
  ```

2. Стартиране на API проекта:
    ```bash
    cd RecipeFinder.API
    dotnet run
    ```

3. Стартиране на MVC приложението:
    Отворете нов терминал:
    ```bash
    cd RecipeFinder.MVC
    dotnet run
    ```

4. Отваряне на браузъра на следните адреси:

🔹 **API (RecipeFinder.API)**  
- HTTP: `http://localhost:5109`  
- Swagger: `https://localhost:7094/swagger` *(автоматично се отваря при стартиране)*

🔸 **MVC клиент (RecipeFinder.MVC)**  
- HTTPS: `https://localhost:7227`  *(При стартиране браузърът се отваря автоматично към тази страница.)**

