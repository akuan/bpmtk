<h1>BPMTK</h1>
<p>
BPMTK is a workflow and Business Process Management (BPM) Platform targeted at business people, developers and system admins, based on .NET Platform(.NET Standard 2.0). Port some good ideas and code from Activiti and jBPM.
It's open-source and distributed under the Apache license.
</p>
<p>
BPMTK 是开源的业务流程平台, 符合BPMN 2.0业务流程语言规范, 一些设计思想、代码来源于Activiti、jBPM， 基于.NET Standard 2.0平台, C#语言开发. 
</p>

<h3>Visual Studio</h3>
<ul>
    <li>Download Visual Studio 2017 or 2019 (any edition) from https://www.visualstudio.com/downloads/</li>
    <li>Open bpmtk.sln and wait for Visual Studio to restore all Nuget packages</li>
</ul>

<h3>Create database schema</h3>
<p>(ConsoleApp project) </p>
<ul>
    <li>Remove exists migrations: dotnet ef migrations remove</li>
    <li>Create migrations: dotnet ef migrations add {MigrationName}</li>
    <li>Update database: dotnet ef database update</li>
</ul>
<h3>本库更改</h3> 
<ul>
    <li>使用.Net 6替换原来的.net standard 2.1</li>
    <li>升级依赖库</li>
    <li>使用sqlsuger替换EF(进行中)</li>
</ul>
