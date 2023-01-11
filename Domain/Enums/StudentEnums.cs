using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public class StudentEnums
    {
        public enum Navigation_Menus
        {
            [Description("Master")]
            Master = 1,
            [Description("Student")]
            Student = 2,
            [Description("Parent")]
            Parent = 3,
        }
        public enum NavigationMenu_Master
        {
            [Description("Student")]
            Student = 4,
            [Description("Parent")]
            Parent = 5,
        }
    }
}
