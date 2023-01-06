﻿using Bpmtk.Engine;
using Bpmtk.Engine.Models;
using Bpmtk.Engine.Repository;
using Bpmtk.Engine.Runtime;
using Bpmtk.Engine.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using NHibernate.Util;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //Setup LoggerFactory.           
            var loggerFactory = LoggerFactory.Create(builder =>
        builder.AddSystemdConsole(options =>
        {
            options.IncludeScopes = true;
            options.TimestampFormat = "HH:mm:ss ";
        }));
            ILogger<Program> logger = loggerFactory.CreateLogger<Program>();
            //Create Bpmtk-Context Factory, and configure database.
            var conextFactory = new ContextFactory();
            var conn = "server = localhost; port = 3306; database = bpmtk4; user id = root; password = mctx123456; SslMode = None";
            logger.LogDebug(conn);
            conextFactory.Configure(builder =>
            {
                builder.EnableSensitiveDataLogging();
                builder.UseLoggerFactory(loggerFactory);
                builder.UseLazyLoadingProxies(true);
                builder.UseMySql(conn,ServerVersion.AutoDetect(conn));
            });
            
            //Create custom ProcessEventListener.
            var processEventListener = new DemoProcessEventListener();

            //Build Process Engine.
            var engine = new ProcessEngineBuilder()
                .SetContextFactory(conextFactory)
                .SetLoggerFactory(loggerFactory)
                .AddProcessEventListener(processEventListener)
                .Configure(options =>
                {
                    options.SetProperty("isActivityRecorderDisabled", false);
                })
                .Build();

            //init process-engine props. (optional)
            engine.SetValue("isActivityRecorderDisabled", false);

            //Create new context.
            var context = engine.CreateContext();
            
            //Start db transaction.
            var transaction = context.DbSession.BeginTransaction(); 

            //Register current context. (optional)
            Context.SetCurrent(context);

            //Create one test user.
            var identityManager = context.IdentityManager;

            //Check if test user exists.
            var user = identityManager.FindUserById("Aaron");
            if (user == null)
            {
                logger.LogDebug("User Arron not Exitst,will Create User");
                user = new User() { Id = "Aaron", Name = "Aaron" };
                identityManager.CreateUser(user);
            }
            //Set current authenticated user id.
            context.SetAuthenticatedUser(user.Id);

            var deploymentManager = context.DeploymentManager;

            var processId = "nestedForkJoin"; //processDefinitionKey
            var q = deploymentManager.CreateDefinitionQuery()
                .FetchLatestVersionOnly()
                .SetKey(processId)
                .FetchIdentityLinks()                 
                .ListAsync().Result;
            ////var rx = x.ToList();
            if (!q.Any<IProcessDefinition>())
            {
                logger.LogDebug($"Process ${processId} not Deployment，will Deployment");
                //Deploy BPMN 2.0 Model.
                var modelContent = GetBpmnModelContentFromResource("ConsoleApp.resources.ParallelGatewayTest.testNestedForkJoin.bpmn20.xml");

                var deploymentBuilder = deploymentManager.CreateDeploymentBuilder();
                var deployment = deploymentBuilder
                    .SetCategory("演示")
                    .SetName("流程演示")
                    .SetMemo("简单的 BPMTK 流程处理.")
                    .SetBpmnModel(modelContent)
                    .Build();
                var processDefinitions = deployment.ProcessDefinitions;

                foreach (var procDef in processDefinitions)
                {
                    Console.WriteLine($"Process '{procDef.Key}' has been deployed.");
                }
            } 
           
            //Start new process-instance.
            var runtimeManager = context.RuntimeManager;
            var pi = runtimeManager.StartProcessByKeyAsync(processId)
                .Result;
            Console.WriteLine($"Start process {processId}"); 

            HumanTasksInteraction(context, pi)
                .GetAwaiter().GetResult();

            //Verify Process Completed.
            pi = runtimeManager.Find(pi.Id);
            Assert.True(pi.State == ExecutionState.Completed);

            //Commit db transaction.
            transaction.Commit();

            //Dispose context. (close databse connection)
            context.Dispose();

            Console.WriteLine("It's OK.");
            Console.ReadKey();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="pi"></param>
        /// <returns></returns>
        protected static async Task HumanTasksInteraction(IContext context, IProcessInstance pi)
        {
            //Create taskQuery.
            var taskManager = context.TaskManager;
            var query = taskManager.CreateQuery()
                            .SetProcessInstanceId(pi.Id)
                            .SetState(TaskState.Active); //only fetch active-tasks.

            // After process start, only task 0 should be active
            var tasks = await query.ListAsync();
            
            Assert.True(tasks.Count == 1);
            Assert.True(tasks[0].Name == "Task 0");

            taskManager.Assign(tasks[0].Id, context.UserId, "我的任务");
            // Completing task 0 will create Task A and B
            await taskManager.CompleteAsync(tasks[0].Id);
            tasks = await query.ListAsync();
            Assert.True(2 == tasks.Count);
            Assert.True("Task A" == tasks[0].Name);
            Assert.True("Task B" == tasks[1].Name);

            // Completing task A should not trigger any new tasks
            await taskManager.CompleteAsync(tasks[0].Id);
            tasks = query.List();
            Assert.True(1 == tasks.Count);
            Assert.True("Task B" == tasks[0].Name);

            // Completing task B creates tasks B1 and B2
            await taskManager.CompleteAsync(tasks[0].Id);
            tasks = await query.ListAsync();
            Assert.True(2 == tasks.Count);
            Assert.True("Task B1" == tasks[0].Name);
            Assert.True("Task B2" == tasks[1].Name);
            //this.Commit();


            // Completing B1 and B2 will activate both joins, and process reaches
            // task C
            await taskManager.CompleteAsync(tasks[0].Id);
            await taskManager.CompleteAsync(tasks[1].Id);
            tasks = await query.ListAsync();
            Assert.True(1 == tasks.Count);
            Assert.True("Task C" == tasks[0].Name);

            // Completing Task C will finish the process.
            await taskManager.CompleteAsync(tasks[0].Id);
            tasks = await query.ListAsync();
            Assert.True(0 == tasks.Count); //all tasks completed.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        protected static byte[] GetBpmnModelContentFromResource(string resourceName)
        { 
            using (var ms = new MemoryStream())
            {
                var stream = typeof(Program).Assembly.GetManifestResourceStream(resourceName);
                stream.CopyTo(ms);
                stream.Close();
                stream.Dispose();

                return ms.ToArray();
            }
        }
    }
}
