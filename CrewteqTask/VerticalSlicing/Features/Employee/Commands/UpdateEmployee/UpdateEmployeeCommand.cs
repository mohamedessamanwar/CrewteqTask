using CrewteqTask.VerticalSlicing.Data.Repository.Interface;
using CrewteqTask.VerticalSlicing.Features.Common;
using CrewteqTask.VerticalSlicing.Features.Employee.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CrewteqTask.VerticalSlicing.Features.Employee.Commands.UpdateEmployee
{
    public class UpdateEmployeeCommand : IRequest<ServiceResult<EmployeeDto>>
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public bool IsActive { get; set; }
    }

    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, ServiceResult<EmployeeDto>>
    {
        private readonly IGenericRepository<Data.Entities.Employee> _employeeRepository;

        public UpdateEmployeeCommandHandler(IGenericRepository<Data.Entities.Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<ServiceResult<EmployeeDto>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Basic validation
                if (request.Id <= 0)
                    return ServiceResult<EmployeeDto>.Failure(400, "Invalid employee ID.");

                if (string.IsNullOrWhiteSpace(request.FirstName))
                    return ServiceResult<EmployeeDto>.Failure(400, "First name is required.");

                if (string.IsNullOrWhiteSpace(request.LastName))
                    return ServiceResult<EmployeeDto>.Failure(400, "Last name is required.");

                if (string.IsNullOrWhiteSpace(request.Email))
                    return ServiceResult<EmployeeDto>.Failure(400, "Email is required.");

                // Email format validation
                if (!IsValidEmail(request.Email))
                    return ServiceResult<EmployeeDto>.Failure(400, "Invalid email format.");

                // Find existing employee
                var employee = await _employeeRepository.GetById(request.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (employee == null)
                    return ServiceResult<EmployeeDto>.Failure(404, "Employee not found.");

                // Check if email already exists for another employee
                var existingEmployeeWithEmail = await _employeeRepository.GetAll()
                    .FirstOrDefaultAsync(e => e.Email == request.Email && e.Id != request.Id, cancellationToken);

                if (existingEmployeeWithEmail != null)
                    return ServiceResult<EmployeeDto>.Failure(409, "Email already exists for another employee.");

                // Update employee
                employee.FirstName = request.FirstName;
                employee.LastName = request.LastName;
                employee.Email = request.Email;
                employee.IsActive = request.IsActive;
                employee.UpdatedAt = DateTime.UtcNow;

                _employeeRepository.Update(employee);
                await _employeeRepository.SaveChangesAsync();

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

                return ServiceResult<EmployeeDto>.SuccessResult(200, employeeDto, "Employee updated successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<EmployeeDto>.Failure(500, $"An error occurred: {ex.Message}");
            }
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}