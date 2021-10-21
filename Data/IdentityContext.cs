using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using BaseCoreAPI.Data.Entities;

namespace BaseCoreAPI.Data
{
    public class IdentityContext : IdentityDbContext<User>, IPersistedGrantDbContext
    {
        private readonly  IOptions<OperationalStoreOptions> _operationalStoreOptions;

        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
        }

        public IdentityContext(DbContextOptions<IdentityContext> options, IOptions<OperationalStoreOptions> storeOptions): base(options) 
        {
            _operationalStoreOptions = storeOptions;
        }

        public DbSet<PersistedGrant> PersistedGrants { get; set; }
        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

        public Task<int> SaveChangesAsync()
        {
            return new Task<int>(DoNothing);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ConfigurePersistedGrantContext(_operationalStoreOptions.Value);
        }

        private int DoNothing()
        {
            return 0;
        }
    }
}
