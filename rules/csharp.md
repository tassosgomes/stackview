# Instruções para Agentes de IA: Desenvolvimento .NET C# e ASP.NET Core

## Índice
1. [Princípios Fundamentais](#princípios-fundamentais)
2. [Convenções de Nomenclatura](#convenções-de-nomenclatura)
3. [Estilo de Código](#estilo-de-código)
4. [Melhores Práticas de Programação](#melhores-práticas-de-programação)
5. [Bibliotecas Recomendadas](#bibliotecas-recomendadas)
6. [Testes Unitários](#testes-unitários)
7. [Testes de Integração](#testes-de-integração)
8. [Testes End-to-End (E2E)](#testes-end-to-end-e2e)
9. [CancellationToken - Boas Práticas](#cancellationtoken---boas-práticas)
10. [Padrões de Arquitetura](#padrões-de-arquitetura)
11. [Tratamento de Erros](#tratamento-de-erros)
12. [Performance e Otimização](#performance-e-otimização)

---

## Princípios Fundamentais

### Objetivos de Qualidade de Código
- **Legibilidade**: Código deve ser auto-explicativo e fácil de entender
- **Manutenibilidade**: Facilitar modificações e extensões futuras
- **Testabilidade**: Código deve ser facilmente testável
- **Performance**: Otimizar quando necessário, sem sacrificar legibilidade
- **Consistência**: Seguir padrões estabelecidos em todo o projeto

### Diretrizes Gerais
- Utilize recursos modernos da linguagem C# sempre que possível
- Evite construções obsoletas
- Prefira clareza sobre brevidade
- Escreva código pensando em quem irá mantê-lo no futuro

---

## Convenções de Nomenclatura

### Classes e Interfaces
```csharp
// ✅ Correto
public class ProductService { }
public interface IProductRepository { }
public record Customer(string FirstName, string LastName);

// ❌ Incorreto
public class productService { }
public interface ProductRepository { }
```

### Métodos e Propriedades
```csharp
// ✅ Correto - PascalCase
public string GetFullName() { }
public int TotalAmount { get; set; }
public async Task<User> GetUserByIdAsync(int userId) { }

// ❌ Incorreto
public string getFullName() { }
public int total_amount { get; set; }
```

### Variáveis e Parâmetros
```csharp
// ✅ Correto - camelCase
public void ProcessOrder(string customerName, int orderId)
{
    var totalAmount = CalculateTotal();
    var isValidOrder = ValidateOrder(orderId);
}

// ❌ Incorreto
public void ProcessOrder(string CustomerName, int OrderId)
{
    var TotalAmount = CalculateTotal();
    var is_valid_order = ValidateOrder(OrderId);
}
```

### Campos Privados
```csharp
// ✅ Correto - underscore prefix com camelCase
private readonly ILogger _logger;
private static readonly string _connectionString;

// ❌ Incorreto
private readonly ILogger logger;
private static readonly string ConnectionString;
```

### Constantes
```csharp
// ✅ Correto - PascalCase
public const int MaxRetryAttempts = 3;
private const string DefaultCulture = "pt-BR";

// ❌ Incorreto
public const int MAX_RETRY_ATTEMPTS = 3;
private const string default_culture = "pt-BR";
```

---

## Estilo de Código

### Formatação e Layout
```csharp
// ✅ Correto - Estilo Allman para chaves
public class OrderService
{
    public async Task<Order> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var order = new Order
        {
            CustomerId = request.CustomerId,
            Items = request.Items,
            CreatedAt = DateTime.UtcNow
        };

        return await _repository.SaveAsync(order, cancellationToken);
    }
}
```

### Uso de var
```csharp
// ✅ Correto - tipo claro pelo contexto
var customer = new Customer();
var orders = await _repository.GetOrdersAsync();

// ❌ Incorreto - tipo não claro
var result = ProcessData();
var count = GetCount();

// ✅ Melhor alternativa quando tipo não é claro
Customer result = ProcessData();
int count = GetCount();
```

### String Interpolation
```csharp
// ✅ Correto
string message = $"Order {orderId} created for customer {customerName}";

// ✅ Para strings longas, use raw string literals
var sqlQuery = """
    SELECT o.Id, o.Total, c.Name
    FROM Orders o
    INNER JOIN Customers c ON o.CustomerId = c.Id
    WHERE o.CreatedAt >= @startDate
    """;

// ❌ Incorreto
string message = "Order " + orderId + " created for customer " + customerName;
```

### Inicialização de Collections
```csharp
// ✅ Correto - Collection expressions (C# 12+)
string[] languages = ["C#", "Python", "JavaScript"];
List<int> numbers = [1, 2, 3, 4, 5];

// ✅ Alternativa para versões anteriores
var languages = new[] { "C#", "Python", "JavaScript" };
var numbers = new List<int> { 1, 2, 3, 4, 5 };
```

---

## Melhores Práticas de Programação

### Async/Await
```csharp
// ✅ Correto
public async Task<User> GetUserAsync(int id, CancellationToken cancellationToken)
{
    var user = await _repository.GetByIdAsync(id, cancellationToken);
    return user ?? throw new UserNotFoundException($"User {id} not found");
}

// ✅ ConfigureAwait(false) em bibliotecas
public async Task<string> GetDataAsync()
{
    var response = await _httpClient.GetAsync("api/data").ConfigureAwait(false);
    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
}

// ❌ Incorreto - bloqueio síncrono
public User GetUser(int id)
{
    return _repository.GetByIdAsync(id).Result; // Pode causar deadlock
}
```

### Dependency Injection
```csharp
// ✅ Correto - Constructor injection
public class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<OrderService> _logger;
    private readonly IEmailService _emailService;

    public OrderService(
        IOrderRepository repository,
        ILogger<OrderService> logger,
        IEmailService emailService)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }
}
```

### SOLID Principles
```csharp
// ✅ Single Responsibility Principle
public class EmailValidator
{
    public bool IsValid(string email)
    {
        return !string.IsNullOrEmpty(email) && email.Contains("@");
    }
}

public class UserService
{
    private readonly IUserRepository _repository;
    private readonly EmailValidator _emailValidator;

    public async Task<User> CreateUserAsync(CreateUserRequest request)
    {
        if (!_emailValidator.IsValid(request.Email))
        {
            throw new ArgumentException("Invalid email format");
        }

        // Lógica de criação do usuário
    }
}
```

### Exception Handling
```csharp
// ✅ Correto - Exceções específicas
public async Task<User> GetUserAsync(int id)
{
    try
    {
        var user = await _repository.GetByIdAsync(id);
        return user ?? throw new UserNotFoundException($"User with ID {id} not found");
    }
    catch (SqlException ex) when (ex.Number == 2) // Timeout
    {
        _logger.LogWarning("Database timeout while getting user {UserId}", id);
        throw new ServiceUnavailableException("Database is temporarily unavailable", ex);
    }
}

// ❌ Incorreto - Exception genérica
public async Task<User> GetUserAsync(int id)
{
    try
    {
        return await _repository.GetByIdAsync(id);
    }
    catch (Exception ex)
    {
        throw; // Não adiciona valor
    }
}
```

---

## Bibliotecas Recomendadas

### Core Libraries
```xml
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Options" Version="8.0.0" />
```

### Web Development (ASP.NET Core)
```xml
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
<PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />
```

### Database
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />
<PackageReference Include="Dapper" Version="2.1.28" />
```

### HTTP Client
```xml
<PackageReference Include="Microsoft.Extensions.Http" Version="8.0.0" />
<PackageReference Include="RestSharp" Version="110.2.0" />
<PackageReference Include="System.Net.Http.Json" Version="8.0.0" />
```

### Serialization
```xml
<PackageReference Include="System.Text.Json" Version="8.0.0" />
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
```

### Logging
```xml
<PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
<PackageReference Include="Serilog.Sinks.Console" Version="5.0.1" />
<PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
```

### Resilience
```xml
<PackageReference Include="Polly" Version="8.2.0" />
<PackageReference Include="Polly.Extensions.Http" Version="3.0.0" />
```

### Utilities
```xml
<PackageReference Include="FluentValidation" Version="11.8.1" />
<PackageReference Include="Mapster" Version="7.4.0" />
<PackageReference Include="MediatR" Version="12.2.0" />
<PackageReference Include="AutoMapper" Version="12.0.1" />
```

---

## Testes Unitários

### Framework Recomendado: xUnit
```xml
<PackageReference Include="xunit" Version="2.6.6" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.5.6" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
<PackageReference Include="Moq" Version="4.20.70" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="AutoFixture" Version="4.18.1" />
```

### Estrutura de Teste - AAA Pattern
```csharp
public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _repositoryMock;
    private readonly Mock<ILogger<OrderService>> _loggerMock;
    private readonly OrderService _sut; // System Under Test

    public OrderServiceTests()
    {
        _repositoryMock = new Mock<IOrderRepository>();
        _loggerMock = new Mock<ILogger<OrderService>>();
        _sut = new OrderService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateOrderAsync_WithValidRequest_ShouldReturnCreatedOrder()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = 1,
            Items = new[] { new OrderItem { ProductId = 1, Quantity = 2 } }
        };
        
        var expectedOrder = new Order { Id = 123, CustomerId = 1 };
        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await _sut.CreateOrderAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(123);
        result.CustomerId.Should().Be(1);
        
        _repositoryMock.Verify(
            r => r.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task CreateOrderAsync_WithInvalidCustomerName_ShouldThrowArgumentException(string customerName)
    {
        // Arrange
        var request = new CreateOrderRequest { CustomerName = customerName };

        // Act & Assert
        await _sut.Invoking(s => s.CreateOrderAsync(request, CancellationToken.None))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Customer name cannot be null or empty");
    }
}
```

### Naming Convention para Testes
```csharp
// Padrão: MethodName_StateUnderTest_ExpectedBehavior
[Fact]
public void CalculateDiscount_WithPremiumCustomer_ShouldApply20PercentDiscount()

[Fact]
public void ValidateEmail_WithInvalidFormat_ShouldReturnFalse()

[Fact]
public async Task GetUserAsync_WhenUserNotFound_ShouldThrowUserNotFoundException()
```

### Testes Parametrizados
```csharp
[Theory]
[InlineData("admin@test.com", true)]
[InlineData("user@company.org", true)]
[InlineData("invalid-email", false)]
[InlineData("", false)]
[InlineData(null, false)]
public void IsValidEmail_WithVariousInputs_ShouldReturnExpectedResult(string email, bool expected)
{
    // Arrange & Act
    var result = EmailValidator.IsValid(email);

    // Assert
    result.Should().Be(expected);
}

[Theory]
[MemberData(nameof(GetOrderTestData))]
public void CalculateTotal_WithDifferentOrders_ShouldReturnCorrectTotal(Order order, decimal expectedTotal)
{
    // Arrange & Act
    var total = _calculator.CalculateTotal(order);

    // Assert
    total.Should().Be(expectedTotal);
}

public static IEnumerable<object[]> GetOrderTestData()
{
    yield return new object[] { new Order { Items = [] }, 0m };
    yield return new object[] { new Order { Items = [new() { Price = 10m, Quantity = 2 }] }, 20m };
}
```

---

## Testes de Integração

### Framework e Setup
```xml
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />
<PackageReference Include="Testcontainers" Version="3.7.0" />
```

### WebApplicationFactory Customizada
```csharp
public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithDatabase("testdb")
        .WithUsername("testuser")
        .WithPassword("testpass")
        .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove o DbContext original
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Adiciona o DbContext de teste
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });

            // Seed data de teste
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();
            SeedTestData(context);
        });
    }

    private static void SeedTestData(ApplicationDbContext context)
    {
        context.Users.AddRange(
            new User { Id = 1, Name = "Test User", Email = "test@example.com" },
            new User { Id = 2, Name = "Admin User", Email = "admin@example.com" }
        );
        context.SaveChanges();
    }
}
```

### Testes de API
```csharp
[Collection("IntegrationTests")]
public class UsersControllerTests : IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UsersControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetUsers_ShouldReturnAllUsers()
    {
        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var users = await response.Content.ReadFromJsonAsync<List<User>>();
        users.Should().HaveCount(2);
        users.Should().Contain(u => u.Name == "Test User");
    }

    [Fact]
    public async Task CreateUser_WithValidData_ShouldReturnCreatedUser()
    {
        // Arrange
        var newUser = new CreateUserRequest
        {
            Name = "New User",
            Email = "newuser@example.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/users", newUser);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdUser = await response.Content.ReadFromJsonAsync<User>();
        createdUser.Should().NotBeNull();
        createdUser!.Name.Should().Be("New User");
        createdUser.Email.Should().Be("newuser@example.com");
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => Task.CompletedTask;
}

[CollectionDefinition("IntegrationTests")]
public class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory>
{
}
```

---

## Testes End-to-End (E2E)

### Framework Recomendado: Playwright
```xml
<PackageReference Include="Microsoft.Playwright" Version="1.41.0" />
<PackageReference Include="Microsoft.Playwright.NUnit" Version="1.41.0" />
```

### Setup e Configuração
```csharp
[TestFixture]
public class E2ETests : PageTest
{
    private IWebHost? _webHost;
    private string _baseUrl = "http://localhost:5000";

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        // Instalar browsers necessários
        Microsoft.Playwright.Program.Main(new[] { "install" });
        
        // Iniciar aplicação
        _webHost = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls(_baseUrl);
            })
            .Build();

        await _webHost.StartAsync();
    }

    [OneTimeTearDown]
    public async Task GlobalTeardown()
    {
        if (_webHost != null)
        {
            await _webHost.StopAsync();
            _webHost.Dispose();
        }
    }
}
```

### Testes de Interface
```csharp
public class LoginTests : PageTest
{
    [Test]
    public async Task Login_WithValidCredentials_ShouldRedirectToDashboard()
    {
        // Arrange
        await Page.GotoAsync($"{_baseUrl}/login");

        // Act
        await Page.FillAsync("#email", "admin@example.com");
        await Page.FillAsync("#password", "password123");
        await Page.ClickAsync("#login-button");

        // Assert
        await Expect(Page).ToHaveURLAsync($"{_baseUrl}/dashboard");
        await Expect(Page.Locator(".welcome-message")).ToContainTextAsync("Welcome, Admin!");
    }

    [Test]
    public async Task Login_WithInvalidCredentials_ShouldShowErrorMessage()
    {
        // Arrange
        await Page.GotoAsync($"{_baseUrl}/login");

        // Act
        await Page.FillAsync("#email", "invalid@example.com");
        await Page.FillAsync("#password", "wrongpassword");
        await Page.ClickAsync("#login-button");

        // Assert
        await Expect(Page.Locator(".error-message")).ToContainTextAsync("Invalid credentials");
        await Expect(Page).ToHaveURLAsync($"{_baseUrl}/login");
    }
}
```

### Page Object Model
```csharp
public class LoginPage
{
    private readonly IPage _page;

    public LoginPage(IPage page)
    {
        _page = page;
    }

    public async Task NavigateAsync()
    {
        await _page.GotoAsync("/login");
    }

    public async Task LoginAsync(string email, string password)
    {
        await _page.FillAsync("#email", email);
        await _page.FillAsync("#password", password);
        await _page.ClickAsync("#login-button");
    }

    public async Task<bool> IsErrorMessageVisibleAsync()
    {
        return await _page.Locator(".error-message").IsVisibleAsync();
    }

    public async Task<string> GetErrorMessageAsync()
    {
        return await _page.Locator(".error-message").TextContentAsync() ?? "";
    }
}

// Uso nos testes
[Test]
public async Task Login_WithInvalidCredentials_ShouldShowError()
{
    var loginPage = new LoginPage(Page);
    
    await loginPage.NavigateAsync();
    await loginPage.LoginAsync("invalid@test.com", "wrong");
    
    (await loginPage.IsErrorMessageVisibleAsync()).Should().BeTrue();
    (await loginPage.GetErrorMessageAsync()).Should().Contain("Invalid credentials");
}
```

---

## CancellationToken - Boas Práticas

### 1. Tornar Opcional em APIs Públicas, Obrigatório Internamente
```csharp
// ✅ API pública - opcional
public async Task<User> GetUserAsync(int id, CancellationToken cancellationToken = default)
{
    return await GetUserInternalAsync(id, cancellationToken);
}

// ✅ Método interno - obrigatório
private async Task<User> GetUserInternalAsync(int id, CancellationToken cancellationToken)
{
    cancellationToken.ThrowIfCancellationRequested();
    return await _repository.GetByIdAsync(id, cancellationToken);
}
```

### 2. Evitar Cancelamento Após Side Effects
```csharp
public async Task<Order> ProcessOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken)
{
    // ✅ Pode ser cancelado até aqui
    cancellationToken.ThrowIfCancellationRequested();
    
    var order = await _repository.CreateOrderAsync(request, cancellationToken);
    
    // ❌ Não cancele após salvar no banco
    // A partir daqui use CancellationToken.None para operações não críticas
    await _emailService.SendConfirmationAsync(order.CustomerEmail, CancellationToken.None);
    
    return order;
}
```

### 3. Usar CancellationToken.None Após o Ponto de "Não Cancelamento"
```csharp
public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request, CancellationToken cancellationToken)
{
    // Validações podem ser canceladas
    cancellationToken.ThrowIfCancellationRequested();
    await ValidatePaymentAsync(request, cancellationToken);
    
    // Processamento do pagamento - ponto crítico
    var result = await _paymentGateway.ProcessAsync(request, cancellationToken);
    
    // Após processamento bem-sucedido, não cancele operações secundárias
    await _auditService.LogPaymentAsync(result, CancellationToken.None);
    await _notificationService.NotifyAsync(request.CustomerId, CancellationToken.None);
    
    return result;
}
```

### 4. Verificar CanBeCanceled para Otimizações
```csharp
public async Task<string> ProcessLargeDataAsync(byte[] data, CancellationToken cancellationToken)
{
    if (!cancellationToken.CanBeCanceled)
    {
        // Usar implementação otimizada quando não há possibilidade de cancelamento
        return await ProcessDataOptimizedAsync(data);
    }
    
    // Implementação com verificações de cancelamento
    return await ProcessDataWithCancellationChecksAsync(data, cancellationToken);
}
```

### 5. Ignorar CancellationToken para Operações Rápidas
```csharp
public async Task<ValidationResult> ValidateAsync(string input, CancellationToken cancellationToken)
{
    // Para operações muito rápidas, não é necessário verificar cancelamento
    if (string.IsNullOrEmpty(input))
        return ValidationResult.Invalid("Input cannot be empty");
    
    // Apenas para operações mais longas
    if (input.Length > 1000)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await ValidateComplexInputAsync(input, cancellationToken);
    }
    
    return ValidationResult.Valid();
}
```

### 6. Propagação Correta em Métodos Assíncronos
```csharp
public async Task<IEnumerable<Product>> GetProductsAsync(int categoryId, CancellationToken cancellationToken)
{
    // ✅ Propagar o token para todos os métodos que o suportam
    var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
    var products = await _productRepository.GetByCategoryAsync(categoryId, cancellationToken);
    
    // ✅ Para processamento local rápido, verificação manual
    var result = new List<Product>();
    foreach (var product in products)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (await IsProductAvailableAsync(product.Id, cancellationToken))
        {
            result.Add(product);
        }
    }
    
    return result;
}
```

### 7. Timeout com CancellationToken
```csharp
public async Task<string> CallExternalServiceAsync(CancellationToken cancellationToken)
{
    using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
    using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(
        cancellationToken, timeoutCts.Token);
    
    try
    {
        return await _httpClient.GetStringAsync("https://api.external.com/data", 
            combinedCts.Token);
    }
    catch (OperationCanceledException) when (timeoutCts.Token.IsCancellationRequested)
    {
        throw new TimeoutException("External service call timed out");
    }
}
```

---

## Padrões de Arquitetura

### Clean Architecture
```csharp
// Domain Layer
public class Order
{
    public int Id { get; private set; }
    public string CustomerEmail { get; private set; }
    public List<OrderItem> Items { get; private set; } = new();
    public OrderStatus Status { get; private set; }
    
