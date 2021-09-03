using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace asp.netCoreWebAppMVC.Models
{
    public interface IStudentRepository
    {
        Student GetStudentById(int StudentId);
    }
}
