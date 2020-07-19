// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        protected readonly IConfiguration Configuration;
        protected readonly ILogger Logger;

        public MainDialog(IConfiguration configuration, ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            Configuration = configuration;
            Logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "pt-BR"));
            AddDialog(new OfficeLessDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            await stepContext.Context.SendActivityAsync(
                  MessageFactory.Text("Olá, para iniciarmos um bate-papo, vou precisar de algumas informações. Caso queira encerrar o diálogo digite **sair**"), cancellationToken);

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Qual é o seu nome?"),
            };

            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var officeLessDetails = new OfficeLessDetails();
            officeLessDetails.Nome = stepContext.Result.ToString();
            if (!string.IsNullOrEmpty(officeLessDetails.Nome)) {
               return await stepContext.BeginDialogAsync(nameof(OfficeLessDialog), officeLessDetails, cancellationToken);
            }
            else
            {
                // User said "no" so we will skip the next step. Give -1 as the age.
                //return await stepContext.NextAsync(-1, cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Ok, infelizmente para seguir com o fluxo eu precisaria de tal informação. :-(."), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken);
            }

        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            //if (stepContext.Result != null)
            //{
            //    var result = stepContext.Result;
            //    await stepContext.Context.SendActivityAsync(MessageFactory.Text(result.ToString()), cancellationToken);
            //}
            //else
            //{
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Obrigado."), cancellationToken);
            //}
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
