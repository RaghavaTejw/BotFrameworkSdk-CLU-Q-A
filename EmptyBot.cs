// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.22.0

using Azure;
using Azure.AI.Language.Conversations;
using Azure.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EmptyBot1
{
    public class EmptyBot : ActivityHandler
    {
        private readonly BotState ConversationState;
        private readonly BotState UserState;
        private readonly ConversationAnalysisClient client;
        public EmptyBot(UserState userstate,ConversationState conversationState) 
        {
            ConversationState=conversationState;
            UserState = userstate;

            string endpoint = "https://qnabotlanguage.cognitiveservices.azure.com/";
            string apiKey = "60f814c16802462f85ba9c88a6acdcb4";
            var credential = new AzureKeyCredential(apiKey);
            client = new ConversationAnalysisClient(new Uri(endpoint), credential);

        }
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello world!"), cancellationToken);
                    var signincard = new HeroCard()
                    {
                        Title="Sign In Card",
                        Buttons=new List<CardAction>()
                        {
                             new CardAction()
                            {
                                Type=ActionTypes.Signin,
                                Title="Sign-In",
                                Value="https://accounts.google.com/v3/signin/identifier?flowEntry=ServiceLogin&flowName=GlifWebSignIn&ifkv=AdF4I75Y2Lm2j6TMPX_O9eD8eetbzGS1rnn65jPuvZaZX3adVuXNbt6HKkCKJ8RxRgWxUDYGDzgvmQ&ddm=0&continue=https%3A%2F%2Faccounts.google.com%2FManageAccount%3Fnc%3D1"
                            }
                        }
                    };
                    await turnContext.SendActivityAsync(MessageFactory.Attachment(signincard.ToAttachment()), cancellationToken);
                    var heroCard = new HeroCard
                    {
                        Text = "Welcome to Human Logic Bot Assistant",
                        Buttons = new List<CardAction>() 
                        { 
                            new CardAction() { Title="About Human Logic",Value="Growth through learning",Type=ActionTypes.ImBack},
                            new CardAction() { Title="Get Course Available",Value="Get Courses Available",Type=ActionTypes.ImBack},
                            new CardAction() { Title="Get all my courses",Value="Get all my registered courses",Type=ActionTypes.ImBack},
                            new CardAction() { Title="passed courses",Value="passed courses",Type=ActionTypes.ImBack}
                        }
                    };
                    await turnContext.SendActivityAsync(MessageFactory.Attachment(heroCard.ToAttachment()), cancellationToken);
                }
            }
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occurred during the turn.
            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            string userInput = turnContext.Activity.Text;
            // CLU
            //string projectName = "simple_CLU";
            //string deploymentName = "simple_CLU_Deployment";

            //var data = new
            //{
            //    analysisInput = new
            //    {
            //        conversationItem = new
            //        {
            //            text = userInput,
            //            id = "1",
            //            participantId = "1",
            //        }
            //    },
            //    parameters = new
            //    {
            //        projectName,
            //        deploymentName,

            //        // Use Utf16CodeUnit for strings in .NET.
            //        stringIndexType = "Utf16CodeUnit",
            //    },
            //    kind = "Conversation",
            //};
            //Response response = client.AnalyzeConversation(RequestContent.Create(data));
            //using JsonDocument result = JsonDocument.Parse(response.ContentStream);
            //JsonElement conversationalTaskResult = result.RootElement;
            //JsonElement conversationPrediction = conversationalTaskResult.GetProperty("result").GetProperty("prediction");
            //await turnContext.SendActivityAsync(MessageFactory.Text($"Top intent: {conversationPrediction.GetProperty("topIntent").GetString()}"));

            //foreach (var intent in conversationPrediction.GetProperty("intents").EnumerateArray())
            //{
            //    if (intent.GetProperty("category").GetString().Equals(conversationPrediction.GetProperty("topIntent").GetString()))
            //    {
            //        await turnContext.SendActivityAsync(MessageFactory.Text($"intent recognized: {intent.GetProperty("category")}, confidence score: {intent.GetProperty("confidenceScore")}"), cancellationToken);
            //    }
            //}

            //foreach (var entity in conversationPrediction.GetProperty("entities").EnumerateArray())
            //{
            //    await turnContext.SendActivityAsync(MessageFactory.Text($"category: {entity.GetProperty("category")}"), cancellationToken);
            //}


            // Orchestration workflow


            var conversationStateAccessor=ConversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationData=await conversationStateAccessor.GetAsync(turnContext, () => new ConversationData());

            conversationData.utterance=userInput;
            

            string projectName = "simple_orchestration";
            string deploymentName = "simple_orchestration_deployment";

            var data = new
            {
                analysisInput = new
                {
                    conversationItem = new
                    {
                        text = userInput,
                        id = "1",
                        participantId = "1",
                    }
                },
                parameters = new
                {
                    projectName,
                    deploymentName,

                    // Use Utf16CodeUnit for strings in .NET.
                    stringIndexType = "Utf16CodeUnit",
                },
                kind = "Conversation",
            };

            Response response = client.AnalyzeConversation(RequestContent.Create(data));
            using JsonDocument result = JsonDocument.Parse(response.ContentStream);
            JsonElement conversationalTaskResult = result.RootElement;
            JsonElement orchestrationPrediction = conversationalTaskResult.GetProperty("result").GetProperty("prediction");


            await turnContext.SendActivityAsync(MessageFactory.Text($"TopIntent: {orchestrationPrediction.GetProperty("topIntent")}"));

            string respondingProjectName = orchestrationPrediction.GetProperty("topIntent").GetString();
            JsonElement targetIntentResult = orchestrationPrediction.GetProperty("intents").GetProperty(respondingProjectName);
            
            if (targetIntentResult.GetProperty("targetProjectKind").GetString() == "QuestionAnswering")
            {
                JsonElement questionAnsweringResponse = targetIntentResult.GetProperty("result");
                foreach (JsonElement answer in questionAnsweringResponse.GetProperty("answers").EnumerateArray())
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"{answer.GetProperty("answer").GetString()}"));
                    var promptsarray = answer.GetProperty("dialog").GetProperty("prompts").EnumerateArray();

                    conversationData.intent = respondingProjectName;
                    

                    if (promptsarray.Count() > 0)
                    {
                       
                        var buttons = new List<CardAction>();
                        foreach (var prompt in promptsarray)
                        {
                            buttons.Add(
                                new CardAction
                                {
                                    Title = prompt.GetProperty("displayText").GetString(),
                                    Type = ActionTypes.ImBack,
                                    Value = prompt.GetProperty("displayText").GetString()
                                });

                            conversationData.answers.Add(prompt.GetProperty("displayText").GetString());

                        }
                        var heroCard = new HeroCard
                        {
                            Text = null,
                            Buttons = buttons
                        };
                        await turnContext.SendActivityAsync(MessageFactory.Attachment(heroCard.ToAttachment()), cancellationToken);
                    }
                    else
                    {
                        conversationData.answers.Add(answer.GetProperty("answer").GetString());
                    }
                    
                    
                }
                
            }

            else if (targetIntentResult.GetProperty("targetProjectKind").GetString() == "Conversation")
            {
                JsonElement cluResponse = targetIntentResult.GetProperty("result").GetProperty("prediction");
                //await turnContext.SendActivityAsync(MessageFactory.Text($"intent recognized:  {cluResponse.GetProperty("topIntent")}"), cancellationToken);

                conversationData.intent = cluResponse.GetProperty("topIntent").GetString();
                //foreach (var entity in cluResponse.GetProperty("entities").EnumerateArray())
                //{
                //    await turnContext.SendActivityAsync(MessageFactory.Text($"category : {entity.GetProperty("category")},   identified device : {entity.GetProperty("text")}"), cancellationToken);
                //}
                
            }
            await conversationStateAccessor.SetAsync(turnContext, conversationData);
            await ConversationState.SaveChangesAsync(turnContext);

            await turnContext.SendActivityAsync(MessageFactory.Text($"utterance:  {conversationData.utterance},intent:  {conversationData.intent}"), cancellationToken);
            if (conversationData.answers != null)
            {
               foreach (var answer in conversationData.answers)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"answer:  {answer}"), cancellationToken);

                }

            }

            


        }
    }
}
