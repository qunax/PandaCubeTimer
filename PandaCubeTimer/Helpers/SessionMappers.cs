using PandaCubeTimer.Models;
using PandaCubeTimer.Models.DTOs;
using PandaCubeTimer.Services;

namespace PandaCubeTimer.Helpers;

public static class SessionMappers
{
    public static SessionInAppDTO ToDTO(this Session model)
    {
        return new SessionInAppDTO
        {
            Id = model.Id,
            Name = model.Name,
            DisciplineId = model.DisciplineId
        };
    }
}