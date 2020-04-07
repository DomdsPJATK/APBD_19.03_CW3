using System;
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

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            using (var client = new SqlConnection("Data Source=db-mssql;Initial Catalog=s19036;Integrated Security=True"))
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
                    
                    com.CommandText = $"Select MAX(StartDate) FROM Enrollment WHERE Semester = 1 AND IdStudy = {studiesId}";
                    db = com.ExecuteReader();
                    if (db[0].ToString() == "")
                    {
                        db.Close();
                        com.CommandText = "Select MAX(IdEnrollment) From enrollment";
                        enrollmentId = Convert.ToInt32(com.ExecuteScalar()) + 1;
                        db.Close();
                        com.CommandText = $"INSERT INTO enrollment (IdEnrollment,Semester, IdStudy, StartDate) VALUES ({enrollmentId},1,{studiesId},'2015-04-20')";
                        com.ExecuteReader();
                    }
                    else
                    {
                        com.CommandText =
                            $"Select * FROM Enrollment WHERE StartDate LIKE '{db[0].ToString()}' AND Semester = 1 AND IdStudy = {studiesId}";
                        db.Close();
                        db = com.ExecuteReader();
                        enrollmentId = Convert.ToInt32(db["IdEnrollment"].ToString());
                        Console.WriteLine("wchodzi tu");
                    }
                    
                    db.Close();
                    Console.WriteLine(enrollmentId);
                    com.CommandText = $"INSERT INTO Student(FirstName, LastName, IndexNumber, BirthDate, IdEnrollment) VALUES ('{request.FirstName}', '{request.LastName}', '{request.IndexNumber}', '{request.BirthDate.ToString("MM-dd-yy")}',{enrollmentId})";
                    com.ExecuteReader();

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

        public void PromoteStudent(int semester, string studiesName)
        {
            throw new System.NotImplementedException();
        }
        
    }
}