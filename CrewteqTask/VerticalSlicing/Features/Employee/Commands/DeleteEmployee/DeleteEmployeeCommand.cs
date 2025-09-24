using CrewteqTask.VerticalSlicing.Data.Repository.Interface;
using CrewteqTask.VerticalSlicing.Features.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CrewteqTask.VerticalSlicing.Features.Employee.Commands.DeleteEmployee
{
    public class DeleteEmployeeCommand : IRequest<ServiceResult>
    {
        public int Id { get; set; }
    }

    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, ServiceResult>
    {
        private readonly IGenericRepository<Data.Entities.Employee> _employeeRepository;

        public DeleteEmployeeCommandHandler(IGenericRepository<Data.Entities.Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<ServiceResult> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Id <= 0)
                    return ServiceResult.Failure(400, "Invalid employee ID.");

                var employee = await _employeeRepository.GetById(request.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (employee == null)
                    return ServiceResult.Failure(404, "Employee not found.");

                _employeeRepository.Delete(employee);
                await _employeeRepository.SaveChangesAsync();

                return ServiceResult.SuccessResult(200, "Employee deleted successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}