using HoleriteAPI.Core.Application.DTOs;
using HoleriteAPI.Core.Application.Interfaces;
using HoleriteAPI.Core.Application.Requests;
using HoleriteAPI.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace HoleriteAPI.Consumer.Controllers
{
    [ApiController]
    [Route("api/holerite")]
    public class HoleriteController(IHoleriteService holeriteService) : ControllerBase
    {
        private readonly IHoleriteService _holeriteService = holeriteService;

        // GET method
        [HttpGet]
        public ActionResult<HoleriteResponse> Get([FromQuery] HoleriteRequest request)
        {
            HoleriteResponseDTO totais = _holeriteService.CalculaTotais(request);
            // return Ok((totais.TotalINSS, totais.TotalIRRF));
            return Ok(
                new HoleriteResponse { Dados = totais, Message = "Holerite calculado com sucesso!" }
            );
        }
    }
}
