namespace JsonFormattingAssignmentTest.TestCase1
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Salary { get; set; }
        public List<Department> MyDepartmentList { get; set; }
        public List<int> Marks { get; set; }
        public string[] MyArray { get; set; }
    }

    public class Department
    {
        public int DeptId { get; set; } 
        public string DepartmentName { get; set; }
    }
}
