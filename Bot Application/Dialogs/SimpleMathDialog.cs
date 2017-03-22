using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace Bot_Application.Dialogs
{
    [LuisModel("bb3f9f1a-2e53-4841-9c60-a5a24ddf3b9d", "b906bcbff8604806a5f19401ca25a3bf")]
    [Serializable]
    public class SimpleMathDialog : LuisDialog<object>
    {
        private bool TryFindNumbers(LuisResult result, out IEnumerable<double> numbers)
        {
            numbers = result.Entities.Where(entity => entity.Type == "builtin.number").Select(entity => { Double.TryParse(entity.Entity, out double entityAsDouble); return entityAsDouble; });
            if (numbers != null && numbers.Count() > 0)
            {
                return true;
            }
            else
            {
                numbers = new List<double>();
                return false;
            }
        }

        private double Subtract(LuisResult result)
        {
            var subtractFrom = result.Entities.Where(entity => entity.Type == "StartNumber").FirstOrDefault();
            var numbers = result.Entities.Where(entity => entity.Type == "builtin.number" && ! (entity.StartIndex == subtractFrom.StartIndex && entity.EndIndex == subtractFrom.EndIndex)).Select(entity => { Double.TryParse(entity.Entity, out double entityAsDouble); return entityAsDouble; });

            if (subtractFrom != null && double.TryParse(subtractFrom.Entity, out double number))
            {                
                return number - numbers.Sum();
            }
            else
            {
                return 0.0 - numbers.Sum();                
            }
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand: " + string.Join(", ", result.Intents.Select(i => i.Intent));
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("sum numbers")]
        public async Task Sum(IDialogContext context, LuisResult result)
        {
            if (TryFindNumbers(result, out IEnumerable<double> numbersToSum))
            {
                string reply = $"sum is {numbersToSum.Sum()}";
                await context.PostAsync(reply);
            }
            else
            {
                await context.PostAsync("did not find numbers to sum");
            }
            context.Wait(MessageReceived);
        }

        [LuisIntent("subtract numbers")]
        public async Task Subtract(IDialogContext context, LuisResult result)
        {
            string reply;
            reply = ComputeSubtractReply(result);

            await context.PostAsync(reply);
            context.Wait(MessageReceived);
        }

        private static string ComputeSubtractReply(LuisResult result)
        {
            string reply;
            var subtractFrom = result.Entities.Where(entity => entity.Type == "StartNumber").FirstOrDefault();
            var numbers = result.Entities.Where(entity => entity.Type == "builtin.number" && !(entity.StartIndex == subtractFrom.StartIndex && entity.EndIndex == subtractFrom.EndIndex)).Select(entity => { Double.TryParse(entity.Entity, out double entityAsDouble); return entityAsDouble; });

            if (subtractFrom != null && double.TryParse(subtractFrom.Entity, out double number))
            {

                string equation = $"{number}";
                foreach (double n in numbers)
                {
                    equation += $"- {n}";
                }                
                reply = $"{equation} =  {number - numbers.Sum()}";
            }
            else
            {
                reply = $"Could not find number to substarct from, using 0.0 instead... {0.0} {numbers.Select(n => "-" + n)} =  is {0.0 - numbers.Sum()}";
            }

            return reply;
        }

        public SimpleMathDialog()
        {
        }


        public SimpleMathDialog(ILuisService service)
            : base(service)
        {
        }     
       
    }
}