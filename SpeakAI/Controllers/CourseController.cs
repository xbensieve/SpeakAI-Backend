using BLL.Interface;
using Common.DTO;
using Common.Enum;
using DTO.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SpeakAI.Controllers
{
    
    [Route("api/courses")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(Guid id)
        {
            var response = await _courseService.GetCourseByIdAsync(id);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] UpdateCourseDTO courseDto)
        {
            var response = await _courseService.UpdateCourseAsync(id, courseDto);
          return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            var response = await _courseService.DeleteCourseAsync(id);
            return Ok(response);
        }

        [HttpGet("topic/{id}")]
        public async Task<IActionResult> GetTopic(Guid id)
        {
            var response = await _courseService.GetTopicByIdAsync(id);
            return Ok(response);
        }

        [HttpPost("{courseId}/topic")]
        public async Task<IActionResult> AddTopic(Guid courseId, [FromBody] CreateTopicDTO topicDto)
        {
            var response = await _courseService.AddTopicToCourseAsync(courseId, topicDto);
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDTO courseDto)
        {
            var response = await _courseService.CreateCourseWithTopicsAndExercisesAsync(courseDto);
            return Ok(response);
        }

        [HttpPut("topic/{id}")]
        public async Task<IActionResult> UpdateTopic(Guid id, [FromBody] UpdateTopicDTO topicDto)
        {
            var response = await _courseService.UpdateTopicAsync(id, topicDto);
            return Ok(response);
        }

        [HttpDelete("topic/{id}")]
        public async Task<IActionResult> DeleteTopic(Guid id)
        {
            var response = await _courseService.DeleteTopicAsync(id);
            return Ok(response);
        }

        [HttpGet("exercise/{id}")]
        public async Task<IActionResult> GetExercise(Guid id)
        {
            var response = await _courseService.GetExerciseByIdAsync(id);
            return Ok(response);
        }

        [HttpPost("topics/{topicId}/exercises")]
        public async Task<IActionResult> AddExercise(Guid topicId, [FromBody] CreateExerciseDTO exerciseDto)
        {
            var response = await _courseService.AddExerciseToTopicAsync(topicId, exerciseDto);
            return Ok(response);
        }

        [HttpPut("exercise/{id}")]
        public async Task<IActionResult> UpdateExercise(Guid id, [FromBody] UpdateExerciseDTO exerciseDto)
        {
            var response = await _courseService.UpdateExerciseAsync(id, exerciseDto);
            return Ok(response);
        }

        [HttpDelete("exercise/{id}")]
        public async Task<IActionResult> DeleteExercise(Guid id)
        {
            var response = await _courseService.DeleteExerciseAsync(id);
            return Ok(response);
        }
        [HttpPost("{courseId}/enrollments")]
        public async Task<IActionResult> Enroll(Guid courseId, [FromBody] Guid userId)
        {
            var result = await _courseService.EnrollCourseAsync(userId, courseId);
           return Ok(result);
        }

        [HttpGet("enrollments/{enrolledCourseId}")]
        public async Task<IActionResult> GetEnrolledDetails(Guid enrolledCourseId)
        {
            var result = await _courseService.GetEnrolledCourseDetailsAsync(enrolledCourseId);
           return Ok(result);
        }

        [HttpPost("exercises/{exerciseId}/submissions")]
        public async Task<IActionResult> SubmitExercise(Guid exerciseId, [FromBody] SubmitExerciseDTO dto)
        {
            var result = await _courseService.SubmitExerciseAsync(exerciseId, dto.UserId, dto.EarnedPoints);
           return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCourses([FromQuery] string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return BadRequest(new ResponseDTO(
                        message: "Search keyword is required",
                        statusCode: StatusCodeEnum.BadRequest,
                        success: false
                    ));
                }

                var courses = await _courseService.SearchCourses(keyword);
                return Ok(new ResponseDTO(
                    message: "Search results retrieved successfully",
                    statusCode:  StatusCodeEnum.OK,
                    success: true,
                    result: courses
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseDTO(
                    message: $"Internal server error: {ex.Message}",
                    statusCode: StatusCodeEnum.InteralServerError,
                    success: false
                ));
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            var result = await _courseService.GetAllCoursesAsync();
           return Ok(result);
        }
        [HttpGet("user/{userId}/enrolled-courses")]
        public async Task<IActionResult> GetEnrollcourseByUserId (Guid Userid)
        {
            var result = await _courseService.GetByEnrollcoursebyUserID(Userid);
           return Ok(result);
        }

        [HttpGet("{courseId}/details")]
        public async Task<IActionResult> GetCourseDetail(Guid courseId)
        {
            var response = await _courseService.GetCourseDetailAsync(courseId);
            return Ok(response);
        }

        [HttpGet("users/{userId}/courses/{courseId}/enrollment-status")]
        public async Task<IActionResult> CheckEnrollment(Guid userId, Guid courseId)
        {
            var response = await _courseService.CheckUserEnrollmentAsync(userId, courseId);
            return Ok(response);
        }
    }
}
