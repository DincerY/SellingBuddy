﻿using System.Net;
using CatalogService.Api.Core.Application;
using CatalogService.Api.Core.Domain;
using CatalogService.Api.Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext _catalogContext;

        public CatalogController(CatalogContext context)
        {
            _catalogContext = context ?? throw new ArgumentNullException(nameof(context));

            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        }

        [HttpGet]
        [Route("getfakeitem/{id}")]
        public async Task<IActionResult> GetFakeCatalog(int id)
        {
            return Ok($"Get Catalog With Id :{id}");
        }





        [HttpGet]
        [Route("items")]
        //Bı kısımı aslında swagger tarafı için yaptık
        [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<CatalogItem>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> ItemsAsync([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0, string ids = null)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = await GetItemsByIdsAsync(ids);

                if (!items.Any())
                {
                    return BadRequest("ids value invalid. Must be comma-separated list of number");
                }

                return Ok(items);

            }

            var totalItems = await _catalogContext.CatalogItems.LongCountAsync();

            var itemsOnPage = await _catalogContext.CatalogItems.OrderBy(c => c.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            //itemsOnPage = ChangeUriPlaceHolder(itemsOnPage);

            var model = new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }

        private async Task<List<CatalogItem>> GetItemsByIdsAsync(string ids)
        {
            var numIds = ids.Split(',').Select(id => (OK: int.TryParse(id, out int x), Value: x));

            if (!numIds.All(nid => nid.OK))
            {
                return new List<CatalogItem>();
            }

            var idsToSelect = numIds.Select(id => id.Value);

            var items = await _catalogContext.CatalogItems.Where(ci => idsToSelect.Contains(ci.Id)).ToListAsync();

            //items = ChangeUriPlaceHolder(items);

            return items;
        }



        [HttpGet]
        [Route("items/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(CatalogItem), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CatalogItem>> ItemByIdAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var item = await _catalogContext.CatalogItems.SingleOrDefaultAsync(ci => ci.Id == id);

            if (item != null)
            {
                item.PictureUri = "";
                return item;
            }

            return NotFound();
        }



        [HttpGet]
        [Route("items/withname/{name:minlength(1)}")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedItemsViewModel<CatalogItem>>> ItemsWithNameAsync(string name,
            [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var totalItems = await _catalogContext.CatalogItems
                .Where(c => c.Name.StartsWith(name))
                .LongCountAsync();

            var itemsOnPage = await _catalogContext.CatalogItems
                .Where(c => c.Name.StartsWith(name))
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

           // itemsOnPage = ChangeUriPlaceHolder(itemsOnPage);

            return new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);

        }

        [HttpGet]
        [Route("items/type/{catalogTypeId}/brand/{catalogBrandId:int?}")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedItemsViewModel<CatalogItem>>> ItemsByTypeIdAndBrandIdAsync(int? catalogTypeId, int? catalogBrandId, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var root = (IQueryable<CatalogItem>)_catalogContext.CatalogItems;

            root = root.Where(ci => ci.CatalogBrandId == catalogBrandId);

            if (catalogBrandId.HasValue)
            {
                root = root.Where(ci => ci.CatalogBrandId == catalogBrandId);
            }

            var totalItems = await root.LongCountAsync();

            var itemsOnPage = await root
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            //itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

            return new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);


        }

        [HttpGet]
        [Route("items/type/all/brand/{catalogBrandId:int?}")]
        public async Task<ActionResult<PaginatedItemsViewModel<CatalogItem>>> ItemsByBrandIdAsync([FromQuery] int? catalogBrandId, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {

            var root = (IQueryable<CatalogItem>)_catalogContext.CatalogItems;

            if (catalogBrandId.HasValue)
            {
                root = root.Where(ci => ci.CatalogBrandId == catalogBrandId);
            }

            var totalItems = await root.LongCountAsync();

            var itemsOnPage = await root
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            //itemsOnPage = ChangeUriPlaceholder(itemsOnPage);


            return new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);


        }


        [HttpGet]
        [Route("catalogtypes")]
        [ProducesResponseType(typeof(List<CatalogType>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<CatalogType>>> CatalogTypesAsync()
        {
            return await _catalogContext.CatalogTypes.ToListAsync();
        }

        [HttpGet]
        [Route("catalogbrands")]
        [ProducesResponseType(typeof(List<CatalogBrand>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<CatalogBrand>>> CatalogBrandsAsync()
        {
            return await _catalogContext.CatalogBrands.ToListAsync();
        }



        [HttpPut]
        [Route("items")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult> UpdateProductAsync([FromBody] CatalogItem productToUpdate)
        {
            var catalogItem = await _catalogContext.CatalogItems.SingleOrDefaultAsync(i => i.Id == productToUpdate.Id);

            if (catalogItem == null)
            {
                return NotFound(new { Message = $"Item with id {productToUpdate.Id} not found" });
            }

            var oldPrice = catalogItem.Price;
            var raiseProductPriceChangedEvent = oldPrice != productToUpdate.Price;

            catalogItem = productToUpdate;
            _catalogContext.CatalogItems.Update(catalogItem);

            await _catalogContext.SaveChangesAsync();

            return CreatedAtAction(nameof(ItemByIdAsync).ToString(), new { id = productToUpdate.Id }, null);

        }


        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]

        public async Task<ActionResult> DeleteProductAsync(int id)
        {
            var product = _catalogContext.CatalogItems.SingleOrDefault(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            _catalogContext.CatalogItems.Remove(product);

            await _catalogContext.SaveChangesAsync();

            return NoContent();

        }








    }
}
