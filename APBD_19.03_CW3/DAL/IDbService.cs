using System.Collections.Generic;
using APBD_19._03_CW3.Model;

namespace APBD_19._03_CW3.DAL
{
    public interface IDbService
    {

        public IEnumerable<Student> GetStudents();

    }
}