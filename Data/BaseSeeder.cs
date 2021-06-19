using BaseCoreAPI.Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BaseCoreAPI.Data
{
    public class BaseSeeder
    {
        private readonly BaseContext _ctx;

        public BaseSeeder(BaseContext ctx)
        {
            _ctx = ctx;
        }

        public void Seed()
        {
            _ctx.Database.EnsureCreated();

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

            _ctx.SaveChanges();
        }
    }
}

