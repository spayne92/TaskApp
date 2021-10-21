using BaseCoreAPI.Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BaseCoreAPI.Data
{
    public class BaseSeeder
    {
        private readonly BaseContext _ctx;
        private readonly IdentityContext _idCtx;

        public BaseSeeder(BaseContext ctx, IdentityContext idCtx)
        {
            _ctx = ctx;
            _idCtx = idCtx;
        }

        public void Seed()
        {
            _ctx.Database.EnsureCreated();
            _idCtx.Database.EnsureCreated();

            if (!_ctx.Tasks.Any())
            {
                var room = _ctx.Rooms.Where(o => o.Id == 1).FirstOrDefault();
                if (room != null)
                {
                    room.Surfaces = new List<Surface>()
                    {
                        new Surface()
                        {
                            Name = "Counter",
                            Tasks = new List<Task>()
                            {
                                new Task()
                                {
                                    Name = "Cloth Clean",
                                    Repeating = true
                                }
                            }
                        }
                    };
                }
            }

            if (!_idCtx.Users.Any())
            {
                var user = new User(){ UserName = "scott" };
            }

            _ctx.SaveChanges();
            _idCtx.SaveChanges();
        }
    }
}

