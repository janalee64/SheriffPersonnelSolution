using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SheriffPersonnel.Shared.DTOs;

namespace SheriffPersonnel.API.Services
{
    public interface ITrainingService
    {
        Task<IEnumerable<EmployeeTrainingsDto>> GetEmployeeTrainingsByEmployeeIdAsync(int employeeId);
        Task<EmployeeTrainingsDto?> GetEmployeeTrainingByIdAsync(int id);
        Task<EmployeeTrainingsDto> CreateEmployeeTrainingAsync(EmployeeTrainingsDto dto);
        Task<EmployeeTrainingsDto?> UpdateEmployeeTrainingAsync(int id, EmployeeTrainingsDto dto);
        Task<bool> DeleteEmployeeTrainingAsync(int id);

        Task<IEnumerable<TrainingExpirationDto>> GetTrainingExpirationsAsync(DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<TrainingBySchoolDto>> GetTrainingBySchoolAsync(TrainingBySchoolRequestDto request);

        Task<IEnumerable<TrainingSessionsDto>> GetTrainingSessionsAsync();
        Task<TrainingSessionsDto> CreateTrainingSessionAsync(TrainingSessionsDto dto);
        Task<TrainingSessionsDto?> UpdateTrainingSessionAsync(int id, TrainingSessionsDto dto);
        Task<IEnumerable<TrainingSchoolsDto>> GetTrainingSchoolsAsync();
        Task<MultipleTrainingAssignmentResultDto> AssignMultipleTrainingsAsync(MultipleTrainingAssignmentRequestDto request);
    }
}
