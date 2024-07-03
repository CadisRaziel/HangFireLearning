using Hangfire;
using HangfireBasicAuthenticationFilter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Adicionando o banco que estamos usando ao hangfire
//Seria tipo informar ao hangfire que estamos usando o banco mysqlserver e a connectionString dele esta na variavel connectionString
builder.Services.AddHangfire( (sp, config) =>
{
    //Pegar a string de conexao do nosso banco sqlserver e adicionar ao hangfire
    var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection");
    config.UseSqlServerStorage(connectionString);
});

//Abrir o servidor do hangfire
builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//-> quando o swagger abrir coloque dessa forma (https://localhost:7223/Hangfire) o /HangFire vai abrir a dashBoard do hangfire
//app.UseHangfireDashboard();

//"/job-dashboard" -> ao inves de colocar hangfire na url colocamos o /job-dashboard
//app.UseHangfireDashboard("/job-dashboard"); 

app.UseHangfireDashboard("/job-dashboard", new DashboardOptions
{
    DashboardTitle = "Titulo qualquer", //-> titulo que vai aparecer ao abrir a dashboard do hangfire
    DisplayStorageConnectionString = false, //-> para nao aparecer o nome da conexao com sql na parte debaixo do dashboard

    //Colocando authenticacao (usando o package Hangfire.Dashboard.Basic.Authentication)
    Authorization = new[]
    {
        new HangfireCustomBasicAuthenticationFilter
        {
            User= "admin",
            Pass= "admin"
        }
    }
}); 

app.Run();
