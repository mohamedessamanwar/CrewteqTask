using CrewteqTask.VerticalSlicing.Data.Repository.Interface;
using CrewteqTask.VerticalSlicing.Features.Common;
using CrewteqTask.VerticalSlicing.Features.Employee.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CrewteqTask.VerticalSlicing.Features.Employee.Queries.GetAllEmployees
{
    public class GetAllEmployeesQuery : IRequest<ServiceResult<PaginatedEmployeeDto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
    }

    public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, ServiceResult<PaginatedEmployeeDto>>
    {
        private readonly IGenericRepository<Data.Entities.Employee> _employeeRepository;

        public GetAllEmployeesQueryHandler(IGenericRepository<Data.Entities.Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<ServiceResult<PaginatedEmployeeDto>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.PageNumber < 1)
                    return ServiceResult<PaginatedEmployeeDto>.Failure(400, "Page number must be greater than 0.");

                if (request.PageSize < 1 || request.PageSize > 100)
                    return ServiceResult<PaginatedEmployeeDto>.Failure(400, "Page size must be between 1 and 100.");

                var query = _employeeRepository.GetAll();

                // Apply filters
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    query = query.Where(e => 
                        e.FirstName.Contains(request.SearchTerm) ||
                        e.LastName.Contains(request.SearchTerm) ||
                        e.Email.Contains(request.SearchTerm));
                }

                if (request.IsActive.HasValue)
                {
                    query = query.Where(e => e.IsActive == request.IsActive.Value);
                }

                // Get total count
                var totalCount = await query.CountAsync(cancellationToken);

                // Calculate pagination
                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
                var skip = (request.PageNumber - 1) * request.PageSize;

                // Get paginated results
                var employees = await query
                    .OrderBy(e => e.Id)
                    .Skip(skip)
                    .Take(request.PageSize)
                    .Select(e => new EmployeeDto
                    {
                        Id = e.Id,
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        Email = e.Email,
                        IsActive = e.IsActive,
                        CreatedAt = e.CreatedAt,
                        UpdatedAt = e.UpdatedAt
                    })
                    .ToListAsync(cancellationToken);

                var paginatedResult = new PaginatedEmployeeDto
                {
                    Employees = employees,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = totalPages,
                    HasNextPage = request.PageNumber < totalPages,
                    HasPreviousPage = request.PageNumber > 1
                };

                return ServiceResult<PaginatedEmployeeDto>.SuccessResult(200, paginatedResult, "Employees retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<PaginatedEmployeeDto>.Failure(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}