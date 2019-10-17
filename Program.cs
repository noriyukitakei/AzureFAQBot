// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                // ILogger�C���^�[�t�F�[�X�ɓ���̎�����DI���邽�߂̎菇�ɂȂ�܂��BConfigureLogging���\�b�h�ōs���܂��B
                .ConfigureLogging(builder => {
                    // AddApplicationInsights���\�b�h���g���ƁAILogger�C���^�[�t�F�[�X��Application Insights�Ƀ��M���O���邽�߂�
                    // ������DI����܂��B�����ɂ́AApplication Insights�̃C���X�g�D�����[�V�����L�[��ݒ肵�܂��B
                    builder.AddApplicationInsights("");

                    // Info���x���ȏ�̃��O�̂�Application Insights�ɑ��M�����悤�ݒ���s���܂��B
                    builder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>
                                 ("", LogLevel.Information);

                });
    }
}
