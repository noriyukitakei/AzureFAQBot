nuget restore
msbuild EchoBot.sln -p:DeployOnBuild=true -p:PublishProfile=azure-faq-bot-Web-Deploy.pubxml -p:Password=WAHjMnngcjcj2lzQYoLptzE4kFLgglZ7YCRRyvDEhxyTKo5MP8nCXnrPyekx

