using Dapper;
using MedicalCare.API.Data;
using MedicalCare.API.Models;

namespace MedicalCare.API.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly DbConnectionFactory _connectionFactory;

        public DoctorService(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"SELECT 
                            d.Doctor_Id,
                            d.Doctor_RUT,
                            d.Doctor_FirstName,
                            d.Doctor_LastName,
                            d.Speciality_Id,
                            s.Speciality_Name
                        FROM Doctor d
                        INNER JOIN Speciality s ON d.Speciality_Id = s.Speciality_Id";
            return await connection.QueryAsync<Doctor>(sql);
        }

        public async Task<Doctor?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM Doctor WHERE Doctor_Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Doctor>(sql, new { Id = id });
        }

        public async Task<int> CreateAsync(Doctor doctor)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO Doctor (Doctor_RUT, Doctor_FirstName, Doctor_LastName, Speciality_Id) 
                        VALUES (@Doctor_RUT, @Doctor_FirstName, @Doctor_LastName, @Speciality_Id);
                        SELECT CAST(SCOPE_IDENTITY() as int);";
            return await connection.QuerySingleAsync<int>(sql, doctor);
        }

        public async Task<bool> UpdateAsync(Doctor doctor)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"UPDATE Doctor 
                        SET Doctor_RUT = @Doctor_RUT,
                            Doctor_FirstName = @Doctor_FirstName, 
                            Doctor_LastName = @Doctor_LastName, 
                            Speciality_Id = @Speciality_Id 
                        WHERE Doctor_Id = @Doctor_Id";
            var rowsAffected = await connection.ExecuteAsync(sql, doctor);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "DELETE FROM Doctor WHERE Doctor_Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }
    }
}