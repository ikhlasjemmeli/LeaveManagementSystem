using AutoMapper;
using LeaveManagementSystem.Data.Repositories.Interfaces;
using LeaveManagementSystem.Dtos;
using LeaveManagementSystem.Enums;
using LeaveManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly ILeaveRequestRepository _repo;
        private readonly IMapper _mapper;

        public LeaveRequestsController(ILeaveRequestRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        // Retrieve all leave requests and associated employees
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var leaves = await _repo.GetAllLeaveRequests();
            return Ok(_mapper.Map<IEnumerable<LeaveRequestDto>>(leaves));
        }

        // Retrieve a leave request by its id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var request = await _repo.GetByIdAsync(id);
            if (request == null)
                return NotFound();

            var result = _mapper.Map<LeaveRequestDto>(request);
            return Ok(result);
        }



        // Create a new leave request
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLeaveRequestDto dto)
        {
            var request = _mapper.Map<LeaveRequest>(dto);
            request.CreatedAt = DateTime.UtcNow;
            request.Status = LeaveStatus.Pending;

            var error = await _repo.CreateAsync(request);

            if (error != null)
                return BadRequest(new { error });

            return Ok(new { message = "Leave request successfully created." });
        }




        // Update an existing leave request
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateLeaveRequestDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            _mapper.Map(dto, existing);

            await _repo.UpdateAsync(existing);
            await _repo.SaveAsync();

            return NoContent();
        }


        // Deletes a leave request by id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var request = await _repo.GetByIdAsync(id);
            if (request == null)
                return NotFound();

            await _repo.DeleteAsync(request);
            await _repo.SaveAsync();

            return NoContent();
        }

        // Filter leave requests based on query parameters
        [HttpGet("filter")]
        public async Task<IActionResult> Filter([FromQuery] LeaveRequestFilterDto filter)
        {
            var results = await _repo.FilterLeaveRequests(filter);
            var mappedResults = _mapper.Map<IEnumerable<LeaveRequestDto>>(results);

            return Ok(new
            {
                data = mappedResults,
                page = filter.Page,
                pageSize = filter.PageSize
            });
        }

        // Generate a report of leave requests
        [HttpGet("report")]
        public async Task<IActionResult> GetReport([FromQuery] int year, [FromQuery] string? department, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var result = await _repo.GetSummaryReport(year, department, startDate, endDate);
            return Ok(result);
        }

        // Approve a leave request by id
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var error = await _repo.ApproveAsync(id);
            if (error != null)
                return BadRequest(new { error });

            return Ok(new { message = "Request successfully approved." });
        }



    }
}
