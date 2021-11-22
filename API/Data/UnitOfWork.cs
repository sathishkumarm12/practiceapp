using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.data;
using API.Interfaces;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext dataContext;
        private readonly IMapper mapper;

        public UnitOfWork(DataContext dataContext, IMapper mapper)
        {
            this.dataContext = dataContext;
            this.mapper = mapper;
        }

        public IUserRepository UserRepository => new UserRepository(dataContext, mapper);

        public IMessageRepository MessageRepository => new MessageRepository(dataContext, mapper);

        public ILikesRepository LikesRepository => new LikesRepository(dataContext);

        public async Task<bool> Complete()
        {
            return await dataContext.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return dataContext.ChangeTracker.HasChanges();
        }
    }
}