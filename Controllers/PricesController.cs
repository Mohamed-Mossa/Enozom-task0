using Enozom_task.Repositories;

[Route("api/[controller]")]
[ApiController]
public class PricesController : ControllerBase
{
    private readonly IRepository<Price> _priceRepository;

    public PricesController(IRepository<Price> priceRepository)
    {
        _priceRepository = priceRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Price>>> GetPrices()
    {
        var prices = await _priceRepository.GetAll();
        return Ok(prices);
    }
    [HttpGet("SearchByPriceRange")]
    public async Task<ActionResult<IEnumerable<object>>> GetRoomsByPriceRange(decimal minPrice, decimal maxPrice)
    {
        var pricesInRange = await _priceRepository.GetAll();

        var rooms = pricesInRange
            .Where(p => p.Cost >= minPrice && p.Cost <= maxPrice)
            .Select(p => new
            {
                HotelId = p.HotelID,
                HotelName = p.Hotel?.name ?? "Unknown"
                // Add other properties related to the room as needed
            })
            .Distinct()
            .ToList();

        if (!rooms.Any())
        {
            return NotFound("No rooms found within the specified price range.");
        }

        return Ok(rooms);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Price>> GetPrice(int id)
    {
        var price = await _priceRepository.GetById(id);
        if (price == null) return NotFound();

        return Ok(price);
    }

    [HttpPost]
    public async Task<ActionResult<Price>> PostPrice(Price price)
    {
        await _priceRepository.Create(price);
        return CreatedAtAction(nameof(GetPrice), new { id = price.Id }, price);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPrice(int id, Price price)
    {
        if (id != price.Id) return BadRequest();

        await _priceRepository.Update(id, price);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePrice(int id)
    {
        await _priceRepository.Delete(id);
        return NoContent();
    }
}
