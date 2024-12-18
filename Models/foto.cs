//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace adventureworks.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class foto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public foto()
        {
            this.comentarios = new HashSet<comentario>();
        }
    
        public int foto_id { get; set; }
        public string foto_titulo { get; set; }
        public string foto_file { get; set; }
        [DataType(DataType.MultilineText)]
        public string foto_descripcion { get; set; }
        public Nullable<System.DateTime> foto_fecha_creacion { get; set; }
        public int usuario_id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<comentario> comentarios { get; set; }
        public virtual usuario usuario { get; set; }

        public string FotoFile
        {
            get
            {
                if (this.foto_file != null && this.foto_file.Length > 0)
                {
                    if (this.foto_file.Contains("data:image/png;base64,")
                        || this.foto_file.Contains("data:image/jpg;base64,")
                        || this.foto_file.Contains("data:image/jpeg;base64,")
                        || this.foto_file.Contains("data:image/gif;base64,"))
                    {
                        return this.foto_file;
                    }
                    else
                    {
                        return "data:image/png;base64," + this.foto_file;
                    }
                }
                return "/Content/imagenes/no-image.png";
            }
        }
    }
}
