using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;


namespace ReniBot.Repository
{
    public class UnitOfWork : IDisposable
    {
        private readonly BotContext _context;
        //private NodeRepository _nodeRepository;
        //private SettingRepository _settingRepository;
        private readonly ApplicationRepository _applicationRepository;
        private readonly TemplateRepository _templateRepository;
        private readonly BrainRepository _brainRepository;
        private readonly AimlDocRepository _aimlDocRepository;
        private readonly BotUserRepository _botUserRepository;
        private readonly BotUserResultRepostiory _botUserResultRepository;
        private readonly BotUserPredicateRepository _botUserPredicateRepository;
        private readonly BotUserRequestRepository _botUserRequestRepository;

        public UnitOfWork()
        {
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder();
            _context = new BotContext(builder.Options);
            //_nodeRepository = new NodeRepository(_context);
            //_settingRepository = new SettingRepository(_context);
            _applicationRepository = new ApplicationRepository(_context);
            _templateRepository = new TemplateRepository(_context);
            _brainRepository = new BrainRepository(_context);
            _aimlDocRepository = new AimlDocRepository(_context);
            _botUserRepository = new BotUserRepository(_context);
            _botUserResultRepository = new BotUserResultRepostiory(_context);
            _botUserPredicateRepository = new BotUserPredicateRepository(_context);
            _botUserRequestRepository = new BotUserRequestRepository(_context);
        }

        //public NodeRepository NodeRepository { get { return _nodeRepository; } }
        //public SettingRepository SettingRepository { get { return _settingRepository; } }
        public ApplicationRepository ApplicationRepository { get { return _applicationRepository; } }
        public TemplateRepository TemplateRepository { get { return _templateRepository; } }
        public BrainRepository BrainRepository { get { return _brainRepository; } }
        public AimlDocRepository AimlDocRepository { get { return _aimlDocRepository; } }
        public BotUserRepository BotUserRepository { get { return _botUserRepository; } }
        public BotUserResultRepostiory BotUserResultRepository { get { return _botUserResultRepository; } }
        public BotUserPredicateRepository BotUserPredicateRepository { get { return _botUserPredicateRepository; } }
        public BotUserRequestRepository BotUserRequestRepository { get { return _botUserRequestRepository; } }

        public int Save()
        {
            int result;
            try
            {
                result = _context.SaveChanges();
            }
            catch (ValidationException)
            {
                throw;
            }

            return result;
        }

        public BotContext Context
        {

            get
            {
                return _context;
            }
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
