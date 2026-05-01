using PandaCubeTimer.Models;
using PandaCubeTimer.Models.DTOs;

namespace PandaCubeTimer.Helpers;

public static class SessionMappers
{
    // Из Model в DTO
    public static SessionDTO ToDTO(this Session model)
    {
        return new SessionDTO
        {
            Id = model.Id,
            Name = model.Name,
            DisciplineId = model.DisciplineId
        };
    }

    // Из DTO обратно в Model
    public static Session ToModel(this SessionDTO dto)
    {
        return new Session
        {
            Id = dto.Id,
            Name = dto.Name,
            DisciplineId = dto.DisciplineId
        };
    }
}