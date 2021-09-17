using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp.netCoreWebAppMVC.Models
{
    public class HonorsStudent
    {
        Student Student { get; set; }

        String HonorsGrade { get; set; }

        int HonorsYear { get; set; }
    }
}