    public void AddItem(int productId, int quantity, decimal price)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Cannot modify confirmed order");
            
        Items.Add(new OrderItem(productId, quantity, price));
    }
}

// Application Layer
public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderResponse>
{
    private readonly IOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<OrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order(request.CustomerEmail);
        
        foreach (var item in request.Items)
        {
            order.AddItem(item.ProductId, item.Quantity, item.Price);
        }
        
        await _repository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new OrderResponse(order.Id, order.CustomerEmail);
    }
}
```

### Repository Pattern
```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly DbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }
}
```

### CQRS com MediatR
```csharp
// Command
public record CreateUserCommand(string Name, string Email) : IRequest<CreateUserResponse>;

public record CreateUserResponse(int Id, string Name, string Email);

// Command Handler
public class CreateUserHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly IUserRepository _repository;
    private readonly IValidator<CreateUserCommand> _validator;

    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);
        
        var user = new User(request.Name, request.Email);
        await _repository.AddAsync(user, cancellationToken);
        
        return new CreateUserResponse(user.Id, user.Name, user.Email);
    }
}

// Query
public record GetUserQuery(int Id) : IRequest<GetUserResponse>;

public record GetUserResponse(int Id, string Name, string Email);

// Query Handler
public class GetUserHandler : IRequestHandler<GetUserQuery, GetUserResponse>
{
    private readonly IUserRepository _repository;

