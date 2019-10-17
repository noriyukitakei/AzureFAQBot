// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot : ActivityHandler
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        // .NET Coreのいろんな設定を取得できるIConfigurationの実装を
        // DIコンテナから取得してます。IConfigurationの実装は.NET Coreが自動でDIコンテナに入れてくれています。
        // またILoggerインターフェースの実装をDIコンテナから取得します。後ほどご紹介する
        // Program.csで、ILoggerインターフェースを実装したApplication Insightsのログ出力の実装をDIしますので、
        // このILoggerを利用すると、Application Insightsにログが出力されます。
        public EchoBot(IConfiguration configuration, ILogger<EchoBot> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // QnAMakerのインスタンスを生成し、その引数として、QnAMakerの接続情報を表すQnAMakerEndpointの
            // インスタンスを指定します。そのインスタンスのプロパティとして、先程appSettings.jsonで
            // 指定した3つの値を入れます。
            var qnaMaker = new QnAMaker(new QnAMakerEndpoint
            {
                KnowledgeBaseId = _configuration["QnAKnowledgebaseId"],
                EndpointKey = _configuration["QnAAuthKey"],
                Host = _configuration["QnAEndpointHostName"]
            });

            // QnA MakerからQAデータを取得する際にしているオプションのインスタンスを生成します。
            // 以下の指定は、スコアの高い順から10件取得し、スコアが50以上のものを取得するということを表しています。
            var options = new QnAMakerOptions { Top = 10, ScoreThreshold = 0.5f };

            // QnA Makerにアクセスして回答を取得します。
            var response = await qnaMaker.GetAnswersAsync(turnContext, options);

            // 回答情報を格納したresponse変数がNULLではなく、回答が1つ以上あった場合、その回答を表示します。
            // それ以外は、適切な回答が見つからなかった旨のメッセージを表示します。
            if (response != null && response.Length > 0)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(response[0].Answer), cancellationToken);
            }
            else
            {
                // ユーザーに対して適切な回答を返せなかった問い合わせをApplication Insightsにロギングします。
                // 検索しやすいように特定のイベントIDを指定します。
                _logger.LogInformation(turnContext.Activity.Text);
                await turnContext.SendActivityAsync(MessageFactory.Text("No QnA Maker answers were found."), cancellationToken);
            }

            //await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text}"), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello and welcome!"), cancellationToken);
                }
            }
        }
    }
}
