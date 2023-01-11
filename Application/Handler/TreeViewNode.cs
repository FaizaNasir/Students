using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Handlers
{
    public class TreeViewNode
    {
        public int id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public state state { get; set; }
    }
    public class state
    {
        public bool opened { get; set; }
        public bool selected { get; set; }
        public bool disabled { get; set; }
    }
}