    public async Task<GetUserResponse> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return user == null 
            ? throw new UserNotFoundException($"User {request.Id} not found")
            : new GetUserResponse(user.Id, user.Name, user.Email);
    }
}
```

---

## Tratamento de Erros

### Global Exception Handler (ASP.NET Core 8+)
```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            ValidationException ex => (400, "Validation Error", ex.Message),
            NotFoundException ex => (404, "Resource Not Found", ex.Message),
            UnauthorizedAccessException => (401, "Unauthorized", "Authentication required"),
            ArgumentNullException ex => (400, "Bad Request", $"Required parameter {ex.ParamName} is missing"),
            _ => (500, "Internal Server Error", "An unexpected error occurred")
        };

        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}

// Registro no Program.cs
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
app.UseExceptionHandler();
```

### Custom Exceptions
```csharp
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
    protected DomainException(string message, Exception innerException) : base(message, innerException) { }
}

public class UserNotFoundException : DomainException
{
    public UserNotFoundException(int userId) 
        : base($"User with ID {userId} was not found") { }
}

public class ValidationException : DomainException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred")
    {
        Errors = errors;
    }
}
```

### Result Pattern
```csharp
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public string? Error { get; private set; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    private Result(string error)
    {
        IsSuccess = false;
        Error = error;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string error) => new(error);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value!) : onFailure(Error!);
    }
}

