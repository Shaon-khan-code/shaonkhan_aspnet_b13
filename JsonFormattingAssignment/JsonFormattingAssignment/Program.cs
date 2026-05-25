using JsonFormattingAssignment;

namespace Assignment_1;

class Program
{
    static void Main()
    {
        Course course = new Course
        {
            //Id = null,    
            Title = "Asp.net",
            Fees = 30000,
            Teacher = new Instructor
            {
                Name = "Jalaluddin",
                Email = "jalal@example.com",
                PresentAddress = new Address { Street = "Street 101", City = "Dhaka", Country = "Bangladesh" },
                PermanentAddress = new Address { Street = "Street 102", City = "Rangpur", Country = "Bangladesh" },
                PhoneNumbers = new List<Phone>
                {
                    new Phone { Number = "123456", Extension = "01", CountryCode = "+880" },
                    new Phone { Number = "654321", Extension = "02", CountryCode = "+880" }
                }
            },
            Topics = new List<Topic>
            {
                new Topic
                {
                    Title = "C# Basics",
                    Description = "Introduction to C#",
                    Sessions = new List<Session>
                    {
                        new Session { DurationInHour = 2, LearningObjective = "Learn syntax" },
                        new Session { DurationInHour = 3, LearningObjective = "OOP concepts" }
                    }
                }
            },
            Tests = new List<AdmissionTest>
            {
                new AdmissionTest
                {
                    StartDateTime = DateTime.Now,
                    EndDateTime = DateTime.Now.AddHours(2),
                    TestFees = 150

                }
            },
            Tags = ["c#", "Bangla"],
            StartDate = DateTime.Now

        };

        string json = JsonFormatter.Convert(course);
        Console.WriteLine(json);
    }
}
