using CrewteqTask.VerticalSlicing.Features.Employee.DTOs;
using CrewteqTask.VerticalSlicing.Features.Employee.ViewModels;

namespace CrewteqTask.VerticalSlicing.Features.Employee.Mappings
{
    public interface IEmployeeMappingService
    {
        EmployeeViewModel MapToViewModel(EmployeeDto dto);
        PaginatedEmployeesViewModel MapToPaginatedViewModel(PaginatedEmployeeDto dto);
        CreateEmployeeDto MapToCreateDto(CreateEmployeeViewModel viewModel);
        UpdateEmployeeDto MapToUpdateDto(UpdateEmployeeViewModel viewModel, int id);
    }

    public class EmployeeMappingService : IEmployeeMappingService
    {
        public EmployeeViewModel MapToViewModel(EmployeeDto dto)
        {
            return new EmployeeViewModel
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                IsActive = dto.IsActive,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };
        }

        public PaginatedEmployeesViewModel MapToPaginatedViewModel(PaginatedEmployeeDto dto)
        {
            return new PaginatedEmployeesViewModel
            {
                Employees = dto.Employees.Select(MapToViewModel).ToList(),
                Pagination = new PaginationMetadata
                {
                    TotalCount = dto.TotalCount,
                    PageNumber = dto.PageNumber,
                    PageSize = dto.PageSize,
                    TotalPages = dto.TotalPages,
                    HasNextPage = dto.HasNextPage,
                    HasPreviousPage = dto.HasPreviousPage
                }
            };
        }

        public CreateEmployeeDto MapToCreateDto(CreateEmployeeViewModel viewModel)
        {
            return new CreateEmployeeDto
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                IsActive = viewModel.IsActive
            };
        }

        public UpdateEmployeeDto MapToUpdateDto(UpdateEmployeeViewModel viewModel, int id)
        {
            return new UpdateEmployeeDto
            {
                Id = id,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                IsActive = viewModel.IsActive
            };
        }
    }
}