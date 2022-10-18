using Microsoft.EntityFrameworkCore;
using quizz.Data;
using quizz.Repositories;
using quizz.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseLazyLoadingProxies();
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<ITopicService, TopicService>();
builder.Services.AddTransient<IQuizService, QuizService>();
builder.Services.AddTransient<IQuestionService, QuestionService>();
builder.Services.AddTransient<IMcqOptionService, McqOptionService>();
builder.Services.AddTransient<ITestCaseService, TestCaseService>();
builder.Services.AddTransient<ITestCaseService, TestCaseService>();
builder.Services.AddTransient<IFileHelper, FileHelper>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();