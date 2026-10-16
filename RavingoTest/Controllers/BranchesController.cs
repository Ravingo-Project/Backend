using Microsoft.AspNetCore.Mvc;
using RavingoTest.Dto;
using RavingoTest.Service.Interface;

namespace RavingoTest.Controllers
{
    [ApiController]
    [Route("branches")]
    public class BranchesController  : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchesController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpGet("{id:int}/stock")]
        public async Task<ActionResult<IEnumerable<BranchStockDto>>> GetBranchStock(int id)
        {
            try
            {
                var stock = await _branchService.GetStockByBranchIdAsync(id);
                return Ok(stock);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

    }
}
