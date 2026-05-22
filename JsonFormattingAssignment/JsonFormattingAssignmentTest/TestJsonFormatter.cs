using JsonFormattingAssignment;
using JsonFormattingAssignmentTest.TestCase1;
using System.Net;

namespace JsonFormattingAssignmentTest
{
    public class TestJsonFormatter
    {
        Course _course;

        [SetUp]
        public void Setup()
        {
            _course = new Course();
            _course.Title = "Asp.net";
            _course.Fees = 30000;
            _course.Teacher = new Instructor()
            {
                Name = "Jalaluddin"
            };
            Address presentAddress = new Address();
            presentAddress.Street = "101";
            presentAddress.City = "Dhaka";
            presentAddress.Country = "Bangladesh";

            Address permanentAddress = new Address();
            permanentAddress.Street = "102";
            permanentAddress.City = "Rangpur";
            permanentAddress.Country = "Bangladesh";

            _course.Teacher.PresentAddress = presentAddress;
            _course.Teacher.PermanentAddress = permanentAddress;
            _course.StartDate = new DateTime(2022, 12, 1);

            _course.Tests = new List<AdmissionTest>();
            _course.Tags = new string[] { "C#", "HTML", "CSS" };

            string[] stringArray = new string[] { "Hello", "World" };

            List<Course> CourseList = new List<Course>();
            CourseList.Add(_course);
            CourseList.Add(_course);
            CourseList.Add(_course);
            CourseList.Add(_course);

            AdmissionTest admissionTest1 = new AdmissionTest
            {
                StartDateTime = new DateTime(2022, 10, 3, 9, 9, 9),
                EndDateTime = new DateTime(2022, 10, 3, 11, 11, 11),
                TestFees = 100
            };
            AdmissionTest admissionTest2 = new AdmissionTest
            {
                StartDateTime = new DateTime(2022, 11, 3, 9, 9, 9),
                EndDateTime = new DateTime(2022, 11, 3, 10, 10, 10),
                TestFees = 150
            };

            _course.Tests = new List<AdmissionTest>();
            _course.Tests.Add(admissionTest1);
            _course.Tests.Add(admissionTest2);

            
        }

        [Test]
        [TestCase("{\"Title\":\"Asp.net\",\"Supervisor\":null,\"Teacher\":{\"Name\":\"Jalaluddin\",\"Email\":null,\"PresentAddress\":{\"Street\":\"101\",\"City\":\"Dhaka\",\"Country\":\"Bangladesh\"},\"PermanentAddress\":{\"Street\":\"102\",\"City\":\"Rangpur\",\"Country\":\"Bangladesh\"},\"PhoneNumbers\":null},\"Topics\":null,\"Fees\":30000,\"Tests\":[{\"StartDateTime\":\"10/3/2022 9:09:09 AM\",\"EndDateTime\":\"10/3/2022 11:11:11 AM\",\"TestFees\":100},{\"StartDateTime\":\"11/3/2022 9:09:09 AM\",\"EndDateTime\":\"11/3/2022 10:10:10 AM\",\"TestFees\":150}],\"Tags\":[\"C#\",\"HTML\",\"CSS\"],\"StartDate\":\"12/1/2022 12:00:00 AM\"}")]
        public void Test1(string expectedResult)
        {
            string formattedString = JsonFormatter.Convert(_course);
            Assert.AreEqual(formattedString, expectedResult);
        }
    }
}