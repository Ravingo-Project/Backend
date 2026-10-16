using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RavingoTest.Dto;
using RavingoTest.Service.Interface;

namespace RavingoTest.Controllers
{
    [ApiController]
    [Route("transfers")]
    public class TransfersController:ControllerBase
    {
        private readonly ITransferService _transferService;

        public TransfersController(ITransferService transferService)
        {
            _transferService = transferService;
        }


        [HttpPost]
        [EnableRateLimiting("TransferLimitPolicy")]
        public async Task<ActionResult<TransferResponseDto>> CreateTransfer([FromBody] CreateTransferDto dto)
        {
            try
            {
                var result = await _transferService.CreateTransferAsync(dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /transfers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransferResponseDto>>> GetTransfers()
        {
            var transfers = await _transferService.GetTransferHistoryAsync();
            return Ok(transfers);
        }
    }
}
