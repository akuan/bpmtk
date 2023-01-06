﻿using Bpmtk.Engine.Models;
using Bpmtk.Engine.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bpmtk.Engine.WebApi.Controllers
{
    [Route("/api/packages")]
    public class PackageController : ControllerBase
    {
        private readonly IContext context;
        private readonly IPackageManager packageManager;

        public PackageController(IContext context)
        {
            this.context = context;
            this.packageManager = context.PackageManager;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<PackageModel>>> Get([FromQuery] PackageFilter filter)
        {
            var result = new PagedResult<PackageModel>();
            
            var query = this.packageManager.CreateQuery();

            var list = await query.ListAsync(filter.Page, filter.PageSize);
            result.Count = await query.CountAsync();
            result.Page = filter.Page;
            result.PageSize = filter.PageSize;
            result.Items = list.Select(x => PackageModel.Create(x))
                .ToList();

            return this.Ok(result);
        }

        [HttpGet("{id}/bpmn-model/xml")]
        public async Task<ActionResult<string>> GetModelXml(int id)
        {
            var content = await packageManager.GetModelContentAsync(id);
            if(content != null)
                return Encoding.UTF8.GetString(content);

            return null;
        }

        [HttpGet("{id}/items")]
        public async Task<ActionResult<IList<PackageItem>>> GetItems(int id)
        {
            var items = await packageManager.GetItemsAsync(id);
            if (items != null)
                return this.Ok(items);

            return null;
        }

        [HttpGet("{id}/historic-models")]
        public async Task<ActionResult<IList<HistoricModel>>> GetHistoricModels(int id)
        {
            var items = await packageManager.GetHistoricModelsAsync(id);
            if (items != null)
                return this.Ok(items);

            return this.Ok();
        }

        [HttpGet("historic-items/{itemId}/xml")]
        public async Task<ActionResult<string>> GetHistoricItemContent(int itemId)
        {
            var content = await packageManager.GetHistoricModelContentAsync(itemId);
            if (content != null)
                return Encoding.UTF8.GetString(content);

            return null;
        }


        [HttpGet("historic-models/{id}/xml")]
        public async Task<ActionResult<string>> GetHistoricModelXml(int id)
        {
            var content = await packageManager.GetHistoricModelContentAsync(id);
            if (content != null)
                return Encoding.UTF8.GetString(content);

            return null;
        }

        /// <summary>
        /// Deploy BPMN 2.0 model.
        /// </summary>
        //[HttpPost("deploy")]
        //public ActionResult Deploy([FromBody] DeployBpmnModel model)
        //{
        //    return this.Ok();
        //}

        //[HttpGet("{id}/active-activity-ids")]
        //public ActionResult<IEnumerable<string>> GetActiveActivityIds(long id)
        //{
        //    var q = this.context.RuntimeManager.GetActiveActivityIdsAsync(id).Result;
        //    return q.ToArray();
        //}
    }
}
