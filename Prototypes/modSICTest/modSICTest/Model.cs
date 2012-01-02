using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace modSICTest
{
    class Model
    {
        private DatabaseEntities db;

        public Model()
        {
            Console.WriteLine(AppDomain.CurrentDomain.GetData("DataDirectory"));
            var connectionString = ConfigurationManager.ConnectionStrings["DatabaseEntities"].ToString();
            db = new DatabaseEntities(connectionString);
        }

        public IQueryable<OvalDefinition> GetDefinitionsToCollect(bool test)
        {
            if (test)
            {
                return db.OvalDefinitions.Where(n => n.Active == true).Take(1); 
            }
            else
            {
                return db.OvalDefinitions.Where(n => n.Active == true);
            }
        }

        public void InsertExecution(int idDefinition, DateTime date, long duration, int nTrue, int nFalse, int nError, int nUnknown)
        {
            var o = new Execution()
            {
                Date = date,               
                IdDefinition = idDefinition,
                NTrue = nTrue,
                NFalse = nFalse,
                NError = nError,
                NUnknown = nUnknown,
                Duration = duration
            };

            db.Executions.AddObject(o);
            db.SaveChanges();
        }

        public Execution GetLastExecution(int idDefinition)
        {
            return db.Executions.OrderByDescending(n => n.Date).Where(m => m.IdDefinition == idDefinition).FirstOrDefault();
        }
    }
}
