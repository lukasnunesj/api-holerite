using HoleriteAPI.Core.Application.DTOs;

namespace HoleriteAPI.Core.Application.Responses
{
    public class HoleriteResponse()
    {
        public required HoleriteResponseDTO Dados { get; set; }
        public string? Message { get; set; }

        public List<string>? Errors { get; set; } = [];
    }
}
