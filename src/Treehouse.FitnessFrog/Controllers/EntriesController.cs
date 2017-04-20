using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        public ActionResult Add()
        {
            var entry = new Entry
            {
                Date = DateTime.Today

            };
            ViewBag.ActivitiesSelectListItems = new SelectList(Data.Data.Activities, "Id", "Name");

            return View(entry);
        }
        [HttpPost]
        public ActionResult Add(Entry entry)
        {
            //validate entry
            ValidateEntry(entry);
            //check if the entry is valid
            if (ModelState.IsValid)
            {
                _entriesRepository.AddEntry(entry);
                return RedirectToAction("Index");
            }
            PopulateActivityListItems();
            return View(entry);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //TODO  get the requested entry from the repository
            Entry entry = _entriesRepository.GetEntry((int)id);
            //TODO return "entry" Not found if the entry wasn't found
            if( entry == null)
            {
               return HttpNotFound();
            }
            PopulateActivityListItems();
            return View(entry);
        }

        [HttpPost]
        public ActionResult Edit(Entry entry)
        {
            ValidateEntry(entry);
            //if the entry is valid
            if (ModelState.IsValid)
            {
                //update the entry using repository
                _entriesRepository.UpdateEntry(entry);
                //redirect to index view, where al entries are listed
                return RedirectToAction("Index");
            }
            //TODO populate the entry list items viewbag
            PopulateActivityListItems();
            return View(entry);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //retrieve entry for the provided if parameter is value
            Entry entry = _entriesRepository.GetEntry((int)id);

            if(entry == null)
            {
                return HttpNotFound();
            }
            return View(entry);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            //delete
            _entriesRepository.DeleteEntry(id);
            //return to index
            return RedirectToAction("Index");
        }

        private void ValidateEntry(Entry entry)
        {
            //if there aren't any "Duration" field validation errors
            //then make sure that the duration is greater than "0"
            if (ModelState.IsValidField("Duration") && entry.Duration <= 0)
            {
                ModelState.AddModelError("Duration", "La duracion debe ser mayor a 0");
            }
        }
        private void PopulateActivityListItems()
        {
            ViewBag.ActivitiesSelectListItems = new SelectList(Data.Data.Activities, "Id", "Name");
        }
    }
}