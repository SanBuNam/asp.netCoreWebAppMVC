using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp.netCoreWebAppMVC.Models
{
    public class TestStudentRepository : IStudentRepository
    {
        public List<Student> DataSource()
        {
            return new List<Student>()
            {
                new Student() {StudentID=101, Name = "James", Branch = "CSE", Section = "A", Gender = "Male"},
                new Student() {StudentID=102, Name = "James2", Branch = "CSE2", Section = "A2", Gender = "Male"},
                new Student() {StudentID=103, Name = "James3", Branch = "CSE3", Section = "A3", Gender = "Male"},
                new Student() {StudentID=104, Name = "James4", Branch = "CSE4", Section = "A4", Gender = "Male"},
                new Student() {StudentID=105, Name = "James5", Branch = "CSE5", Section = "A5", Gender = "Male"}
            };
        }

        public Student GetStudentById(int StudentId)
        {
            return DataSource().FirstOrDefault(e => e.StudentID == StudentId);
        }
    }
}
