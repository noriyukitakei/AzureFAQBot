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

        // .NET Core�̂����Ȑݒ���擾�ł���IConfiguration�̎�����
        // DI�R���e�i����擾���Ă܂��BIConfiguration�̎�����.NET Core��������DI�R���e�i�ɓ���Ă���Ă��܂��B
        // �܂�ILogger�C���^�[�t�F�[�X�̎�����DI�R���e�i����擾���܂��B��قǂ��Љ��
        // Program.cs�ŁAILogger�C���^�[�t�F�[�X����������Application Insights�̃��O�o�͂̎�����DI���܂��̂ŁA
        // ����ILogger�𗘗p����ƁAApplication Insights�Ƀ��O���o�͂���܂��B
        public EchoBot(IConfiguration configuration, ILogger<EchoBot> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // QnAMaker�̃C���X�^���X�𐶐����A���̈����Ƃ��āAQnAMaker�̐ڑ�����\��QnAMakerEndpoint��
            // �C���X�^���X���w�肵�܂��B���̃C���X�^���X�̃v���p�e�B�Ƃ��āA���appSettings.json��
            // �w�肵��3�̒l�����܂��B
            var qnaMaker = new QnAMaker(new QnAMakerEndpoint
            {
                KnowledgeBaseId = _configuration["QnAKnowledgebaseId"],
                EndpointKey = _configuration["QnAAuthKey"],
                Host = _configuration["QnAEndpointHostName"]
            });

            // QnA Maker����QA�f�[�^���擾����ۂɂ��Ă���I�v�V�����̃C���X�^���X�𐶐����܂��B
            // �ȉ��̎w��́A�X�R�A�̍���������10���擾���A�X�R�A��50�ȏ�̂��̂��擾����Ƃ������Ƃ�\���Ă��܂��B
            var options = new QnAMakerOptions { Top = 10, ScoreThreshold = 0.5f };

            // QnA Maker�ɃA�N�Z�X���ĉ񓚂��擾���܂��B
            var response = await qnaMaker.GetAnswersAsync(turnContext, options);

            // �񓚏����i�[����response�ϐ���NULL�ł͂Ȃ��A�񓚂�1�ȏ゠�����ꍇ�A���̉񓚂�\�����܂��B
            // ����ȊO�́A�K�؂ȉ񓚂�������Ȃ������|�̃��b�Z�[�W��\�����܂��B
            if (response != null && response.Length > 0)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(response[0].Answer), cancellationToken);
            }
            else
            {
                // ���[�U�[�ɑ΂��ēK�؂ȉ񓚂�Ԃ��Ȃ������₢���킹��Application Insights�Ƀ��M���O���܂��B
                // �������₷���悤�ɓ���̃C�x���gID���w�肵�܂��B
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
