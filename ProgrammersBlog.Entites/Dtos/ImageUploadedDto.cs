using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.Entites.Dtos
{
    public class ImageUploadedDto
    {
        public string FullName { get; set; }      // Resmi yükledikten sonra verdiği isim.
        public string OldName { get; set; }       // Resim hangi isimle geldi.
        public string Extension { get; set; }     // Rsmin uzantısı(png vs.)
        public string Path { get; set; }          // Resmin tam yolu
        public string FolderName { get; set; }    // Hangi klasörde
        public long Size { get; set; }            // Boyutu
    }
}
