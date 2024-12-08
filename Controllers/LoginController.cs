using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using adventureworks.Models;
using adventureworks.Utils;

namespace adventureworks.Controllers
{
    public class LoginController : Controller
    {
        private projectsEntities1 db = new projectsEntities1();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string usuario_codigo, string usuario_contrasena)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Primero buscamos al usuario por el código
                    var user = db.usuarios.FirstOrDefault(u => u.usuario_codigo == usuario_codigo);

                    // Verificamos si el usuario existe y si la contraseña es correcta
                    if (user != null)
                    {
                        if (PasswordHelper.VerifyPassword(usuario_contrasena, user.usuario_contrasena))
                        {
                            // Crea una cookie
                            HttpCookie authCookie = new HttpCookie("UserSession");
                            authCookie.Values["usuario_id"] = user.usuario_id.ToString();
                            authCookie.Values["usuario_nombre"] = user.usuario_nombre;
                            authCookie.Values["usuario_codigo"] = user.usuario_codigo;
                            authCookie.Expires = DateTime.Now.AddHours(1); // Expira en 1 hora
                            Response.Cookies.Add(authCookie);

                            TempData["message"] = "Bienvenid@ " + user.usuario_nombre;
                            TempData["icon"] = "success";

                            return RedirectToAction("Index", "Fotos");
                        }
                        else
                        {
                            // Si el usuario no fue encontrado
                            TempData["message"] = "La contraseña es incorrecta";
                            TempData["icon"] = "error";
                            return View();
                        }

                    }
                    else
                    {
                        // Si las credenciales no son correctas
                        TempData["message"] = "El usuario no existe";
                        TempData["icon"] = "error";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = "Ocurrio un error en la solicitud: <br>"+ex.Message;
                TempData["icon"] = "error";
                return View();
            }

            return View();
        }

        public ActionResult Logout()
        {
            if (Request.Cookies["UserSession"] != null)
            {
                HttpCookie authCookie = new HttpCookie("UserSession");
                authCookie.Expires = DateTime.Now.AddDays(-1); // Expira inmediatamente
                Response.Cookies.Add(authCookie);
            }

            return RedirectToAction("Login", "Login");
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
