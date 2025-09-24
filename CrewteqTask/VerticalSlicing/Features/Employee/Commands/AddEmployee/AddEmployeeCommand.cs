using CrewteqTask.VerticalSlicing.Data.Repository.Interface;
using CrewteqTask.VerticalSlicing.Features.Common;
using CrewteqTask.VerticalSlicing.Features.Employee.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CrewteqTask.VerticalSlicing.Features.Employee.Commands.AddEmployee
{
    public class AddEmployeeCommand : IRequest<ServiceResult<EmployeeDto>>
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }

    public class AddEmployeeCommandHandler : IRequestHandler<AddEmployeeCommand, ServiceResult<EmployeeDto>>
    {
        private readonly IGenericRepository<Data.Entities.Employee> _employeeRepository;

        public AddEmployeeCommandHandler(IGenericRepository<Data.Entities.Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<ServiceResult<EmployeeDto>> Handle(AddEmployeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Basic validation
                if (string.IsNullOrWhiteSpace(request.FirstName))
                    return ServiceResult<EmployeeDto>.Failure(400, "First name is required.");

                if (string.IsNullOrWhiteSpace(request.LastName))
                    return ServiceResult<EmployeeDto>.Failure(400, "Last name is required.");

                if (string.IsNullOrWhiteSpace(request.Email))
                    return ServiceResult<EmployeeDto>.Failure(400, "Email is required.");

                // Email format validation
                if (!IsValidEmail(request.Email))
                    return ServiceResult<EmployeeDto>.Failure(400, "Invalid email format.");

                // Check if email already exists
                var existingEmployee = await _employeeRepository.GetAll()
                    .FirstOrDefaultAsync(e => e.Email == request.Email, cancellationToken);

                if (existingEmployee != null)
                    return ServiceResult<EmployeeDto>.Failure(409, "Email already exists.");

                var employee = new Data.Entities.Employee
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    IsActive = request.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _employeeRepository.AddAsync(employee);
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

                return ServiceResult<EmployeeDto>.SuccessResult(201, employeeDto, "Employee created successfully.");
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