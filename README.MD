# EQuery
A very simple ORM(query only) for .NET and SQL SERVER 2008.

## Features
- Code first style mapping
- Much faster than Entity Framwork
- Compiled query
- Built-in profile

## Example
    // POCO
    public class Assignment
    {
        public int ID { get; set; }
        public string Title { get; set; }        
        public Assignment Parent { get; set; }
        public string Description { get; set; }
        public DateTime UpdateDate { get; set; }

        public IEnumerable<Assignment> Children { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}", ID, Title, Description, UpdateDate);
        }
    }

    class ELabFactory : QueryFactory
    {
        protected override void OnMapping(IMappingBuilder builder)
        {
            builder.Entity<Assignment>() 
                   .Key(_=>_.ID, "Assignment_id")
                   .Reference(_=>_.Parent, "ParentAssignment_id")
                   ;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var connStr = @"Data Source=cns-etdbdevvs1;Initial Catalog=YOUR_DATABASE;Integrated Security=True";

            var factory = new ELabFactory(); //this should be cached

            var query = factory.CreateQuery(connStr);
            
            var a = query.Get<Assignment>(3);
            var p = query.LoadReference(a, _ => _.Parent);
            
            //cache itms
            var itms =
               factory.CreateQuery(connStr).All<Assignment>()
                    .Where(_ => _.Parent.Parent.ID == 1 && _.Title.Contains("english"))                    
					.Take(10)
					.Skip(10)
                    .ToList()
                    ;
			
			foreach(var itm in itms)
				Console.WriteLine(itm);
				
            Console.WriteLine(Profiler.Statistics());
            Console.ReadKey();
        }
    }