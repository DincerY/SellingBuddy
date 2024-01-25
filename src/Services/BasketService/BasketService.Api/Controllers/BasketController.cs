using System.Net;
using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Core.Application.Services;
using BasketService.Api.Core.Domain.Models;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BasketService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository repository;
    private readonly IIdentityService identityService;
    private readonly IEventBus _eventBus;
    private readonly ILogger<BasketController> _logger;

    public BasketController(IBasketRepository repository, IIdentityService identityService, IEventBus eventBus, ILogger<BasketController> logger)
    {
        this.repository = repository;
        this.identityService = identityService;
        _eventBus = eventBus;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Basket Service is Up and Running");
    }


    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> GetBasketByIdAsync(string id)
    {
        var basket = await repository.GetBasketAsync(id);

        return Ok(basket ?? new CustomerBasket(id));
    }

    [HttpPost]
    [Route("update")]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> UpdateBasketAsync([FromBody] CustomerBasket value)
    {
        return Ok(await repository.UpdateBasketAsync(value));
    }

    [HttpPost]
    [Route("additem")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult> AddItemToBasket([FromBody] BasketItem basketItem)
    {
        var userId = identityService.GetUserName().ToString();

        var basket = await repository.GetBasketAsync(userId);

        if (basket == null)
        {
            basket = new CustomerBasket(userId);
        }

        basket.Items.Add(basketItem);

        await repository.UpdateBasketAsync(basket);

        return Ok();
    }


    [HttpPost]
    [Route("checkout")]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> CheckoutAsync([FromBody] BasketCheckout basketCheckout,
        [FromHeader(Name = "x-request-id")] string requestId)
    {
        var userId = basketCheckout.Buyer;

        var basket = await repository.GetBasketAsync(userId);

        if (basket == null)
        {
            return BadRequest();
        }

        var userName = identityService.GetUserName();

        var eventMessage = new OrderCreatedIntegrationEvent(userId, userName, basketCheckout.City,
            basketCheckout.Street, basketCheckout.State, basketCheckout.Country, basketCheckout.ZipCode,
            basketCheckout.CardNumber, basketCheckout.CardHoldName, basketCheckout.CardExpiration,
            basketCheckout.CardSecurityNumber, basketCheckout.CardTypeId, basketCheckout.Buyer, basket);

        try
        {
            _eventBus.Publish(eventMessage);
        }
        catch (Exception e)
        {
            _logger.LogError(e,"Error Publishing integration event: {IntegrationEvent} from {BasketService.App}",eventMessage.Id);
            throw;
        }

        return Accepted();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
    public async Task DeleteBasketByIdAsync(string id)
    {
        await repository.DeleteBasketAsync(id);
    }



}