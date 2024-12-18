﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using adventureworks.Models;
using adventureworks.Utils;
using PagedList;

namespace adventureworks.Controllers
{
    public class FotosController : Controller
    {
        private projectsEntities1 db = new projectsEntities1();

        // GET: fotos
        public ActionResult Index()
        {
            try
            {
                var fotos = db.fotos
                .Include(f => f.usuario) // Incluye los datos relacionados de usuario
                .OrderByDescending(f => f.foto_fecha_creacion) // Ordenar por la fecha de creacion mas reciente
                .Take(8); // Toma solo los ultimos 12 registros
                return View(fotos.ToList());
            }
            catch (Exception ex)
            {
                TempData["message"] = "Ocurrio un error: \n"+ex.Message;
                return View();
            }
        }
        public ActionResult Listado(int? page)
        {
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            var fotos = db.fotos
                .Include(f => f.usuario)
                .OrderByDescending(f => f.foto_fecha_creacion)
                .ToPagedList(pageNumber, pageSize); // Aplicar paginacion
            return View(fotos);
        }

        // GET: fotos/Details/5
        public ActionResult Details(int? id)
        {
            bool opcionesUser = false;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Validar sesion para saber si el usuario puede o no comentar
            if (Request.Cookies["UserSession"] != null)
            {
                opcionesUser = true;
                ViewBag.session = true;
            }
            else
            {
                opcionesUser = false;
                ViewBag.session = false;
            }

                foto foto = db.fotos.Include(f => f.comentarios.Select(c => c.usuario)).FirstOrDefault(f => f.foto_id == id);
            if (foto == null)
            {
                TempData["message"] = "La foto no fue encontrada.";
                TempData["icon"] = "warning";
                return RedirectToAction("Index");
            }
            else
            {
                // Verificamos si el creador de la publicacion es el mismo que ha iniciado sesion
                // para que aparezca o no, el boton de eliminar esta foto
                if (opcionesUser)
                {
                    int usuarioIdSession = Convert.ToInt32(Request.Cookies["UserSession"]["usuario_id"]);
                    if ((int) foto.usuario_id == usuarioIdSession)
                    {
                        opcionesUser = true;
                    }
                    else
                    {
                        opcionesUser = false;
                    }
                }
            }

            ViewBag.opcionesUser = opcionesUser;

            // Ordenar los comentarios de forma DESC, desde el ultimo hecho hasta el primero
            foto.comentarios = foto.comentarios.OrderByDescending(c => c.comentario_id).ToList();
            return View(foto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details([Bind(Include = "comentario_subject,comentario_body,foto_id,usuario_id")] comentario comentario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    comentario.usuario_id = Convert.ToInt32(Request.Cookies["UserSession"]["usuario_id"]);
                    comentario.comentario_fecha_creacion = TimeZoneHelper.ConvertToElSalvadorTime(DateTime.UtcNow);
                    db.comentarios.Add(comentario);
                    db.SaveChanges();
                    TempData["message"] = "Su comentario ha sido publicado con exito.";
                    TempData["icon"] = "success";
                    // return RedirectToAction("Edit", new { id = foto.foto_id });
                    return RedirectToAction("Details", new { id = comentario.foto_id});
                }
                catch (DbEntityValidationException ex)
                {
                    
                    TempData["icon"] = "error";
                    foreach (var validationError in ex.EntityValidationErrors)
                    {
                        foreach (var error in validationError.ValidationErrors)
                        {
                            TempData["message"] = $"Property: {error.PropertyName}, Error: {error.ErrorMessage}; ";
                            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                        }
                    }
                    return RedirectToAction("Details", new { id = comentario.foto_id });
                }
            }
            else
            {
                TempData["message"] = "Ocurrio un error desconocido.";
                TempData["icon"] = "error";
                return RedirectToAction("Details", new { id = comentario.foto_id });
            }

            return RedirectToAction("Details", new { id = comentario.foto_id });
        }

