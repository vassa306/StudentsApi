using Microsoft.EntityFrameworkCore;

namespace studentsapi.Tests.TestData
{
    public abstract class DBTestBase
    {
        protected Data.CollegeDBContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<Data.CollegeDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
           var context = new Data.CollegeDBContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            TestDataHelper.SeedStudents(context);
            return context;
        }
    }
}
