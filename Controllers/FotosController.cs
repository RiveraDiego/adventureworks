using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
        public ActionResult Create([Bind(Include = "foto_id,foto_titulo,foto_file,foto_descripcion,foto_fecha_creacion,usuario_id")] foto foto, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                // Verifica el tipo de archivo
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = System.IO.Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    ViewBag.FileError = "Formato de archivo no permitido. Solo se permiten imágenes.";
                    ViewBag.usuario_id = new SelectList(db.usuarios, "usuario_id", "usuario_nombre", foto.usuario_id);
                    return View(foto);
                }

                // Leer el archivo y convertirlo a Base64
                using (var binaryReader = new System.IO.BinaryReader(file.InputStream))
                {
                    byte[] fileBytes = binaryReader.ReadBytes(file.ContentLength);
                    var mimeType = file.ContentType; // Obtiene el tipo MIME del archivo (e.g., "image/png", "image/jpeg")
                    foto.foto_file = $"data:{mimeType};base64," + Convert.ToBase64String(fileBytes);
                }
            }
            else
            {
                ViewBag.FileError = "Por favor, selecciona un archivo de imagen.";
                ViewBag.usuario_id = new SelectList(db.usuarios, "usuario_id", "usuario_nombre", foto.usuario_id);
                return View(foto);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    foto.foto_fecha_creacion = DateTime.Now;
                    db.fotos.Add(foto);
                    db.SaveChanges();
                    // return RedirectToAction("Edit", new { id = foto.foto_id });
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationError in ex.EntityValidationErrors)
                    {
                        foreach (var error in validationError.ValidationErrors)
                        {
                            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                        }
                    }
                }
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
