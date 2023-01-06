﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bpmtk.Engine.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bpmtk.Engine.WebApi.Controllers
{
    /// <summary>
    /// Task APIs
    /// </summary>
    [Route("api/tasks")]
    [ApiController]
    public class TaskInstanceController : ControllerBase
    {
        private readonly IContext context;
        private readonly ITaskManager taskManager;

        public TaskInstanceController(IContext context)
        {
            this.context = context;
            this.taskManager = context.TaskManager;
        }

        [HttpGet]
        public virtual async Task<ActionResult<PagedResult<TaskInstanceModel>>> Get([FromQuery] TaskInstanceFilter filter)
        {
            var result = new PagedResult<TaskInstanceModel>();

            if (filter == null)
                filter = new TaskInstanceFilter();

            var query = this.taskManager.CreateQuery()
                .SetAssignee(filter.Assignee);

            var list = await query.ListAsync(filter.Page, filter.PageSize);

            result.Count = await query.CountAsync();
            result.Items = list.Select(x => TaskInstanceModel.Create(x)).ToList();
            result.Page = filter.Page;
            result.PageSize = filter.PageSize;

            return result;
        }

        /// <summary>
        /// Find the specified Task.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskInstanceModel>> Get(int id)
        {
            var item = await this.taskManager.CreateQuery()
                .SetId(id)
                .SingleAsync();
            if (item != null)
                return TaskInstanceModel.Create(item);

            return this.NotFound();
        }

        /// <summary>
        /// Gets IdentityLinks of Task.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}/identity-links")]
        public ActionResult<IEnumerable<IdentityLinkModel>> GetIdentityLinks(int id)
        {
            var q = this.context.DbSession.IdentityLinks
                .Where(x => x.Task.Id == id)
                .Select(x => new IdentityLinkModel
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    GroupId = x.GroupId,
                    Type = x.Type
                }).ToArray();
            
            return q;
        }

        [HttpGet("{id}/rendered-form")]
        public ActionResult<string> Render(long id)
        {
            return "<div>test</div>";
        }

        [HttpPut("{id}/completed")]
        public async Task<ActionResult> Complete(long id, CompleteTaskModel model)
        {
            try
            {
                await this.taskManager.CompleteAsync(id, model.Variables, model.Comment);

                return this.Ok();
            }
            catch(Exception ex)
            {
                return this.StatusCode(500, ex.Message);
            }            
        }

        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
