using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuiQue.Models;

namespace QuiQue

{
    public class QuickQueueContext : DbContext
    {
        public QuickQueueContext(DbContextOptions<QuickQueueContext> options)
                    : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Queue> Queues { get; set; }
    }
}
