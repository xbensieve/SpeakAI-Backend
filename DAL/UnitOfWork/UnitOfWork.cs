using DAL.Data;
using DAL.Entities;
using DAL.GenericRepository.IRepository;
using DAL.GenericRepository.Repository;
using DAL.IRepositories;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SpeakAIContext _context;

        public UnitOfWork(SpeakAIContext context)
        {
            _context = context;
            Course = new CourseRepository(_context);
            User = new UserRepository(_context);
            Level = new LevelRepository(_context);
            UserLevel = new UserLevelRepository(_context);
            Topic = new TopicRepository(_context);
            Exercise = new ExerciseRepository(_context);
            EnrolledCourse = new EnrolledCourseRepository(_context);
            TopicProgress = new TopicProgressRepository(_context);
            ExerciseProgress = new ExerciseProgressRepository(_context);
            RefreshToken = new RefreshTokenRepository(_context);
            ChatMessages = new ChatRepository(_context);
            Order = new OrderRepository(_context);
            Transaction = new TransactionRepository(_context);
            Voucher = new VoucherRepository(_context);
        }

        public ICourseRepository Course { get; private set; }
        public IUserRepository User { get; private set; }
        public ILevelRepository Level { get; private set; }
        public IUserLevelRepository UserLevel { get; private set; }
        public ITopicRepository Topic { get; private set; }
        public IExerciseRepository Exercise { get; private set; }
        public IEnrolledCourseRepository EnrolledCourse { get; private set; }
        public ITopicProgressRepository TopicProgress { get; private set; }
        public IExerciseProgressRepository ExerciseProgress { get; private set; }

        public IRefreshTokenRepository RefreshToken { get; private set; }
        public IChatRepository ChatMessages { get; private set; }
        public IOrderRepository Order { get; private set; }
        public ITransactionRepository Transaction { get; private set; }

        public IVoucherRepository Voucher { get; private set; }


        public async Task<IDbContextTransaction> BeginTransactionAsync(System.Data.IsolationLevel isolationLevel)
        {
            return await _context.Database.BeginTransactionAsync(isolationLevel);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public bool SaveChange()
        {
            return _context.SaveChanges() > 0;
        }

        public async Task<bool> SaveChangeAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
