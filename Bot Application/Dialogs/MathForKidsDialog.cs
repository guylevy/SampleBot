using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

[Serializable]
public class MathForKidsDialog : IDialog<object>
{
    private QuestionModel _question;
    private bool _sessionStarted;

    public async Task StartAsync(IDialogContext context)
    {
        context.Wait(MessageReceivedAsync);
    }
    public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
    {
        var message = await argument;
        if (!_sessionStarted)
        {
            if (string.Equals(message.Text, "Let's study math", StringComparison.OrdinalIgnoreCase))
            {
                _sessionStarted = true;
                await context.PostAsync($"Sounds great {context.Activity.From}. Let's start from addition.");
            }
            else
            {
                await context.PostAsync("Enter \"Let's study math\" to start the conversation.");
                context.Wait(MessageReceivedAsync);
                return;
            }
        }

        if (_question == null)
        {
            _question = GetQuestion();
            await context.PostAsync(_question.Question);
        }
        else
        {
            if (_question.Answer == message.Text)
            {
                _question = GetQuestion();
                await context.PostAsync($"Amazing. {_question.Question}");
            }
            else
                await context.PostAsync($"Think one more time. It's easy. {_question.Question}");
        }
        context.Wait(MessageReceivedAsync);
    }

    private QuestionModel GetQuestion()
    {
        var random = new Random();
        var number1 = random.Next(20);
        var number2 = random.Next(20);

        return new QuestionModel
        {
            Answer = $"{number1 + number2}",
            Question = $"What is the answer for {number1} + {number2}?"
        };
    }
}

[Serializable]
internal class QuestionModel
{
    public string Answer { get; internal set; }
    public string Question { get; internal set; }
}