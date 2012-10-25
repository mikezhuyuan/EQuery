//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using EQuery.Utils;
//using EQuery;

//namespace EQuery.Console
//{
//    public class TestType
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }
//        public DateTime Date { get; set; }
//        public double Float { get; set; }
//    }

//    public class Test
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }
//    }

//    public class Student
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }
//        public Class Class { get; set; }
//    }

//    public class Class
//    {
//        public int Id { get; set; }
//        public string Name { get; set; }
//        public IEnumerable<Student> Students { get; set; }
//    }



//    class MyQueryFactory : QueryFactory
//    {
//        protected override void OnMapping(IMappingBuilder builder)
//        {
//            builder.Entity<TestType>()
//                   //.Key(_=>_.Id, "Id")
//                   ;

//            builder.Entity<Test>();

//            builder.Entity<Class>()
//                   .Key(_=>_.Id, "Class_id")
//                   .Property(_=>_.Name, "ClassName")                   
//                   ;

//            builder.Entity<Student>()
//                   .Key(_ => _.Id, "Student_id")
//                   .Reference(_ => _.Class, "Class_id")
//                   ;

//        }
//    }

//    class Program
//    {
//        static void Main(string[] args)
//        {

//            var connStr = @"Data Source=.;Initial Catalog=test;Integrated Security=True";

//            var factory = new MyQueryFactory();

//            var query = factory.CreateQuery(connStr);

//            Profiler.Begin();

//            //for (var i = 0; i < 1000; i++)

//            var func = 
//                query.All<Student>().Compile(stus =>
//                    stus.OrderBy(_ => _.Id)
//                );
            
//            //query.All<Student>() // Map to `student` table
//            //     .Where(_ => _.Class.Name.StartsWith("MS")) // `WHERE` Condition
//            //     .OrderBy(_=>_.Id)
//            //     .ToList();


//            var r = func();
       





//            //for (var i = 0; i < 1000; i++)
//            //{
//            //    var stu = query.Get<Student>(1);
//            //    //var cls = query.LoadReference(stu, _ => _.Class);
//            //}

//            //var cls = query.Get<Class>(1);
//            //var stus = query.LoadCollection(cls, _ => _.Students);

//            System.Console.WriteLine(Profiler.Statistics());
            
//            System.Console.ReadKey();
//        }
//    }
//}
