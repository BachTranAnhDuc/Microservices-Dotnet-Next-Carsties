using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;

    public AuctionsController(AuctionDbContext context, IMapper mapper) {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions() {
        var autions = await _context.Auctions.Include(x => x.Item)
            .OrderBy(x => x.Item.Make)
            .ToListAsync();

        return _mapper.Map<List<AuctionDto>>(autions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAutionById(Guid id) {
        var auction = await _context.Auctions.Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null) return NotFound();

        return _mapper.Map<AuctionDto>(auction);
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto) {
        var auction = _mapper.Map<Auction>(auctionDto);

        auction.Seller = "test seller";
        auction.Winner = "test winner";

        _context.Auctions.Add(auction);

        var result = await _context.SaveChangesAsync() > 0;
        
        if (!result) return BadRequest("Could not save changes to the DB");

        return CreatedAtAction(nameof(GetAutionById), new { auction.Id }, _mapper.Map<AuctionDto>(auction));
    }
}
