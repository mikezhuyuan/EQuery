using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using EQuery.Utils;
using Dapper;
using Simple.Data;

namespace EQuery.Console
{    






    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }        
        public Class Class { get; set; }
        
        //business logic
        public override string ToString()
        {
            return string.Format("sid: {0}, name: {1}", Id, Name);
        }
    }










    public class Class
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Student> Students { get; set; }

        public override string ToString()
        {
            return string.Format("cid: {0}, name: {1}", Id, Name);
        }
    }








    class MyQueryFactory : QueryFactory
    {
        protected override void OnMapping(IMappingBuilder builder)
        {
            builder.Entity<Class>()
                   //.Table("SchoolClass")
                   .Key(_=>_.Id, "Class_id")
                   .Property(_=>_.Name, "ClassName")                   
                   ;
                   
            builder.Entity<Student>()
                   .Key(_ => _.Id, "Student_id")
                 //.Property(_=>_.Name, "Name") not required
                   .Reference(_ => _.Class, "Class_id")
                   ;
        }
    }




    class Program
    {
        static void Main()
        {
            var connStr = @"Data Source=.;Initial Catalog=test;Integrated Security=True";
                        
            var factory = new MyQueryFactory();
            
            //vs1, vs3
            var query = factory.CreateQuery(connStr);

            Profiler.Begin();





            //1. Get all students. query.All<Student>()

            






            //2. Get student by id. query.Get<Student>(1)

            //
            
            
            
            
            //3. Load class by student. 

            //var stu = query.Get<Student>(1);
            //var cls = query.LoadReference(stu, _ => _.Class);






            //4. Student name starts with 'studentA%'. 

            //var stu = query.All<Student>()
            //    .Where(_ => _.Name.StartsWith("studentA"));



            //query.All<Student>()
            //    .Where(_ => _.Class.Name == "classA");

            




            //6. Order by name query.All<Student>().OrderByDescending(_ => _.Name)

            //7. Paging 

            //query.All<Student>().Skip(1).Take(1);
            //8. Compiled query 
            //var q = query.All<Student>()
            //    .Compile(stus => stus.Skip(1).Take(1));

            //9. Compiled query with parameter query.All<Student>().Compile((IQueryable<Student> stus, int index, int size) => stus.Skip(index).Take(size))

            //10. Performance
            //for (int i = 0; i < 1000;i++ )
            //    query.All<Student>().ToList();
                
            //11. Repository
            //var mock = new MockQuery(new[] { new Student { Id = 1, Name = "MockStudent" } });
            //var repo = new StudentRepository();

            //12. dapper
            //var connection = new SqlConnection(connStr);
            //connection.Open();
            //var stus = connection.Query<Student, Class, Student>("select id = student_id, name from student join class on student.class_id = class_id", (stu, cls) =>{stu.Class=cls;return stu;});
            //foreach (var stu in stus)
            //    WriteLine(stu);

            //13. simple.data
            //var db = Database.OpenConnection(connStr);
                        //var stus = db.Students.FindAll(db.Students.Class_id == 1);
            //foreach (var stu in stus)
            //    WriteLine(stu.Name);            

            WriteLine("\n=========================================");

            WriteLine(Profiler.Statistics());
            
            System.Console.ReadKey();
        }

        static void WriteLine(object obj)
        {
            System.Console.WriteLine(obj);
        }
    }
}
