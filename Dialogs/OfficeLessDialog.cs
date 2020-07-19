// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class OfficeLessDialog: CancelAndHelpDialog
    {

        private OfficeLessDetails officeLessDetails;

        public OfficeLessDialog()
            : base(nameof(OfficeLessDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                TeletrabalhoStepAsync,
                ComparecerNaEmpresaStepAsync,
                JornadaDeTrabalhoStepAsync,
                HorasExtrasStepAsync,
                ImplementarModeloStepAsync,
                QuemPagaOsCustosStepAsync,
                MelhorFormatoContratacaoStepAsync,
                ModalidadeAplicavelStepAsync,
                ConfirmStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> TeletrabalhoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            officeLessDetails = (OfficeLessDetails)stepContext.Options;

            if (officeLessDetails.Destination == null)
            {
                return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text("Ok " + officeLessDetails.Nome + ", fui treinado para falar sobre trabalho remoto. Podemos posseguir ?") }, cancellationToken);
            }
            else
            {
                  return await stepContext.CancelAllDialogsAsync(cancellationToken);
            }
        }

        private async Task<DialogTurnResult> ComparecerNaEmpresaStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            officeLessDetails = (OfficeLessDetails)stepContext.Options;
            if ((bool)stepContext.Result)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Ok, vamos lá... inicialmente vamos a definição."), cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("O TELETRABALHO é qualquer trabalho que não seja, majoritariamente, executado nas dependências da empresa. Ou seja, é possível de ser executado na casa do colaborador, em um coffee shop, coworkings e não apenas no home office."), cancellationToken);
                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text("No teletrabalho, você acha que o colaborador pode comparecer às dependências da empresa?"),
                };

                return await stepContext.PromptAsync(nameof(ConfirmPrompt), promptOptions, cancellationToken);
               
            }
            else
            {
                return await stepContext.BeginDialogAsync(nameof(OfficeLessDialog), officeLessDetails, cancellationToken);
            }

        }
        private async Task<DialogTurnResult> JornadaDeTrabalhoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string resposta = "no regime do teletrabalho, o colaborador pode comparecer às dependências da empresa para realização de algumas tarefas ou reuniões, desde que ocasionalmente, sem descaracterizar o modelo, como nos termos do art. 75-B, parágrafo único da CLT.";
            if ((bool)stepContext.Result)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Isso mesmo, " + resposta), cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Ops! Não é bem assim, " + resposta), cancellationToken);
            }
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Há controle de jornada nesse modelo?"),
            };

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), promptOptions, cancellationToken);

        }

        private async Task<DialogTurnResult> HorasExtrasStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string resposta = "Não há controle de jornada na modalidade do teletrabalho, como é previsto no art. 62, III da CLT. Nesse regime de trabalho, o colaborador tem liberdade para determinar o horário de trabalho e a empresa não tem meios de, e nem deve exercer, o controle da jornada de trabalho daqueles que trabalham fora de suas dependências.";
            if ((bool)stepContext.Result)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Ops! " + resposta), cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Correto. " + resposta), cancellationToken);
            }
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Se não há controle de jornada, há pagamentos de horas extras?"),
            };

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), promptOptions, cancellationToken);

        }

        private async Task<DialogTurnResult> ImplementarModeloStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string resposta = "Não. Esse modelo não combina com o controle da jornada de trabalho. E, por isso, a remuneração deve ser baseada em produtividade e não em horas trabalhadas.";
            if ((bool)stepContext.Result)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Ops! " + resposta), cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(resposta), cancellationToken);
            }
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Posso implementar esse modelo na minha equipe ou devo formar uma nova?"),
            };

            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);

        }

        private async Task<DialogTurnResult> QuemPagaOsCustosStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string resposta = "a lei permite que o teletrabalho seja convertido em trabalho presencial e vice-versa. ";
            resposta       += "No entanto, deve ser respeitado alguns requisitos para a sua aplicação, previstos no art. 75-C e parágrafos da CLT: ";
            resposta       += "\n\n(i) anuência do empregado;\n(ii) período de transição mínima de 15 (quinze) dias;\ne (iii) termo aditivo do contrato de trabalho.";

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Então " + officeLessDetails.Nome + ", " + resposta), cancellationToken);

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("A empresa deve pagar os custos de eletricidade, internet, entre outros, do colaborador neste modelo?"),
            };

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), promptOptions, cancellationToken);

        }


        private async Task<DialogTurnResult> MelhorFormatoContratacaoStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string resposta = "A CLT não determina uma regra. Neste caso, cabe à empresa e ao colaborador definirem, expressamente, no contrato de trabalho . ";
            resposta += "No entanto, deve ser respeitado alguns requisitos para a sua aplicação, previstos no art. 75-C e parágrafos da CLT: \n";
            resposta += "\n(i) anuência do empregado;\n(ii) período de transição mínima de 15 (quinze) dias;\ne (iii) termo aditivo do contrato de trabalho.";

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(resposta), cancellationToken);


            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("No trabalho remoto, qual é a melhor forma de contratação:"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "PJ", "CLT" }),
                }, cancellationToken);

        }

        private async Task<DialogTurnResult> ModalidadeAplicavelStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string resposta = "Isso depende muito da relação firmada entre a empresa e o prestador de serviço ou empregado, isto é, se for simplesmente uma ";
            resposta += "colaboração por projeto e se o prestador de serviços prestar serviços para mais de uma empresa, é mais adequado que o restador ";
            resposta += "de serviços seja autônomo ou PJ, pois entende-se que não há os principais requisitos para a formação do vínculo empregatício.";

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(resposta), cancellationToken);

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("A modalidade de trabalho proposta pode ser executada mesmo fora do Brasil ?"),
            };

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), promptOptions, cancellationToken);

        }


        private async Task<DialogTurnResult> ConfirmStepAsync (WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string resposta = "Com as possibilidades de hoje, em razão da tecnologia, não há limitação do local de trabalho, podendo, em muitos casos, ";
            resposta += "ser executado fora do país do empregador. Em regra, não há qualquer limitação ao local de trabalho, seja ele dentro ou fora do país. ";

            if ((bool)stepContext.Result)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Exatamente. " + resposta), cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Ops! " + resposta), cancellationToken);
            }
            string complemento = "Por outro lado, a legislação brasileira só é aplicável em território brasileiro, de modo que as regras estabelecidas pela CLT ";
            complemento += "ou pelo Ministério do Trabalho e Emprego não são aplicáveis a contratos de trabalhos executados, majoritariamente, fora do país.\n\n";
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(complemento), cancellationToken);

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("O Bot DEV quer saber se você aprendeu um pouquinho mais sobre trabalho remoto."),
            };

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), promptOptions, cancellationToken);

        }



       private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            if ((bool)stepContext.Result)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("**Fonte**: material desenvolvido pelo **OfficeLess** em parceria com a **Grid.** advocacia."), cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Tudo bem, eu também estou aprendendo aos poucos sobre o tema."), cancellationToken);
               
            }
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

    }
}
