namespace JsonFormattingAssignmentTest.TestCase1
{
    public class Course
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public Instructor Supervisor { get; set; }
        public Instructor Teacher { get; set; }
        public List<Topic> Topics { get; set; }
        public double Fees { get; set; }
        public List<AdmissionTest> Tests { get; set; }
        public string[] Tags { get; set; }
        public DateTime? StartDate { get; set; }
    }
}
