using CrewteqTask.VerticalSlicing.Data.Repository.Interface;
using CrewteqTask.VerticalSlicing.Features.Common;
using CrewteqTask.VerticalSlicing.Features.Employee.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CrewteqTask.VerticalSlicing.Features.Employee.Queries.GetEmployeeById
{
    public class GetEmployeeByIdQuery : IRequest<ServiceResult<EmployeeDto>>
    {
        public int Id { get; set; }
    }

    public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, ServiceResult<EmployeeDto>>
    {
        private readonly IGenericRepository<Data.Entities.Employee> _employeeRepository;

        public GetEmployeeByIdQueryHandler(IGenericRepository<Data.Entities.Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<ServiceResult<EmployeeDto>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Id <= 0)
                    return ServiceResult<EmployeeDto>.Failure(400, "Invalid employee ID.");

                var employee = await _employeeRepository.GetById(request.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (employee == null)
                    return ServiceResult<EmployeeDto>.Failure(404, "Employee not found.");

                var employeeDto = new EmployeeDto
                {
                    Id = employee.Id,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = employee.Email,
                    IsActive = employee.IsActive,
                    CreatedAt = employee.CreatedAt,
                    UpdatedAt = employee.UpdatedAt
                };

                return ServiceResult<EmployeeDto>.SuccessResult(200, employeeDto, "Employee retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<EmployeeDto>.Failure(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}