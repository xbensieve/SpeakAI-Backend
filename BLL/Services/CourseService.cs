using BLL.Interface;
using Common.DTO;
using Common.Enum;
using DAL.Entities;
using DAL.UnitOfWork;
using DTO.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    namespace BLL.Services
    {
        public class CourseService : ICourseService
        {
            private readonly IUnitOfWork _unitOfWork;

            public CourseService(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<ResponseDTO> CreateCourseWithTopicsAndExercisesAsync(CreateCourseDTO courseDto)
            {
                try
                {
             
                    if (courseDto.MaxPoint % 1 != 0)
                    {
                        return new ResponseDTO("Total course score must be an integer", StatusCodeEnum.BadRequest, false);
                    }
                    int maxPoint = (int)courseDto.MaxPoint;

                    int totalTopics = courseDto.Topics.Count;
                    if (totalTopics == 0 || maxPoint % totalTopics != 0)
                    {
                        return new ResponseDTO(
                            $"The number of topics must be a divisor of {maxPoint}",
                            StatusCodeEnum.BadRequest,
                            false
                        );
                    }
                    int pointPerTopic = maxPoint / totalTopics;

         
                    var course = new Course
                    {
                        Id = Guid.NewGuid(),
                        CourseName = courseDto.CourseName,
                        Description = courseDto.Description,
                        MaxPoint = maxPoint,
                        IsPremium = courseDto.IsPremium,
                        LevelId = courseDto.LevelId,
                        IsDeleted = false,
                        IsActive = true,
                        UpdatedAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.Course.AddAsync(course);

       
                    foreach (var topicDto in courseDto.Topics)
                    {
                        
                        int totalExercises = topicDto.Exercises.Count;
                        if (totalExercises == 0 || pointPerTopic % totalExercises != 0)
                        {
                            return new ResponseDTO(
                                $"Number of exercises in the topic'{topicDto.TopicName}' must be the wish of {pointPerTopic}",
                                StatusCodeEnum.BadRequest,
                                false
                            );
                        }
                        int pointPerExercise = pointPerTopic / totalExercises;

                        
                        var topic = new Topic
                        {
                            Id = Guid.NewGuid(),
                            TopicName = topicDto.TopicName,
                            MaxPoint = pointPerTopic,
                            IsDeleted = false,
                            IsActive = true,
                            UpdatedAt = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow,
                            CourseId = course.Id
                        };
                        await _unitOfWork.Topic.AddAsync(topic);

                 
                        foreach (var exerciseDto in topicDto.Exercises)
                        {
                            var exercise = new Exercise
                            {
                                Id = Guid.NewGuid(),
                                Content = exerciseDto.Content,
                                MaxPoint = pointPerExercise,
                                IsDeleted = false,
                                IsActive = true,
                                UpdatedAt = DateTime.UtcNow,
                                CreatedAt = DateTime.UtcNow,
                                TopicId = topic.Id
                            };
                            await _unitOfWork.Exercise.AddAsync(exercise);
                        }
                    }

                    await _unitOfWork.SaveChangeAsync();
                    return new ResponseDTO("Create a successful course", StatusCodeEnum.Created, true);
                }
                catch (Exception ex)
                {
                    return new ResponseDTO($"Error: {ex.Message}", StatusCodeEnum.InteralServerError, false);
                }
            }
            public async Task<ResponseDTO> GetCourseByIdAsync(Guid courseId)
            {
                try
                {
                    var course = await _unitOfWork.Course.GetByIdAsync(courseId);
                    if (course == null)
                        return new ResponseDTO("Not Found ", StatusCodeEnum.NotFound, false);

                    return new ResponseDTO("Get Course Successfully", StatusCodeEnum.OK, true, course);
                }
                catch (Exception ex)
                {
                    return new ResponseDTO($"Error: {ex.Message}", StatusCodeEnum.InteralServerError, false);
                }
            }

            public async Task<ResponseDTO> UpdateCourseAsync(Guid courseId, UpdateCourseDTO courseDto)
            {
                try
                {
                    var course = await _unitOfWork.Course.GetByIdAsync(courseId);
                    if (course == null)
                        return new ResponseDTO("Not found course", StatusCodeEnum.NotFound, false);

                    course.CourseName = courseDto.CourseName;
                    course.Description = courseDto.Description;
                    course.MaxPoint = courseDto.MaxPoint;
                    course.LevelId = courseDto.LevelId;
                    course.UpdatedAt = DateTime.UtcNow;

                    await _unitOfWork.Course.UpdateAsync(course);
                    await _unitOfWork.SaveChangeAsync();

                    return new ResponseDTO("Update course successfully", StatusCodeEnum.OK, true);
                }
                catch (Exception ex)
                {
                    return new ResponseDTO($"Error: {ex.Message}", StatusCodeEnum.InteralServerError, false);
                }
            }

            public async Task<ResponseDTO> DeleteCourseAsync(Guid courseId)
            {
                try
                {
                    var course = await _unitOfWork.Course.GetByIdAsync(courseId);
                    if (course == null)
                        return new ResponseDTO("Not found course", StatusCodeEnum.NotFound, false);

                    course.IsDeleted = true;
                    course.UpdatedAt = DateTime.UtcNow;

                    await _unitOfWork.Course.UpdateAsync(course);
                    await _unitOfWork.SaveChangeAsync();

                    return new ResponseDTO("Delete successfully", StatusCodeEnum.OK, true);
                }
                catch (Exception ex)
                {
                    return new ResponseDTO($"Error: {ex.Message}", StatusCodeEnum.InteralServerError, false);
                }
            }

            // Topic operations
            public async Task<ResponseDTO> GetTopicByIdAsync(Guid topicId)
            {
                try
                {
                    var topic = await _unitOfWork.Topic.GetByIdAsync(topicId);
                    if (topic == null)
                        return new ResponseDTO("Not found the topic", StatusCodeEnum.NotFound, false);

                    return new ResponseDTO("Get topic successfully", StatusCodeEnum.OK, true, topic);
                }
                catch (Exception ex)
                {
                    return new ResponseDTO($"Error: {ex.Message}", StatusCodeEnum.InteralServerError, false);
                }
            }

            public async Task<ResponseDTO> AddTopicToCourseAsync(Guid courseId, CreateTopicDTO topicDto)
            {
                try
                {
                    var course = await _unitOfWork.Course.GetByIdAsync(courseId);
                    if (course == null)
                        return new ResponseDTO("Not found the course", StatusCodeEnum.NotFound, false);

                    var topic = new Topic
                    {
                        Id = Guid.NewGuid(),
                        TopicName = topicDto.TopicName,
                        CourseId = courseId,
                        IsDeleted = false,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.Topic.AddAsync(topic);
                    await _unitOfWork.SaveChangeAsync();

                    return new ResponseDTO("Add successfully", StatusCodeEnum.Created, true);
                }
                catch (Exception ex)
                {
                    return new ResponseDTO($"Error: {ex.Message}", StatusCodeEnum.InteralServerError, false);
                }
            }

            public async Task<ResponseDTO> UpdateTopicAsync(Guid topicId, UpdateTopicDTO topicDto)
            {
                try
                {
                    var topic = await _unitOfWork.Topic.GetByIdAsync(topicId);
                    if (topic == null)
                        return new ResponseDTO("Not found", StatusCodeEnum.NotFound, false);

                    topic.TopicName = topicDto.TopicName;
                    topic.IsActive = topicDto.IsActive;
                    topic.IsDeleted = topicDto.IsDeleted;
                    topic.UpdatedAt = DateTime.UtcNow;

                    await _unitOfWork.Topic.UpdateAsync(topic);
                    await _unitOfWork.SaveChangeAsync();

                    return new ResponseDTO("Update topic successfully", StatusCodeEnum.OK, true);
                }
                catch (Exception ex)
                {
                    return new ResponseDTO($"Error: {ex.Message}", StatusCodeEnum.InteralServerError, false);
                }
            }

            public async Task<ResponseDTO> DeleteTopicAsync(Guid topicId)
            {
                try
                {
                    var topic = await _unitOfWork.Topic.GetByIdAsync(topicId);
                    if (topic == null)
                        return new ResponseDTO("Not found", StatusCodeEnum.NotFound, false);

                    topic.IsDeleted = true;
                    topic.UpdatedAt = DateTime.UtcNow;

                    await _unitOfWork.Topic.UpdateAsync(topic);
                    await _unitOfWork.SaveChangeAsync();

                    return new ResponseDTO("Delete topic successfully", StatusCodeEnum.OK, true);
                }
                catch (Exception ex)
                {
                    return new ResponseDTO($"Error: {ex.Message}", StatusCodeEnum.InteralServerError, false);
                }
            }

            public async Task<ResponseDTO> GetExerciseByIdAsync(Guid exerciseId)
            {
                try
                {
                    var exercise = await _unitOfWork.Exercise.GetByIdAsync(exerciseId);
                    if (exercise == null || exercise.IsDeleted)
                        return new ResponseDTO("Not found", StatusCodeEnum.NotFound, false);

                    return new ResponseDTO("Get exercise successfully", StatusCodeEnum.OK, true, exercise);
                }
                catch (Exception ex)
                {
                    return new ResponseDTO($"Error: {ex.Message}", StatusCodeEnum.InteralServerError, false);
                }
            }

            public async Task<ResponseDTO> AddExerciseToTopicAsync(Guid topicId, CreateExerciseDTO exerciseDto)
            {
                try
                {
                    var topic = await _unitOfWork.Topic.GetByIdAsync(topicId);
                    if (topic == null || topic.IsDeleted)
                        return new ResponseDTO("Not found", StatusCodeEnum.NotFound, false);

                    var exercise = new Exercise
                    {
                        Id = Guid.NewGuid(),
                        Content = exerciseDto.Content,
                        TopicId = topicId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    await _unitOfWork.Exercise.AddAsync(exercise);
                    await _unitOfWork.SaveChangeAsync();

                    return new ResponseDTO("Add exercise successfully",StatusCodeEnum.Created , true);
                }
                catch (Exception ex)
                {
                    return new ResponseDTO($"Error: {ex.Message}", StatusCodeEnum.InteralServerError, false);
                }
            }

            public async Task<ResponseDTO> UpdateExerciseAsync(Guid exerciseId, UpdateExerciseDTO exerciseDto)
            {
                try
                {
                    var exercise = await _unitOfWork.Exercise.GetByIdAsync(exerciseId);
                    if (exercise == null || exercise.IsDeleted)
                        return new ResponseDTO("Not found", StatusCodeEnum.NotFound, false);

                    exercise.Content  =  exerciseDto.Content;
                    exercise.UpdatedAt = DateTime.UtcNow;

                    await _unitOfWork.Exercise.UpdateAsync(exercise);
                    await _unitOfWork.SaveChangeAsync();

                    return new ResponseDTO("Update exercise successfully", StatusCodeEnum.OK, true);
                }
                catch (Exception ex)
                {
                    return new ResponseDTO($"Error: {ex.Message}", StatusCodeEnum.InteralServerError, false);
                }
            }

            public async Task<ResponseDTO> DeleteExerciseAsync(Guid exerciseId)
            {
                try
                {
                    var exercise = await _unitOfWork.Exercise.GetByIdAsync(exerciseId);
                    if (exercise == null || exercise.IsDeleted)
                        return new ResponseDTO("Not found", StatusCodeEnum.NotFound, false);

                    exercise.IsDeleted = true;
                    exercise.UpdatedAt = DateTime.UtcNow;

                    await _unitOfWork.Exercise.UpdateAsync(exercise);
                    await _unitOfWork.SaveChangeAsync();

                    return new ResponseDTO("Delete exercise sucessfully", StatusCodeEnum.OK, true);
                }
                catch (Exception ex)
                {
                    return new ResponseDTO($"Error: {ex.Message}", StatusCodeEnum.InteralServerError, false);
                }
            }

            public async Task<ResponseDTO> EnrollCourseAsync(Guid userId, Guid courseId)
            {
                try
                {
                    var user = await _unitOfWork.User.GetByIdAsync(userId);
                    var course = await _unitOfWork.Course.GetByIdAsync(courseId);
                    var courseResponse = await GetCourseByIdAsync(courseId);

                    if (user == null || !courseResponse.IsSuccess)
                        return new ResponseDTO("User or course not found", StatusCodeEnum.NotFound, false);

                    var existingEnrollment = await _unitOfWork.EnrolledCourse
                        .GetByConditionAsync(e => e.UserId == userId && e.CourseId == courseId);

                    if (existingEnrollment != null)
                        return new ResponseDTO("User have enrolled this course", StatusCodeEnum.BadRequest, false);

                 
                    if (course.IsPremium && (!user.IsPremium || (user.PremiumExpiredTime != null && user.PremiumExpiredTime < DateTime.UtcNow)))
                        return new ResponseDTO("Only premium users can enroll in premium courses", StatusCodeEnum.Forbidden, false);

                    var enrolledCourse = new EnrolledCourse
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        CourseId = courseId,
                        IsCompleted = false,
                        ProgressPoints = 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.EnrolledCourse.AddAsync(enrolledCourse);

                    var topics = await _unitOfWork.Topic.GetAllByListAsync(t => t.CourseId == courseId && !t.IsDeleted);
                    foreach (var topic in topics)
                    {
                        var topicResponse = await GetTopicByIdAsync(topic.Id);
                        if (!topicResponse.IsSuccess) continue;

                        var topicProgress = new TopicProgress
                        {
                            Id = Guid.NewGuid(),
                            EnrolledCourseId = enrolledCourse.Id,
                            TopicId = topic.Id,
                            UserId = userId,
                            ProgressPoints = 0,
                            IsCompleted = false,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        await _unitOfWork.TopicProgress.AddAsync(topicProgress);

                        var exercises = await _unitOfWork.Exercise.GetAllByListAsync(e => e.TopicId == topic.Id && !e.IsDeleted);
                        foreach (var exercise in exercises)
                        {
                            var exerciseResponse = await GetExerciseByIdAsync(exercise.Id);
                            if (!exerciseResponse.IsSuccess) continue;

                            var exerciseProgress = new ExerciseProgress
                            {
                                Id = Guid.NewGuid(),
                                EnrolledCourseId = enrolledCourse.Id,
                                ExerciseId = exercise.Id,
                                UserId = userId,
                                ProgressPoints = 0,
                                IsCompleted = false,
                                CreatedAt = DateTime.UtcNow
                            };
                            await _unitOfWork.ExerciseProgress.AddAsync(exerciseProgress);
                        }
                    }

                    await _unitOfWork.SaveChangeAsync();
                    return new ResponseDTO("Enrolled successfully", StatusCodeEnum.Created, true);
                }
                catch (Exception ex)
                {
                    return new ResponseDTO($"Error: {ex.Message}", StatusCodeEnum.InteralServerError, false); 
                }
            }
            public async Task<ResponseDTO> GetEnrolledCourseDetailsAsync(Guid enrolledCourseId)
            {
                try
                {
                    var enrolledCourse = await _unitOfWork.EnrolledCourse.GetByIdAsync(enrolledCourseId);
                    if (enrolledCourse == null)
                        return new ResponseDTO("Not found", StatusCodeEnum.NotFound, false);

                    var course = await _unitOfWork.Course.GetByIdAsync(enrolledCourse.CourseId);
                    var topicsProgress = await _unitOfWork.TopicProgress.GetByEnrolledCourseAsync(enrolledCourseId);

                    var enrolledCourseDetails = new EnrolledCourseDetailsDTO
                    {
                        Course = new CourseDTO
                        {
                            Id = course.Id,
                            CourseName = course.CourseName,
                            Description = course.Description,
                            MaxPoint = course.MaxPoint,
                            IsActive = course.IsActive,
                            LevelId = course.LevelId
                        },
                        Progress = enrolledCourse.ProgressPoints,
                        Topics = topicsProgress.Select(tp => new TopicProgressDTO
                        {
                            Id = tp.TopicId,
                            Progress = tp.ProgressPoints
                        }).ToList()
                    };

                    return new ResponseDTO("Success", StatusCodeEnum.OK, true, enrolledCourseDetails);
                }
                catch (Exception ex)
                {
                    return new ResponseDTO($"Error: {ex.Message}",StatusCodeEnum.InteralServerError , false);
                }
            }

            public async Task<ResponseDTO> SubmitExerciseAsync(Guid exerciseId, Guid userId, decimal earnedPoints)
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        // Lấy Exercise và ExerciseProgress
                        var exercise = await _unitOfWork.Exercise.GetByIdAsync(exerciseId);
                        var exerciseProgress = await _unitOfWork.ExerciseProgress.GetByUserAndExerciseAsync(userId, exerciseId);

                        if (exercise == null || exerciseProgress == null)
                            return new ResponseDTO("Exercise không tồn tại", StatusCodeEnum.NotFound, false);

                        // Cập nhật điểm và trạng thái
                        exerciseProgress.ProgressPoints = Math.Min(exercise.MaxPoint, earnedPoints); 
                        exerciseProgress.IsCompleted = exerciseProgress.ProgressPoints >= exercise.MaxPoint;
                        exerciseProgress.UpdateAt = DateTime.UtcNow;

                        await _unitOfWork.ExerciseProgress.UpdateAsync(exerciseProgress);

                        var allExerciseProgressInTopic = await _unitOfWork.ExerciseProgress.GetByUserAndTopicAsyncz(userId, exercise.TopicId);

                        var topicExercises = await _unitOfWork.ExerciseProgress
                       .GetAllByListAsync(ep =>
                           ep.UserId == userId &&
                           ep.Exercise.TopicId == exercise.TopicId
                       );

                        decimal totalTopicPoints = topicExercises.Sum(ep => ep.ProgressPoints);
                        var topic = await _unitOfWork.Topic.GetByIdAsync(exercise.TopicId);

                        // Cập nhật TopicProgress
                        var topicProgress = await _unitOfWork.TopicProgress
                            .GetByConditionAsync(tp =>
                                tp.UserId == userId &&
                                tp.TopicId == exercise.TopicId
                            );

                        if (topicProgress != null)
                        {
                            topicProgress.ProgressPoints = totalTopicPoints;
                            topicProgress.IsCompleted = totalTopicPoints >= topic.MaxPoint;
                            topicProgress.UpdatedAt = DateTime.UtcNow;
                            await _unitOfWork.TopicProgress.UpdateAsync(topicProgress);
                        }
                        var enrolledCourse = await _unitOfWork.EnrolledCourse
                      .GetByConditionWithIncludesAsync(
                          ec => ec.UserId == userId && ec.CourseId == topic.CourseId,
                          includes: ec => ec.Course 
                      );

                        if (enrolledCourse != null)
                        {
                         
                            var courseTopics = await _unitOfWork.Topic
                                .GetAllByListAsync(t =>
                                    t.CourseId == enrolledCourse.CourseId &&
                                    !t.IsDeleted
                                );

                          
                            bool allTopicsCompleted = true;
                            decimal totalCoursePoints = 0;

                            foreach (var courseTopic in courseTopics)
                            {
                                var tp = await _unitOfWork.TopicProgress
                                    .GetByConditionAsync(t =>
                                        t.UserId == userId &&
                                        t.TopicId == courseTopic.Id
                                    );

                                if (tp == null || !tp.IsCompleted)
                                {
                                    allTopicsCompleted = false;
                                }
                                totalCoursePoints += tp?.ProgressPoints ?? 0;
                            }

                           
                            enrolledCourse.ProgressPoints = Math.Min(totalCoursePoints, enrolledCourse.Course.MaxPoint);
                            enrolledCourse.IsCompleted = allTopicsCompleted;
                            enrolledCourse.UpdatedAt = DateTime.UtcNow;

                            await _unitOfWork.EnrolledCourse.UpdateAsync(enrolledCourse);

                        }
                        var responseData = new SubmitExerciseResponseDTO
                        {
                            // Exercise Progress
                            ExerciseId = exerciseId,
                            ExerciseEarnedPoints = exerciseProgress.ProgressPoints,
                            IsExerciseCompleted = exerciseProgress.IsCompleted,

                            // Topic Progress
                            TopicId = exercise.TopicId,
                            TopicTotalPoints = totalTopicPoints,
                            IsTopicCompleted = topicProgress?.IsCompleted ?? false, // Xử lý null

                            // Enrolled Course
                            CourseId = topic.CourseId,
                            CourseTotalPoints = enrolledCourse?.ProgressPoints ?? 0,
                            IsCourseCompleted = enrolledCourse?.IsCompleted ?? false
                        };

                        await _unitOfWork.SaveChangeAsync();
                        await transaction.CommitAsync();

                        return new ResponseDTO(
                 "Cập nhật điểm thành công",
                 StatusCodeEnum.OK,
                 true,
                 responseData 
             );
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return new ResponseDTO($"Lỗi: {ex.Message}", StatusCodeEnum.InteralServerError, false);
                    }
                }
            }

            public async Task<ResponseDTO> GetAllCoursesAsync()
            {
                try
                {
                    var listCourse =  await _unitOfWork.Course.GetAllByListAsync(c => true && c.IsDeleted == false);
                    return new ResponseDTO("Success",StatusCodeEnum.OK , true, listCourse);

                }
                catch (Exception ex)
                {
                    return new ResponseDTO($"Error: {ex.Message}", StatusCodeEnum.InteralServerError, false);
                }
            }

            public async Task<IEnumerable<Course>> GetAllCourses(string search = "")
            {
                var query = _unitOfWork.Course.GetAll()
                    .Where(c => !c.IsDeleted);

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(c =>
                        c.CourseName.Contains(search) ||
                        c.Description.Contains(search)
                    );
                }

                return await query.ToListAsync();
            }

            public async Task<IEnumerable<Course>> SearchCourses(string keyword)
            {
                return await _unitOfWork.Course
                    .FindAll(c =>
                        c.CourseName.Contains(keyword) ||
                        c.Description.Contains(keyword))
                    .ToListAsync();
            }
            public async Task<ResponseDTO> GetByEnrollcoursebyUserID(Guid userId)
            {
                try
                {
                    var enrollCourse = await _unitOfWork.EnrolledCourse.GetEnrolledCourseByUserIdAsync(userId);
                    return new ResponseDTO("Get EnrollCourse Successfully", StatusCodeEnum.OK, true, enrollCourse);
                }
                catch(Exception ex)
                {
                    return new ResponseDTO($"Error: {ex.Message}", StatusCodeEnum.InteralServerError, false);
                }

            }
            public async Task<ResponseDTO> GetCourseDetailAsync(Guid courseId)
            {
                try
                {
                    var courseQuery = _unitOfWork.Course
                        .GetAll()
                        .Where(c => c.Id == courseId && !c.IsDeleted);

            
                    courseQuery = courseQuery
                        .Include(c => c.Topics.Where(t => !t.IsDeleted))  
                        .ThenInclude(t => t.Exercises.Where(e => !e.IsDeleted)); 

                    var course = await courseQuery.FirstOrDefaultAsync();

                    if (course == null)
                        return new ResponseDTO("Course not found", StatusCodeEnum.NotFound, false);

                    var courseDetail = new CourseDTO
                    {
                        Id = course.Id,
                        CourseName = course.CourseName,
                        Description = course.Description,
                        MaxPoint = course.MaxPoint,
                        IsPremium = course.IsPremium,
                        IsActive = course.IsActive,
                        LevelId = course.LevelId,
                        Topics = course.Topics
                            .Select(t => new TopicDetailDTO
                            {
                                Id = t.Id,
                                TopicName = t.TopicName,
                                MaxPoint = t.MaxPoint,
                                IsActive = t.IsActive,
                                Exercises = t.Exercises
                                    .Select(e => new ExerciseDetailDTO
                                    {
                                        Id = e.Id,
                                        Content = e.Content,
                                        MaxPoint = e.MaxPoint,
                                        IsActive = e.IsActive
                                    }).ToList()
                            }).ToList()
                    };

                    return new ResponseDTO("Course details retrieved successfully", StatusCodeEnum.OK, true, courseDetail);
                }
                catch (Exception ex)
                {
                    return new ResponseDTO($"Error: {ex.Message}", StatusCodeEnum.InteralServerError, false);
                }
            }
            public async Task<ResponseDTO> CheckUserEnrollmentAsync(Guid userId, Guid courseId)
            {
                try
                {
                    var enrollment = await _unitOfWork.EnrolledCourse
                        .GetByConditionAsync(ec =>
                            ec.UserId == userId &&
                            ec.CourseId == courseId);

                    if (enrollment == null)
                        return new ResponseDTO("User has not enrolled in this course", StatusCodeEnum.NotFound, false);

                    return new ResponseDTO("User is enrolled in this course", StatusCodeEnum.OK, true,
                        new { EnrolledCourseId = enrollment.Id });
                }
                catch (Exception ex)
                {
                    return new ResponseDTO($"Error: {ex.Message}", StatusCodeEnum.InteralServerError, false);
                }
            }
        }
    }
}
       
    
