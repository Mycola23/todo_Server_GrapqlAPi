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
                options.ThrowOnUnhandledException = false;
            });
    })
    .AddCors(options =>
    {
        options.AddPolicy("AllowLocalhost5173",
            policy =>
            {
                policy.WithOrigins("http://localhost:5173")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
    })
    .AddControllersWithViews();
    


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
  
    app.UseDeveloperExceptionPage();
}
app.UseCors("AllowLocalhost5173");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();


app.UseGraphQL<ISchema>("/graphql");


if (app.Environment.IsDevelopment())
{
    app.UseGraphQLPlayground("/graphql/playground");
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();