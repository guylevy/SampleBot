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
                string reply = string.Format("sum is {0}", numbersToSum.Sum());
                await context.PostAsync(reply);
            }
            else
            {
                await context.PostAsync("did not find numbers to sum");
            }
            context.Wait(MessageReceived);
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