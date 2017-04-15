rem https://www.appveyor.com/blog/2017/03/17/codecov/

%USERPROFILE%\.nuget\packages\opencover\4.6.519\tools\OpenCover.Console.exe -target:dotnet.exe -targetargs:"test test\NETStandard.HttpListener.Tests\NETStandard.HttpListener.Tests.csproj --no-build" -filter:"+[NETStandard.HttpListener]* -[*Tests*]*" -nodefaultfilters -output:coverage.xml -register:user -oldStyle

%USERPROFILE%\.nuget\packages\ReportGenerator\2.5.6\tools\ReportGenerator.exe -reports:"coverage.xml" -targetdir:"report"

start report\index.htm