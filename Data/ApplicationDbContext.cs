using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using ProjectCMS.Models;
using System.Security.Cryptography;

namespace ProjectCMS.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {

        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Interactions>()
                .HasOne(x => x.User)
                .WithMany(x => x.Iteractions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            builder.Entity<Comment>()
                .HasOne(x => x.User)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            builder.Entity<Idea>()
                .HasOne(x => x.User)
                .WithMany(x => x.Ideas)
                .HasForeignKey(x => x.UserId);
            builder.Entity<Idea>()
                .HasOne(x => x.Event)
                .WithMany(x => x.Ideas)
                .HasForeignKey(x => x.EvId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            builder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();
            base.OnModelCreating(builder);
            SeedCate(builder);
            SeedDepartment(builder);
            SeedUser(builder);
            SeedEvent(builder);
            SeedIdea(builder);
        }
        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
            builder.Properties<DateOnly>()
                .HaveConversion<DateOnlyConverter>()
                .HaveColumnType("date");

            builder.Properties<DateOnly?>()
                .HaveConversion<NullableDateOnlyConverter>()
                .HaveColumnType("date");
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private void SeedDepartment(ModelBuilder builder)
        {
            builder.Entity<Department>().HasData
                (
                new Department
                {
                    DepId = 1,
                    Name = "Business"
                },
                new Department
                {
                    DepId = 2,
                    Name = "Information technology"
                },
                new Department
                {
                    DepId = 3,
                    Name = "Design"
                }
                );
        }
        private void SeedUser (ModelBuilder builder)
        {
            Random random = new Random();
            CreatePasswordHash("123456", out byte[] passwordHash, out byte[] passwordSalt);
            builder.Entity<User>().HasData
                (
                new User
                {
                    UserId = 1,
                    UserName = "admin",
                    Email = "duongtdgch17587@fpt.edu.vn",
                    DepartmentID = 2,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Phone = "0385524965",
                    DoB = DateTime.Parse("1998-11-22"),
                    Address = "Ha Noi",
                    Avatar = "/images/Avatar.jpg",
                    AddedDate = DateTime.Now,
                    Role = "Admin",
                    Status = "Enable"

                }
                );
            builder.Entity<User>().HasData
                (
                new User
                {
                    UserId = 2,
                    UserName = "qam01",
                    Email = "duongtdgch17587@fpt.edu.vn",
                    DepartmentID = 2,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Phone = "0333804202",
                    DoB = DateTime.Parse("2000-08-10"),
                    Address = "Ha Noi",
                    Avatar = "/images/Avatar.jpg",
                    AddedDate = DateTime.Now,
                    Role = "QAM",
                    Status = "Enable"

                }
                );
            builder.Entity<User>().HasData
                (
                new User
                {
                    UserId = 3,
                    UserName = "qac01",
                    Email = "duongtdgch17587@fpt.edu.vn",
                    DepartmentID = 2,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Phone = "0333804202",
                    DoB = DateTime.Parse("2000-08-10"),
                    Address = "Ha Noi",
                    Avatar = "/images/Avatar.jpg",
                    AddedDate = DateTime.Now,
                    Role = "QAC",
                    Status = "Enable"

                }
                );
            for(int i = 4 ; i < 15; i++) {
                builder.Entity<User>().HasData
                (
                new User
                {
                    UserId = i,
                    UserName = "Staff0" + i.ToString(),
                    Email = "staff0"+i.ToString()+"@fpt.edu.vn",
                    DepartmentID = random.Next(1,4),
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Phone = "0123456789",
                    DoB = DateTime.Parse("1998-11-22"),
                    Address = "Ha Noi",
                    Avatar = "/images/Avatar.jpg",
                    AddedDate = DateTime.Parse("2020-01-01"),
                    Role = "Staff",
                    Status = "Enable"
                }
                );
            }
            

        }
        private void SeedEvent(ModelBuilder builder)
        {
            builder.Entity<Event>().HasData(
                new Event {
                    Id = 1, 
                    Name = "Event 2020",
                    Content = "Event on 2020",
                    First_Closure = DateTime.Parse("2020-12-20"),
                    Last_Closure = DateTime.Parse("2020-12-27") 
                });
            builder.Entity<Event>().HasData(
                new Event
                {
                    Id = 2,
                    Name = "Event 2021",
                    Content = "Event on 2021",
                    First_Closure = DateTime.Parse("2021-12-20"),
                    Last_Closure = DateTime.Parse("2021-12-27")
                });
            builder.Entity<Event>().HasData(
                new Event
                {
                    Id = 3,
                    Name = "Event 2022",
                    Content = "Event on 2022",
                    First_Closure = DateTime.Parse("2022-12-20"),
                    Last_Closure = DateTime.Parse("2022-12-27")
                });
            builder.Entity<Event>().HasData(
                new Event
                {
                    Id = 4,
                    Name = "Event 2023",
                    Content = "Event on 2023",
                    First_Closure = DateTime.Parse("2023-12-20"),
                    Last_Closure = DateTime.Parse("2023-12-27")
                });
        }
        private void SeedCate(ModelBuilder builder)
        {
            builder.Entity<Category>().HasData
                (
                new Category
                {
                    Id = 1,
                    Name = "Teaching",
                    Content = "This is content of this category",
                    AddedDate = DateTime.Parse("2020-01-01")
                }
                ); 
            builder.Entity<Category>().HasData
                (new Category 
                {
                    Id = 2,
                    Name = "Intertaiment",
                    Content = "This is content of this category",
                    AddedDate = DateTime.Parse("2020-01-01")
                }); 
            builder.Entity<Category>().HasData
                (new Category
                {
                    Id = 3,
                    Name = "Infrastructure",
                    Content = "This is content of this category",
                    AddedDate = DateTime.Parse("2020-01-01")
                }); 
            builder.Entity<Category>().HasData(
                new Category { Id = 4, Name = "Sport", Content = "This is content of this category", AddedDate = DateTime.Parse("2020-01-01") }
                );
        }
        private void SeedIdea(ModelBuilder builder)
        {
            Random random = new Random();
            for (int i = 1; i <= 10; i++)
            {
                builder.Entity<Idea>().HasData
                    (
                    new Idea
                    {
                        Id = i,
                        Name = "Idea "+i.ToString()+" for event 2020",
                        Content = "This is new Idea",
                        AddedDate = DateTime.Parse("2020"+"-"+random.Next(1,13).ToString()+"-"+random.Next(1, 29).ToString()),
                        IsAnonymous = false,
                        Vote = random.Next(-10, 31),
                        Viewed = random.Next(1, 15),
                        IdeaFile = "Bang TN.pdf",
                        EvId = 1,
                        CateId= random.Next(1,4),
                        UserId = random.Next(4, 15),
                    }
                    );
            }
            for (int i = 11; i <= 20; i++)
            {
                builder.Entity<Idea>().HasData
                    (
                    new Idea
                    {
                        Id = i,
                        Name = "Idea " + i.ToString() + " for event 2021",
                        Content = "This is new Idea",
                        AddedDate = DateTime.Parse("2021" + "-" + random.Next(1, 13).ToString() + "-" + random.Next(1, 29).ToString()),
                        IsAnonymous = false,
                        Vote = random.Next(-10, 31),
                        Viewed = random.Next(1, 15),
                        IdeaFile = "Bang TN.pdf",
                        EvId = 2,
                        CateId = random.Next(1, 4),
                        UserId = random.Next(4, 15),
                    }
                    );
            }
            for (int i = 21; i <= 30; i++)
            {
                builder.Entity<Idea>().HasData
                    (
                    new Idea
                    {
                        Id = i,
                        Name = "Idea " + i.ToString() + " for event 2022",
                        Content = "This is new Idea",
                        AddedDate = DateTime.Parse("2022" + "-" + random.Next(1, 13).ToString() + "-" + random.Next(1, 29).ToString()),
                        IsAnonymous = false,
                        Vote = random.Next(-10, 31),
                        Viewed = random.Next(1, 15),
                        IdeaFile = "Bang TN.pdf",
                        EvId = 3,
                        CateId = random.Next(1, 4),
                        UserId = random.Next(4, 15),
                    }
                    );
            }
            for (int i = 31; i <= 40; i++)
            {
                builder.Entity<Idea>().HasData
                    (
                    new Idea
                    {
                        Id = i,
                        Name = "Idea " + i.ToString() + " for event 2023",
                        Content = "This is new Idea",
                        AddedDate = DateTime.Parse("2023" + "-" + random.Next(1, 5).ToString() + "-" + random.Next(1, 29).ToString()),
                        IsAnonymous = false,
                        Vote = random.Next(-10, 31),
                        Viewed = random.Next(1, 15),
                        IdeaFile = "Bang TN.pdf",
                        EvId = 4,
                        CateId = random.Next(1, 4),
                        UserId = random.Next(4, 15),
                    }
                    );
            }

        }

        public DbSet<Category> _categories { get; set; }
        public DbSet<Comment> _comments { get; set; }
        public DbSet<Department> _departments { get; set; }
        public DbSet<Event> _events { get; set; }
        public DbSet<Idea> _idea { get; set; }
        public DbSet<Interactions> _interactions { get; set; }
        public DbSet<User> _users { get; set; }
    }

    /// <summary>
    /// Converts <see cref="DateOnly" /> to <see cref="DateTime"/> and vice versa.
    /// </summary>
    public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
    {
        /// <summary>
        /// Creates a new instance of this converter.
        /// </summary>
        public DateOnlyConverter() : base(
                d => d.ToDateTime(TimeOnly.MinValue),
                d => DateOnly.FromDateTime(d))
        { }
    }

    /// <summary>
    /// Converts <see cref="DateOnly?" /> to <see cref="DateTime?"/> and vice versa.
    /// </summary>
    public class NullableDateOnlyConverter : ValueConverter<DateOnly?, DateTime?>
    {
        /// <summary>
        /// Creates a new instance of this converter.
        /// </summary>
        public NullableDateOnlyConverter() : base(
            d => d == null
                ? null
                : new DateTime?(d.Value.ToDateTime(TimeOnly.MinValue)),
            d => d == null
                ? null
                : new DateOnly?(DateOnly.FromDateTime(d.Value)))
        { }


    }
}