// Uso
public async Task<Result<User>> GetUserAsync(int id)
{
    try
    {
        var user = await _repository.GetByIdAsync(id);
        return user == null 
            ? Result<User>.Failure($"User {id} not found")
            : Result<User>.Success(user);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting user {UserId}", id);
        return Result<User>.Failure("An error occurred while retrieving the user");
    }
}
```

---

## Performance e Otimização

### Entity Framework Core
```csharp
// ✅ Use AsNoTracking para consultas read-only
public async Task<IEnumerable<Product>> GetProductsAsync()
{
    return await _context.Products
        .AsNoTracking()
        .Where(p => p.IsActive)
        .ToListAsync();
}

// ✅ Projete apenas campos necessários
public async Task<IEnumerable<ProductSummary>> GetProductSummariesAsync()
{
    return await _context.Products
        .Where(p => p.IsActive)
        .Select(p => new ProductSummary
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price
        })
        .ToListAsync();
}

// ✅ Use Include para carregar dados relacionados
public async Task<Order?> GetOrderWithItemsAsync(int orderId)
{
    return await _context.Orders
        .Include(o => o.Items)
        .ThenInclude(i => i.Product)
        .FirstOrDefaultAsync(o => o.Id == orderId);
}

// ✅ Split queries para collections múltiplas
public async Task<Customer?> GetCustomerWithOrdersAndAddressesAsync(int customerId)
{
    return await _context.Customers
        .AsSplitQuery()
        .Include(c => c.Orders)
        .Include(c => c.Addresses)
        .FirstOrDefaultAsync(c => c.Id == customerId);
}
```

### Caching
```csharp
public class ProductService
{
    private readonly IMemoryCache _cache;
    private readonly IProductRepository _repository;
    private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(30);

