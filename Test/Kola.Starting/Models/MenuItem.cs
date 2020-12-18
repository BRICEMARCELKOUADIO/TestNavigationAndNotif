using System;
using System.Collections.Generic;
using System.Text;

namespace Kola.Starting.Models
{
    public class MenuItemModel
    {
        public string Text { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public bool IsActive { get; set; }
    }
}
