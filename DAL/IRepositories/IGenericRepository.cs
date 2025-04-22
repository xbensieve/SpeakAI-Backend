using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepositories
{
    public interface IGenericRepository<T>  where T : class
    {
        T GetById(string id);
        Task<T> GetByIdAsync(Guid id);
        Task<T> GetByEmailAsync(string email);
        IQueryable<T> GetAll();
        IQueryable<T> FindAll(Expression<Func<T, bool>> expression);
        IEnumerable<T> FindAllAsync(Expression<Func<T, bool>> expression);
        Task<T> AddAsync(T entity);
        T Add(T entity);
        Task<T> UpdateAsync(T entity);
        bool Delete(T entity);
        Task<List<T>> GetAllByListAsync(Expression<Func<T, bool>> expression);
        void UpdateRange(List<T> entity);
        void RemoveRange(List<T> entity);
        Task DeleteAsync(Guid id);
        IQueryable<T> FindAllWithIncludes(Expression<Func<T, bool>>? expression = null, params Expression<Func<T, object>>[] includes);
        Task<T> GetByConditionWithIncludesAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes);
        Task<T> GetByConditionAsync(Expression<Func<T, bool>> expression);
        T GetByCondition(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> FindProductAsync(Expression<Func<T, bool>> expression);
        Task<IEnumerable<TopicProgress>> GetByEnrolledCourseAsync(Guid enrolledCourseId);
        Task<ExerciseProgress> GetByUserAndExerciseAsync(Guid userId, Guid exerciseId);
        Task<List<ExerciseProgress>> GetByUserAndTopicAsyncz(Guid userId, Guid topicId);
        Task<TopicProgress> GetByUserAndTopicAsync(Guid userId, Guid topicId);
       
        Task<IEnumerable<Exercise>> GetByTopicIdAsync(Guid topicId);
        Task<IEnumerable<ChatMessages>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<EnrolledCourse>> GetEnrolledCourseByUserIdAsync(Guid UserId);

    }
}
