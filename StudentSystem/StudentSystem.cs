using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentSystem
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public List<Course> recCourses { get; set; }
    }

    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Evaluation { get; set; }
    }

    public interface IStudentManager
    {
        List<Student> GetAllStudents();
        Student GetStudent(int id);
        bool Save(Student student);
        bool Add(Student student);
    }

    public class StudentManager : IStudentManager
    {
        static List<Student> studentList = new List<Student>()
            {
                new Student
                {
                    Id = 1,
                    FirstName = "Student 1",
                    LastName = "Student 1",
                    Age = 24,
                    recCourses = new List<Course>()
                    {
                        new Course
                        {
                            Id = 1,
                            Name = "Web",
                            Description = "Web Design",
                            Evaluation = 4
                        }
                    },
                },

                new Student
                {
                    Id = 2,
                    FirstName = "Student 2",
                    LastName = "Student 2",
                    Age = 24,
                    recCourses = new List<Course>()
                    {
                        new Course
                        {
                            Id = 1,
                            Name = "Web",
                            Description = "Web Design",
                            Evaluation = 4
                        },
                        new Course
                        {
                            Id = 2,
                            Name = "BP",
                            Description = "Basic Programming",
                            Evaluation = 4
                        }
                    },
                }
            };

        public List<Student> GetAllStudents()
        {
            return studentList;
        }


        public Student GetStudent(int id)
        {
            return GetAllStudents().SingleOrDefault(s => s.Id == id);
        }

        public bool Save(Student student)
        {
            Student Student = GetAllStudents().SingleOrDefault(s => s.Id == student.Id);

            if (Student != null)
            {
                Student.LastName = student.LastName;
                Student.FirstName = student.FirstName;
                Student.Age = student.Age;

                foreach (var recCourse in Student.recCourses)
                {
                    Course course = student.recCourses.SingleOrDefault(c => c.Id == recCourse.Id);

                    recCourse.Name = course.Name;
                    recCourse.Description = course.Description;
                    recCourse.Evaluation = course.Evaluation;
                }

                return true;
            }
            
            return false;
        }

        public bool Add(Student student)
        {
            int id = GetAllStudents().Max(s => s.Id) + 1;

            student.Id = id;

            GetAllStudents().Add(student);
            
            return true;
        }
    }
}
