namespace BaseCoreAPI.Data.Entities
{
    public class Task
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Repeating { get; set; }
        public int Frequency { get; set; }
    }
}
