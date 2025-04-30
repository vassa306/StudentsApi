namespace studentsapi.Model
{
    public class CollegeRepository
    {
        public static List<Student> Students { get; set; } = new ()
                {
                    new() {
                        Id = 1,
                        Name = "John",
                        Email = "john.doe@seznam.cz",
                        Address = "123 Main St, Prague"
                    },
                    new() {
                        Id = 2,
                        Name = "Jane",
                        Email = "jane.smith@gmail.com",
                        Address = "456 Elm St, Brno"
                    }
                };
    }
}
