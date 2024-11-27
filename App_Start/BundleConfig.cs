using System.Web;
using System.Web.Optimization;

namespace adventureworks
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información sobre los formularios.  De esta manera estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.           
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/css/bootstrap-icons.css",
                "~/Content/css/styles.css"));

            var scriptBundle = new ScriptBundle("~/bundles/scripts")
                .Include(
                "~/Content/js/bootstrap.bundle.min.js",
                "~/Content/js/scripts.js"
                );
            
            // Limpia las transformaciones predeterminadas (incluida la minimización)
            scriptBundle.Transforms.Clear();

            // Añade el bundle modificado
            bundles.Add(scriptBundle);
        }
    }
}
