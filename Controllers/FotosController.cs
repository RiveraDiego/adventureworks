using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using adventureworks.Models;

namespace adventureworks.Controllers
{
    public class FotosController : Controller
    {
        private projectsEntities1 db = new projectsEntities1();

        // GET: fotos
        public ActionResult Index()
        {
            var fotos = db.fotos.Include(f => f.usuario);
            return View(fotos.ToList());
        }

        // GET: fotos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            foto foto = db.fotos.Find(id);
            if (foto == null)
            {
                return HttpNotFound();
            }
            return View(foto);
        }

        // GET: fotos/Create
        public ActionResult Create()
        {
            ViewBag.usuario_id = new SelectList(db.usuarios, "usuario_id", "usuario_nombre");
            return View();
        }

        // POST: fotos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "foto_id,foto_titulo,foto_file,foto_descripcion,foto_fecha_creacion,usuario_id")] foto foto)
        {
            if (ModelState.IsValid)
            {
                db.fotos.Add(foto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.usuario_id = new SelectList(db.usuarios, "usuario_id", "usuario_nombre", foto.usuario_id);
            return View(foto);
        }

        // GET: fotos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            foto foto = db.fotos.Find(id);
            if (foto == null)
            {
                return HttpNotFound();
            }
            ViewBag.usuario_id = new SelectList(db.usuarios, "usuario_id", "usuario_nombre", foto.usuario_id);
            return View(foto);
        }

        // POST: fotos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "foto_id,foto_titulo,foto_file,foto_descripcion,foto_fecha_creacion,usuario_id")] foto foto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(foto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.usuario_id = new SelectList(db.usuarios, "usuario_id", "usuario_nombre", foto.usuario_id);
            return View(foto);
        }

        // GET: fotos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            foto foto = db.fotos.Find(id);
            if (foto == null)
            {
                return HttpNotFound();
            }
            return View(foto);
        }

        // POST: fotos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            foto foto = db.fotos.Find(id);
            db.fotos.Remove(foto);
            db.SaveChanges();
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
