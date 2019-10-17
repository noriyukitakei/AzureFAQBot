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
                // ILoggerインターフェースに特定の実装をDIするための手順になります。ConfigureLoggingメソッドで行います。
                .ConfigureLogging(builder => {
                    // AddApplicationInsightsメソッドを使うと、ILoggerインターフェースにApplication Insightsにロギングするための
                    // 実装がDIされます。引数には、Application Insightsのインストゥルメーションキーを設定します。
                    builder.AddApplicationInsights("");

                    // Infoレベル以上のログのみApplication Insightsに送信されるよう設定を行います。
                    builder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>
                                 ("", LogLevel.Information);

                });
    }
}
