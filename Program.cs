using GraphQL;
using GraphQL.Types;
using GraphQL.Server.Ui.Playground;
using ToDoProject.Graphql;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<GraphqlService>();
builder.Services.AddHttpClient<GraphqlService>(client => {
    client.BaseAddress = new Uri("https://localhost:7011"); 
});
builder.Services
    .AddSingleton<RootQuery>()
    .AddSingleton<TaskType>()
    .AddSingleton<TaskResponseType>()
    .AddSingleton<CategoryType>()
    .AddSingleton<TaskInputType>()
    .AddSingleton<TaskMutation>()
    .AddHttpContextAccessor()
    .AddSingleton<ISchema, ProjectSchema>(services => new ProjectSchema(services))
    .AddGraphQL(builder => {
        builder
            .AddSystemTextJson()
            .ConfigureExecutionOptions(options => {
                options.EnableMetrics = false;
                // Включаємо детальні помилки для розробки
                options.ThrowOnUnhandledException = false;


            });
    })
    .AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // В режимі розробки показуємо детальні помилки
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// GraphQL endpoint
app.UseGraphQL<ISchema>("/graphql");

// Додаємо GraphQL Playground ТІЛЬКИ для розробки
if (app.Environment.IsDevelopment())
{
    app.UseGraphQLPlayground("/graphql/playground");
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();