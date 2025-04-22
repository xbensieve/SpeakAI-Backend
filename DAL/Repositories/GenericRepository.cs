using DAL.Data;
using DAL.Entities;
using DAL.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly SpeakAIContext _context;
        protected readonly DbSet<T> _dbSet;
        public GenericRepository(SpeakAIContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public bool Delete(T entity)
        {
            _dbSet.Remove(entity);
            return true;
        }

        public IQueryable<T> FindAll(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression).AsQueryable();
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable();
        }

        public T GetById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return entity;
        }

        public IEnumerable<T> FindAllAsync(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression).AsEnumerable();
        }

        public T Add(T entity)
        {
            _dbSet.Add(entity);
            return entity;
        }

        public Task<List<T>> GetAllByListAsync(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression).ToListAsync();
        }

        public void UpdateRange(List<T> entity)
        {
            _dbSet.UpdateRange(entity);
        }

        public void RemoveRange(List<T> entity)
        {
            _dbSet.RemoveRange(entity);
        }
        public async Task<T> GetByConditionAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.FirstOrDefaultAsync(expression);
        }

        public T GetByCondition(Expression<Func<T, bool>> expression)
        {
            return _dbSet.FirstOrDefault(expression);
        }
        public async Task<IEnumerable<T>> FindProductAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.Where(expression).ToListAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                Delete(entity);
                await _context.SaveChangesAsync();
            }
        }
        public IQueryable<T> FindAllWithIncludes(Expression<Func<T, bool>>? expression = null, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            if (expression != null)
            {
                query = query.Where(expression);
            }

            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }

        public async Task<T> GetByConditionWithIncludesAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.Where(expression);
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<TopicProgress>> GetByEnrolledCourseAsync(Guid enrolledCourseId)
        {
            return await _context.TopicProgresses
                .Where(tp => tp.EnrolledCourseId == enrolledCourseId)
                .ToListAsync();
        }
        public async Task<IEnumerable<EnrolledCourse>> GetEnrolledCourseByUserIdAsync(Guid UserId)
        {
            return await _context.EnrolledCourses.Where(tp => tp.UserId == UserId).ToListAsync();
        }
        public async Task<ExerciseProgress> GetByUserAndExerciseAsync(Guid userId, Guid exerciseId)
        {
             var entity = await _context.ExerciseProgresses
                .FirstOrDefaultAsync(ep => ep.UserId == userId && ep.ExerciseId == exerciseId);
          if (entity !=null)
            {
                return entity;
            }
            return null;
        }
        public async Task<List<ExerciseProgress>> GetByUserAndTopicAsyncz(Guid userId, Guid topicId)
        {
            return await _context.ExerciseProgresses
                .Include(ep => ep.Exercise)
                .Where(ep => ep.UserId == userId
                    && ep.Exercise.TopicId == topicId
                    && !ep.Exercise.IsDeleted)
                .ToListAsync(); 
        }
        public async Task<TopicProgress> GetByUserAndTopicAsync(Guid userId, Guid topicId)
        {
            return await _context.TopicProgresses
                .FirstOrDefaultAsync(tp =>
                    tp.UserId == userId &&
                    tp.TopicId == topicId
                ); 
        }
        public async Task<IEnumerable<Exercise>> GetByTopicIdAsync(Guid topicId)
        {
            return await _context.Exercises
                .Where(e => e.TopicId == topicId && !e.IsDeleted)
                .ToListAsync();
        }
        public async Task<IEnumerable<ChatMessages>> GetByUserIdAsync(Guid userId)
        {
            return await _context.ChatMessages.Where(tp => tp.UserId == userId)
                .ToListAsync();
        }
        public async Task<T> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(e => EF.Property<string>(e, "Email") == email);
        }

    }
}
