using CrewteqTask.VerticalSlicing.Features.Common;
using CrewteqTask.VerticalSlicing.Features.Employee.Commands.AddEmployee;
using CrewteqTask.VerticalSlicing.Features.Employee.Commands.DeleteEmployee;
using CrewteqTask.VerticalSlicing.Features.Employee.Commands.UpdateEmployee;
using CrewteqTask.VerticalSlicing.Features.Employee.Mappings;
using CrewteqTask.VerticalSlicing.Features.Employee.Queries.GetAllEmployees;
using CrewteqTask.VerticalSlicing.Features.Employee.Queries.GetEmployeeById;
using CrewteqTask.VerticalSlicing.Features.Employee.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CrewteqTask.VerticalSlicing.Features.Employee.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EmployeeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IEmployeeMappingService _mappingService;

        public EmployeeController(IMediator mediator, IEmployeeMappingService mappingService)
        {
            _mediator = mediator;
            _mappingService = mappingService;
        }

        /// <summary>
        /// Create a new employee
        /// </summary>
        /// <param name="createEmployeeViewModel">Employee creation data</param>
        /// <returns>Created employee</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<EmployeeViewModel>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeViewModel createEmployeeViewModel)
        {
            var command = new AddEmployeeCommand
            {
                FirstName = createEmployeeViewModel.FirstName,
                LastName = createEmployeeViewModel.LastName,
                Email = createEmployeeViewModel.Email,
                IsActive = createEmployeeViewModel.IsActive
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse(result.Message, result.StatusCode);
                return StatusCode(result.StatusCode, errorResponse);
            }

            var viewModel = _mappingService.MapToViewModel(result.Data);
            var successResponse = ApiResponse<EmployeeViewModel>.SuccessResponse(viewModel, result.Message, result.StatusCode);
            return StatusCode(result.StatusCode, successResponse);
        }

        /// <summary>
        /// Get all employees with pagination and filtering
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <param name="searchTerm">Search term for name or email</param>
        /// <param name="isActive">Filter by active status</param>
        /// <returns>Paginated list of employees</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PaginatedEmployeesViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllEmployees(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null)
        {
            var query = new GetAllEmployeesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                IsActive = isActive
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse(result.Message, result.StatusCode);
                return StatusCode(result.StatusCode, errorResponse);
            }

            var viewModel = _mappingService.MapToPaginatedViewModel(result.Data);
            var successResponse = ApiResponse<PaginatedEmployeesViewModel>.SuccessResponse(viewModel, result.Message, result.StatusCode);
            return StatusCode(result.StatusCode, successResponse);
        }

        /// <summary>
        /// Get employee by ID
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Employee details</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<EmployeeViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var query = new GetEmployeeByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse(result.Message, result.StatusCode);
                return StatusCode(result.StatusCode, errorResponse);
            }

            var viewModel = _mappingService.MapToViewModel(result.Data);
            var successResponse = ApiResponse<EmployeeViewModel>.SuccessResponse(viewModel, result.Message, result.StatusCode);
            return StatusCode(result.StatusCode, successResponse);
        }

        /// <summary>
        /// Update an existing employee
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <param name="updateEmployeeViewModel">Employee update data</param>
        /// <returns>Updated employee</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<EmployeeViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeViewModel updateEmployeeViewModel)
        {
            var command = new UpdateEmployeeCommand
            {
                Id = id,
                FirstName = updateEmployeeViewModel.FirstName,
                LastName = updateEmployeeViewModel.LastName,
                Email = updateEmployeeViewModel.Email,
                IsActive = updateEmployeeViewModel.IsActive
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                var errorResponse = ApiResponse<object>.ErrorResponse(result.Message, result.StatusCode);
                return StatusCode(result.StatusCode, errorResponse);
            }

            var viewModel = _mappingService.MapToViewModel(result.Data);
            var successResponse = ApiResponse<EmployeeViewModel>.SuccessResponse(viewModel, result.Message, result.StatusCode);
            return StatusCode(result.StatusCode, successResponse);
        }

        /// <summary>
        /// Delete an employee (soft delete)
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var command = new DeleteEmployeeCommand { Id = id };
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                var errorResponse = ApiResponse.ErrorResponse(result.Message, result.StatusCode);
                return StatusCode(result.StatusCode, errorResponse);
            }

            var successResponse = ApiResponse.SuccessResponse(result.Message, result.StatusCode);
            return StatusCode(result.StatusCode, successResponse);
        }
    }
}