using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts
{
    /// <summary>
    /// Класс статьи
    /// </summary>
    class Article
    {
        
        public string Header { get; set; }         //заголовок
        public string PicturePath { get; set; }    //путь к картинке
        public string InfoText { get; set; }       //инфа

        public Article(string header, string picturePath, string infoText)
        {
            Header = header;
            PicturePath = picturePath;
            InfoText = infoText;
        }
    }
}
