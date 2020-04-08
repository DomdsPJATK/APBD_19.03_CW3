using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using APBD_19._03_CW3.DTOs.Request;
using APBD_19._03_CW3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APBD_19._03_CW3.DAL
{
    public class SqlStudentServiceDb : IStudentServiceDB
    {
        private int studiesId;
        private int enrollmentId;
        private readonly string databaseURL = "Data Source=db-mssql;Initial Catalog=s19036;Integrated Security=True";

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            using (var client =
                new SqlConnection(databaseURL))
            using (var com = new SqlCommand())
            {
                var studies = request.Studies;
                com.Connection = client;
                client.Open();
                com.Transaction = client.BeginTransaction();

                try
                {
                    com.CommandText = "SELECT * FROM studies WHERE name = @studiesName";
                    com.Parameters.AddWithValue("studiesName", studies.name);
                    var db = com.ExecuteReader();
                    if (!db.Read())
                    {
                        db.Close();
                        com.Transaction.Rollback();
                        return new BadRequestResult();
                    }

                    studiesId = (int) db["IdStudy"];
                    db.Close();

                    com.CommandText =
                        $"Select * FROM Enrollment WHERE Semester = 1 AND IdStudy = {studiesId} AND StartDate = (Select MAX(StartDate) FROM Enrollment WHERE Semester = 1 AND IdS tudy = {studiesId})";
                    db = com.ExecuteReader();
                    if (!db.Read())
                    {
                        db.Close();
                        com.CommandText = "Select MAX(IdEnrollment) From enrollment";
                        enrollmentId = Convert.ToInt32(com.ExecuteScalar()) + 1;
                        db.Close();
                        com.CommandText =
                            $"INSERT INTO enrollment (IdEnrollment,Semester, IdStudy, StartDate) VALUES ({enrollmentId},1,{studiesId},'{DateTime.Now}')";
                        com.ExecuteReader();
                    }
                    else
                    {
                        enrollmentId = Convert.ToInt32(db["IdEnrollment"].ToString());
                    }

                    db.Close();
                    com.CommandText = $"Select * FROM Student WHERE IndexNumber = '{request.IndexNumber}'";
                    db = com.ExecuteReader();
                    if (db.Read())
                    {
                        return new BadRequestResult();
                    }

                    db.Close();
                    com.CommandText =
                        "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES (@index, @firstName, @lastName, @date, @idenroll)";
                    com.Parameters.AddWithValue("index", request.IndexNumber);
                    com.Parameters.AddWithValue("firstName", request.FirstName);
                    com.Parameters.AddWithValue("lastName", request.LastName);
                    com.Parameters.AddWithValue("date", request.BirthDate);
                    com.Parameters.AddWithValue("idenroll", enrollmentId);
                    db = com.ExecuteReader();

                    db.Close();
                    com.ExecuteNonQuery();
                    com.Transaction.Commit();
                    client.Close();
                    return new OkResult();
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.ToString());
                    com.Transaction.Rollback();
                }

                client.Close();
                return new OkResult();
            }
        }
        
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult PromoteStudent(int semester, string studiesName)
        {
            using (var client =
                new SqlConnection(databaseURL))
            using (var com = new SqlCommand())
            {
                com.Connection = client;
                client.Open();
                Console.WriteLine(semester);
                Console.WriteLine(studiesName);
                com.CommandText =
                    "SELECT * FROM Enrollment, Studies WHERE Enrollment.IdStudy = Studies.IdStudy AND Studies.Name LIKE @studiesName AND Enrollment.Semester = @semester";
                com.Parameters.AddWithValue("studiesName", studiesName);
                com.Parameters.AddWithValue("semester", semester);
                var db = com.ExecuteReader();
                if (!db.Read())
                {
                    db.Close();
                    return new NotFoundResult();
                }
                
                db.Close();
                com.CommandText = "exec promoteStudent @studiesName, @semester";
                db = com.ExecuteReader();
                
                db.Close();
                com.ExecuteNonQuery();
                client.Close();
                return new OkResult();
            }
        }
    }
}