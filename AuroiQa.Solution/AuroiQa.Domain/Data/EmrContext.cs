using AuroiQa.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroiQa.Domain.Data
{
    public class EmrContext : DbContext
    {
        public EmrContext(DbContextOptions<EmrContext> options) : base(options)
        {
        }

        public DbSet<PatientCataractQa> PatientCataractData { get; set; }
        public DbSet<PatientGlaucomaQa> PatientGlaucomaData { get; set; }
    }
}
