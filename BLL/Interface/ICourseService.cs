using Common.DTO;
using DAL.Entities;
using DTO.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interface
{
    public interface ICourseService
    {
        /// <summary>
    /// Topic
    /// </summary>
    /// <param name="courseDto"></param>
    /// <returns></returns>
        Task<ResponseDTO> CreateCourseWithTopicsAndExercisesAsync(CreateCourseDTO courseDto);
        Task<ResponseDTO> GetCourseByIdAsync(Guid courseId);
        Task<ResponseDTO> UpdateCourseAsync(Guid courseId, UpdateCourseDTO courseDto);
        Task<ResponseDTO> DeleteCourseAsync(Guid courseId);

/// <summary>
/// Topic 
/// </summary>
/// <param name="topicId"></param>
/// <returns></returns>
        Task<ResponseDTO> GetTopicByIdAsync(Guid topicId);
        Task<ResponseDTO> AddTopicToCourseAsync(Guid courseId, CreateTopicDTO topicDto);
        Task<ResponseDTO> UpdateTopicAsync(Guid topicId, UpdateTopicDTO topicDto);
        Task<ResponseDTO> DeleteTopicAsync(Guid topicId);

        /// <summary>
        /// Exercise
        /// </summary>
        /// <param name="exerciseId"></param>
        /// <returns></returns>
        Task<ResponseDTO> GetExerciseByIdAsync(Guid exerciseId);
        Task<ResponseDTO> AddExerciseToTopicAsync(Guid topicId, CreateExerciseDTO exerciseDto);
        Task<ResponseDTO> UpdateExerciseAsync(Guid exerciseId, UpdateExerciseDTO exerciseDto);
        Task<ResponseDTO> DeleteExerciseAsync(Guid exerciseId);
        /// <summary>
        /// Enroll Course
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="courseId"></param>
        /// <returns></returns>
        Task<ResponseDTO> EnrollCourseAsync(Guid userId, Guid courseId);
        Task<ResponseDTO> GetEnrolledCourseDetailsAsync(Guid enrolledCourseId);
        Task<ResponseDTO> GetAllCoursesAsync();
        Task<ResponseDTO> GetByEnrollcoursebyUserID(Guid userId);
    

        Task<ResponseDTO> SubmitExerciseAsync(Guid exerciseId, Guid userId, decimal earnedPoints);
        /// <summary>
        /// search and get all course
        /// </summary>
      

        

        /// <returns></returns>
     
        Task<IEnumerable<Course>> SearchCourses(string keyword);
        Task<ResponseDTO> GetCourseDetailAsync(Guid courseId);
        Task<ResponseDTO> CheckUserEnrollmentAsync(Guid userId, Guid courseId);


    }
}
