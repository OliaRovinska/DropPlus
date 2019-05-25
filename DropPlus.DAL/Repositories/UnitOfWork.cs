using System;
using DropPlus.DAL.Entities;
using DropPlus.DAL.Interfaces;

namespace DropPlus.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DropPlusDbContext _context;

        private IRepository<AppUserResort> _appUserResortRepository;
        private IRepository<Resort> _resortRepository;
        private IRepository<ResortTag> _resortTagRepository;
        private IRepository<Review> _reviewRepository;
        private IRepository<Tag> _tagRepository;

        public UnitOfWork(DropPlusDbContext context)
        {
            _context = context;
        }

        public IRepository<AppUserResort> AppUserResorts => _appUserResortRepository ?? (_appUserResortRepository = new Repository<AppUserResort>(_context.AppUserResorts));
        public IRepository<Resort> Resorts => _resortRepository ?? (_resortRepository = new Repository<Resort>(_context.Resorts));
        public IRepository<ResortTag> ResortTags => _resortTagRepository ?? (_resortTagRepository = new Repository<ResortTag>(_context.ResortTags));
        public IRepository<Review> Reviews => _reviewRepository ?? (_reviewRepository = new Repository<Review>(_context.Reviews));
        public IRepository<Tag> Tags => _tagRepository ?? (_tagRepository = new Repository<Tag>(_context.Tags));


        public void Save()
        {
            _context.SaveChanges();
        }

        private bool _disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}