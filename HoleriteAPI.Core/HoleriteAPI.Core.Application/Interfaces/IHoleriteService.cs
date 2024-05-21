using HoleriteAPI.Core.Application.DTOs;
using HoleriteAPI.Core.Application.Responses;

namespace HoleriteAPI.Core.Application.Interfaces
{
    public interface IHoleriteService
    {
        HoleriteResponseDTO CalculaTotais(HoleriteRequestDTO holeriteDTO);
    }
}
