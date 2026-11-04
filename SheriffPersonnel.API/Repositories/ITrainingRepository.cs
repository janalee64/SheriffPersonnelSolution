using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SheriffPersonnel.Shared.DTOs;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.API.Repositories
{
    public interface ITrainingRepository
    {
        Task<IEnumerable<EmployeeTraining>> GetEmployeeTrainingsByEmployeeIdAsync(int employeeId);
        Task<EmployeeTraining?> GetEmployeeTrainingByIdAsync(int id);
        Task<EmployeeTraining> AddEmployeeTrainingAsync(EmployeeTraining employeeTraining);
        Task<EmployeeTraining> UpdateEmployeeTrainingAsync(EmployeeTraining employeeTraining);
        Task<bool> DeleteEmployeeTrainingAsync(int id);

        Task<IEnumerable<EmployeeTraining>> GetTrainingExpirationsAsync(DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<EmployeeTraining>> GetTrainingBySchoolAsync(TrainingBySchoolRequestDto request);

        Task<IEnumerable<TrainingSession>> GetTrainingSessionsAsync();
        Task<TrainingSession?> GetTrainingSessionByIdAsync(int id);
        Task<TrainingSession> AddTrainingSessionAsync(TrainingSession trainingSession);
        Task<TrainingSession> UpdateTrainingSessionAsync(TrainingSession trainingSession);
        Task<IEnumerable<TrainingSchool>> GetTrainingSchoolsAsync();
    }
}