    public async Task<Product?> GetProductAsync(int id)
    {
        return await _cache.GetOrCreateAsync($"product_{id}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheExpiry;
            return await _repository.GetByIdAsync(id);
        });
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _cache.GetOrCreateAsync($"products_category_{categoryId}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheExpiry;
            entry.SetPriority(CacheItemPriority.High);
            return await _repository.GetByCategoryAsync(categoryId);
        });
    }
}
```

### HttpClient Otimizado
```csharp
public class ExternalApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExternalApiService> _logger;

    public ExternalApiService(HttpClient httpClient, ILogger<ExternalApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        // Configurações de performance
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
    }

    public async Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed for endpoint {Endpoint}", endpoint);
            return default;
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogWarning("Request timeout for endpoint {Endpoint}", endpoint);
            return default;
        }
    }
}

// Registro no DI
builder.Services.AddHttpClient<ExternalApiService>(client =>
{
    client.BaseAddress = new Uri("https://api.external.com/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
```

---

## Checklist de Qualidade

### ✅ Código
- [ ] Seguir convenções de nomenclatura
- [ ] Usar async/await corretamente
- [ ] Implementar CancellationToken adequadamente
- [ ] Tratar exceções de forma específica
- [ ] Aplicar princípios SOLID
- [ ] Usar dependency injection
- [ ] Documentar APIs com XML comments

### ✅ Testes
- [ ] Cobertura de testes > 80%
- [ ] Testes unitários isolados e rápidos
- [ ] Testes de integração para fluxos críticos
- [ ] Testes E2E para funcionalidades principais
- [ ] Naming convention clara nos testes
- [ ] Usar AAA pattern (Arrange-Act-Assert)

### ✅ Performance
- [ ] Consultas EF Core otimizadas
- [ ] Implementar caching onde apropriado
- [ ] HttpClient configurado corretamente
- [ ] Usar streaming para dados grandes
- [ ] Implementar paginação

### ✅ Segurança
- [ ] Validar todas as entradas
- [ ] Usar HTTPS obrigatório
- [ ] Implementar autenticação/autorização
- [ ] Proteger contra SQL injection
- [ ] Usar secrets para dados sensíveis

### ✅ Monitoramento
- [ ] Logging estruturado
- [ ] Health checks implementados
- [ ] Métricas de performance
- [ ] Tratamento de erros global
- [ ] Correlation IDs para rastreamento

---

*Documento atualizado em: Janeiro 2025*
*Versão das tecnologias: .NET 8.0, C# 12*