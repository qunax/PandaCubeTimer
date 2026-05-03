using PandaCubeTimer.Models;
using PandaCubeTimer.Models.DTOs;
using PandaCubeTimer.Services;

namespace PandaCubeTimer.Helpers;

public static class SessionMappers
{
    public static SessionDTO ToDTO(this Session model)
    {
        return new SessionDTO
        {
            Id = model.Id,
            Name = model.Name,
            DisciplineId = model.DisciplineId
        };
    }

    public static Session ToModel(this SessionDTO dto)
    {
        return new Session
        {
            Id = dto.Id,
            Name = dto.Name,
            DisciplineId = dto.DisciplineId
        };
    }

    public static Session ToModel(this SessionSyncDTO dto)
    {
        return new Session
        {
            Id = dto.Id,
            Name = dto.Name,
            DisciplineId = dto.DisciplineId,
            IsDeleted = dto.IsDeleted
        };
    }
}