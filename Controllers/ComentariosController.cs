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
    public class ComentariosController : Controller
    {
        private projectsEntities1 db = new projectsEntities1();

        // GET: Comentarios
        public ActionResult Index()
        {
            var comentarios = db.comentarios.Include(c => c.foto).Include(c => c.usuario);
            return View(comentarios.ToList());
        }

        // GET: Comentarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comentario comentario = db.comentarios.Find(id);
            if (comentario == null)
            {
                return HttpNotFound();
            }
            return View(comentario);
        }

        // GET: Comentarios/Create
        public ActionResult Create()
        {
            ViewBag.foto_id = new SelectList(db.fotos, "foto_id", "foto_titulo");
            ViewBag.usuario_id = new SelectList(db.usuarios, "usuario_id", "usuario_nombre");
            return View();
        }

        // POST: Comentarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "comentario_id,usuario_id,comentario_subject,comentario_body,foto_id")] comentario comentario)
        {
            if (ModelState.IsValid)
            {
                db.comentarios.Add(comentario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.foto_id = new SelectList(db.fotos, "foto_id", "foto_titulo", comentario.foto_id);
            ViewBag.usuario_id = new SelectList(db.usuarios, "usuario_id", "usuario_nombre", comentario.usuario_id);
            return View(comentario);
        }

        // GET: Comentarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comentario comentario = db.comentarios.Find(id);
            if (comentario == null)
            {
                return HttpNotFound();
            }
            ViewBag.foto_id = new SelectList(db.fotos, "foto_id", "foto_titulo", comentario.foto_id);
            ViewBag.usuario_id = new SelectList(db.usuarios, "usuario_id", "usuario_nombre", comentario.usuario_id);
            return View(comentario);
        }

        // POST: Comentarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "comentario_id,usuario_id,comentario_subject,comentario_body,foto_id")] comentario comentario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comentario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.foto_id = new SelectList(db.fotos, "foto_id", "foto_titulo", comentario.foto_id);
            ViewBag.usuario_id = new SelectList(db.usuarios, "usuario_id", "usuario_nombre", comentario.usuario_id);
            return View(comentario);
        }

        // GET: Comentarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            comentario comentario = db.comentarios.Find(id);
            if (comentario == null)
            {
                return HttpNotFound();
            }
            return View(comentario);
        }

        // POST: Comentarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            comentario comentario = db.comentarios.Find(id);
            db.comentarios.Remove(comentario);
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
