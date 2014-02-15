NLog.Targets.Chatwork
=====================

[![Build status](https://ci.appveyor.com/api/projects/status?id=ubcx0i351c34342a)](https://ci.appveyor.com/project/csharp-chatwork-api)

NLogでChatworkに投稿するターゲット

http://tech.tanaka733.net/entry/2013/12/12/nlog-tagets-chatwork

## Requirement

限定公開であるChatwork APIの認証トークンが必要です。

## Configuration

```xml
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <extensions>
    <add assembly="NLog.Targets.Chatwork"/>
  </extensions>
  <targets>
    <target name="cw" xsi:type="ChatworkTarget"
            Token="hogeToken" RoomId="roomIdToPost"
            Layout="[${logger}]${newline}${message}"/>
  </targets>
  <rules>
    <logger name="*" minlevel="Trace" writeTo="cw" />
  </rules>
</nlog>
```
