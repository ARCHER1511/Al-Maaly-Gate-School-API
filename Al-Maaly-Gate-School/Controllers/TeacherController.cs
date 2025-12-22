using Al_Maaly_Gate_School.ControllerResponseHandler;
using Application.DTOs.TeacherDTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Al_Maaly_Gate_School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            await this.HandleAsync(() => _teacherService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id) =>
            await this.HandleAsync(() => _teacherService.GetByIdAsync(id));

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetDetails(string id) =>
            await this.HandleAsync(() => _teacherService.GetTeacherDetailsAsync(id));

        // NEW ENDPOINTS FOR CURRICULUM
        [HttpGet("curriculum/{curriculumId}")]
        public async Task<IActionResult> GetByCurriculum(string curriculumId) =>
            await this.HandleAsync(() => _teacherService.GetTeachersByCurriculumAsync(curriculumId));

        [HttpGet("curriculum/{curriculumId}/count")]
        public async Task<IActionResult> GetCountByCurriculum(string curriculumId) =>
            await this.HandleAsync(() => _teacherService.GetTeacherCountByCurriculumAsync(curriculumId));

        [HttpPost("{teacherId}/curriculum/{curriculumId}")]
        public async Task<IActionResult> AddToCurriculum(string teacherId, string curriculumId) =>
            await this.HandleAsync(() => _teacherService.AddTeacherToCurriculumAsync(teacherId, curriculumId));

        [HttpDelete("{teacherId}/curriculum/{curriculumId}")]
        public async Task<IActionResult> RemoveFromCurriculum(string teacherId, string curriculumId) =>
            await this.HandleAsync(() => _teacherService.RemoveTeacherFromCurriculumAsync(teacherId, curriculumId));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTeacherDto dto) =>
            await this.HandleAsync(() => _teacherService.CreateAsync(dto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateTeacherDto dto) =>
            await this.HandleAsync(() => _teacherService.UpdateAsync(id, dto));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id) =>
            await this.HandleAsync(() => _teacherService.DeleteAsync(id));
        [HttpGet("not-assigned/subject/{subjectId}")]
        public async Task<IActionResult> GetTeachersNotAssignedToSubject(string subjectId)
            => await this.HandleAsync(() => _teacherService.GetTeachersNotAssignedToThisSubject(subjectId));

        [HttpGet("not-assigned/class/{classId}")]
        public async Task<IActionResult> GetTeachersNotAssignedToClass(string classId) =>
            await this.HandleAsync(() => _teacherService.GetTeachersNotAssignedToClassAsync(classId));

        // In TeacherController.cs - Add this endpoint
        [HttpPost("{teacherId}/assign-to-class/{classId}")]
        public async Task<IActionResult> AssignTeacherToClass(string teacherId, string classId)
        {
            // You'll need to create this method in ITeacherService and TeacherService
            return await this.HandleAsync(() => _teacherService.AssignTeacherToClassAsync(teacherId, classId));
        }

        [HttpGet("{teacherId}/subjects")]
        public async Task<IActionResult> GetTeacherSubjects(string teacherId)
        {
            return await this.HandleAsync(() => _teacherService.GetTeacherSubjectsAsync(teacherId));
        }

        [HttpGet("{teacherId}/classes")]
        public async Task<IActionResult> GetTeacherClasses(string teacherId)
        {
            return await this.HandleAsync(() => _teacherService.GetTeacherClassesAsync(teacherId));
        }
    }
}