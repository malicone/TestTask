using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TestTask.Models;

namespace TestTask.Controllers
{
    public class PersonController : Controller
    {
        private TestTaskContext db = new TestTaskContext();

        // GET: Person
        public ActionResult Index()
        {
            if ( db.People.Count() == 0 )
            {
                return View( db.People.ToList() );
            }
            else
            {
                // let's find last element in the list (it's like single linked list)
                var currentPerson = db.People.FirstOrDefault( p => p.NextId == null );
                if ( currentPerson == null )// this means we have elements in the list but no last one - it's data consistency error
                {
                    // we can return db.People.ToList() (unordered list) but it's better to catch error just it happens
                    return HttpNotFound( "Last person in the list not found" );
                }
                var orderedList = new List<Person>( db.People.Count() );
                int counter = 0;
                int maxCounterLimit = db.People.Count();
                while ( currentPerson != null )
                {
                    orderedList.Add( currentPerson );
                    currentPerson = db.People.FirstOrDefault( p => p.NextId == currentPerson.Id );
                    counter++;
                    if ( counter > maxCounterLimit )// to be sure we don't get infinite loop
                        break;
                }
                orderedList.Reverse();
                return View( orderedList );
            }
        }

        // GET: Person/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.People.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // GET: Person/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Person/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FirstName,LastName,BirthdayDate")] Person person)
        {
            if (ModelState.IsValid)
            {
                db.People.Add(person);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(person);
        }

        [HttpPost]
        public JsonResult CreateAndReturn( [Bind( Include = "FirstName,LastName,BirthdayDate,BasePersonId,Direction" )] Person person )
        {
            if ( ModelState.IsValid )
            {
                // transaction ensures we have consistent data (links between records)
                using ( DbContextTransaction transaction = db.Database.BeginTransaction() )
                {
                    try
                    {
                        db.People.Add( person );
                        db.SaveChanges();
                        if ( db.People.Count() > 1 )
                        {
                            // now we setup links between records (to be able show them in defined order)
                            // it's like single linked list
                            if ( person.Direction == InsertDirection.Top )
                            {
                                person.NextId = person.BasePersonId;
                                var prevPerson = db.People.FirstOrDefault( p => p.NextId == person.BasePersonId );
                                if ( prevPerson != null )
                                {
                                    prevPerson.NextId = person.Id;
                                    db.Entry( prevPerson ).State = EntityState.Modified;
                                }
                            }
                            else// Bottom
                            {
                                var basePerson = db.People.Single( p => p.Id == person.BasePersonId );                                
                                var nextPerson = db.People.FirstOrDefault( p => p.Id == basePerson.NextId );
                                if ( nextPerson != null )
                                {
                                    person.NextId = nextPerson.Id;
                                    db.Entry( nextPerson ).State = EntityState.Modified;
                                }
                                basePerson.NextId = person.Id;
                                db.Entry( basePerson ).State = EntityState.Modified;
                            }
                            db.Entry( person ).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        transaction.Commit();
                    }
                    catch ( Exception ex )
                    {
                        transaction.Rollback();
                        Console.WriteLine( "Error occurred: " + ex.Message );
                    }
                }
                return Json( person, JsonRequestBehavior.AllowGet );
            }
            return null;
        }

        // GET: Person/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.People.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // POST: Person/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,BirthdayDate")] Person person)
        {
            if (ModelState.IsValid)
            {
                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(person);
        }

        // GET: Person/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Person person = db.People.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        // POST: Person/Delete/5
        [HttpPost, ActionName("Delete")]
        // max 20190531: commented out to stop 500 error when ajax request is sent. We can send the token in ajax request and 
        // uncomment the line (see text on the link).
        // https://stackoverflow.com/questions/25574410/post-500-internal-server-error-ajax-mvc
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using ( DbContextTransaction transaction = db.Database.BeginTransaction() )
            {
                try
                {
                    Person person = db.People.Find( id );
                    // we have to adjust links between records after removal (to be able show them in defined order).
                    // it's like single linked list
                    var prevPerson = db.People.FirstOrDefault( p => p.NextId == person.Id );
                    var nextPerson = db.People.FirstOrDefault( p => p.Id == person.NextId );
                    if ( prevPerson != null )
                    {
                        if ( nextPerson == null )
                        {
                            prevPerson.NextId = null;
                        }
                        else
                        {
                            prevPerson.NextId = nextPerson.Id;
                            db.Entry( prevPerson ).State = EntityState.Modified;
                        }
                    }
                    if ( nextPerson != null )
                    {
                        if ( prevPerson != null )
                        {
                            prevPerson.NextId = nextPerson.Id;
                            db.Entry( prevPerson ).State = EntityState.Modified;
                        }
                    }
                    db.People.Remove( person );
                    db.SaveChanges();
                    if ( db.People.Count() == 1 )
                    {
                        var lastPerson = db.People.First();
                        lastPerson.NextId = null;
                        db.Entry( lastPerson ).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    transaction.Commit();
                }
                catch ( Exception ex )
                {
                    transaction.Rollback();
                    Console.WriteLine( "Error occurred: " + ex.Message );
                }
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}