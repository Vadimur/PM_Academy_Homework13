using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DepsWebApp.AppDbContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace DepsWebApp.AppDbContext
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class AuthenticationDbContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }

        public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : base(options)
        {

        }

    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