            // GET: fotos/Create
        public ActionResult Create()
        {
            // Validar sesion
            if (Request.Cookies["UserSession"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            return View();
        }

        // POST: fotos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "foto_id,foto_titulo,foto_file,foto_descripcion,foto_fecha_creacion")] foto foto, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                // Verifica el tipo de archivo
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = System.IO.Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    ViewBag.FileError = "Formato de archivo no permitido. Solo se permiten imágenes.";
                    TempData["message"] = "Formato de archivo no permitido. Solo se permiten imágenes.";
                    TempData["icon"] = "warning";
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
                TempData["message"] = "Por favor, selecciona un archivo de imagen.";
                TempData["icon"] = "warning";
                return View(foto);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    foto.foto_fecha_creacion = TimeZoneHelper.ConvertToElSalvadorTime(DateTime.UtcNow);
                    foto.usuario_id = Convert.ToInt32(Request.Cookies["UserSession"]["usuario_id"]);
                    db.fotos.Add(foto);
                    db.SaveChanges();
                    // return RedirectToAction("Edit", new { id = foto.foto_id });
                    TempData["message"] = $"Foto ({foto.foto_titulo}) creada con exito.";
                    TempData["icon"] = "success";
                    return RedirectToAction("Edit", new {id = foto.foto_id});
                }
                catch (DbEntityValidationException ex)
                {
                    TempData["message"] = " ";
                    TempData["icon"] = "warning";
                    foreach (var validationError in ex.EntityValidationErrors)
                    {
                        foreach (var error in validationError.ValidationErrors)
                        {
                            TempData["message"] += $"Property: {error.PropertyName}, Error: {error.ErrorMessage}; ";
                            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                        }
                    }
                    return View();
                }
            }

            return View(foto);
        }

        // GET: fotos/Edit/5
        public ActionResult Edit(int? id)
        {
            foto foto = db.fotos.Find(id);
            // Validar sesion
            if (Request.Cookies["UserSession"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                int cookieUserId = Convert.ToInt32(Request.Cookies["UserSession"]["usuario_id"]);
                if (foto.usuario_id != cookieUserId)
                {
                    return RedirectToAction("Index");
                }
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
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
        public ActionResult Edit([Bind(Include = "foto_id,foto_titulo,foto_file,foto_descripcion")] foto foto, HttpPostedFileBase file)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Obtener la foto original desde la base de datos
                    var existingFoto = db.fotos.Find(foto.foto_id);

                    // Actualizar campos editables
                    existingFoto.foto_titulo = foto.foto_titulo;
                    existingFoto.foto_descripcion = foto.foto_descripcion;

                    if (file != null && file.ContentLength > 0)
                    {
                        // Procesar el nuevo archivo de foto solo si se proporciona
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extension = System.IO.Path.GetExtension(file.FileName).ToLower();
                        if (!allowedExtensions.Contains(extension))
                        {
                            ViewBag.FileError = "Formato de archivo no permitido. Solo se permiten imágenes.";
                            return View(existingFoto);
                        }

                        // Convertir el archivo a Base64
                        using (var binaryReader = new System.IO.BinaryReader(file.InputStream))
                        {
                            byte[] fileBytes = binaryReader.ReadBytes(file.ContentLength);
                            var mimeType = file.ContentType;
                            existingFoto.foto_file = $"data:{mimeType};base64," + Convert.ToBase64String(fileBytes);
                        }
                    }

                    db.Entry(existingFoto).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["message"] = $"Informacion de foto ({foto.foto_titulo}) editada con exito.";
                    TempData["icon"] = "success";
                    return RedirectToAction("MisPublicaciones","Usuarios");
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationError in ex.EntityValidationErrors)
                {
                    foreach (var error in validationError.ValidationErrors)
                    {
                        // Mostrar las propiedades y mensajes de error en la consola o TempData
                        TempData["message"] += $"Property: {error.PropertyName}, Error: {error.ErrorMessage}; ";
                        ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }
                }
                TempData["icon"] = "error";
                return View(foto);
            }

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

        [HttpPost]
        public ActionResult EliminarFoto(int id)
        {
            try
            {
                var foto = db.fotos.Find(id);
                if (foto == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Foto no encontrada.");
                }

                db.fotos.Remove(foto);
                db.SaveChanges();

                return new HttpStatusCodeResult(HttpStatusCode.OK, "Foto eliminada correctamente.");
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Ocurrió un error al eliminar la foto: " + ex.Message);
            }
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
