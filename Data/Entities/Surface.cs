using System.Collections.Generic;

namespace BaseCoreAPI.Data.Entities
{
    public class Surface
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Task> Tasks { get; set; }
    }
}
