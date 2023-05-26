﻿using Microsoft.Extensions.Logging;
using MMS.DataService.IConfiguration;
using MMS.DataService.IRepository;
using MMS.DataService.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.DataService.Data
{
    public class UnitOfWork: IDisposable,IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly ILogger _logger;

        public IPersonRepository Persons { get;private set; }

        public IMessRepository Messes { get; private set; }

        public IMessMemberRepository MessHaveMembers { get; private set; }

        public UnitOfWork(AppDbContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger<UnitOfWork>();
            Persons = new PersonRepository(context, _logger);
            Messes = new MessRepository(context, _logger);
            MessHaveMembers= new MessMemberRepository(context, _logger);

        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}
