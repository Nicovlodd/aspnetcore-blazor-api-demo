using Dapper;
using MedicalCare.API.Data;
using MedicalCare.API.Models;

namespace MedicalCare.API.Services
{
    public class SpecialityService : ISpecialityService
    {
        private readonly DbConnectionFactory _connectionFactory;

        //  conexión a la base de datos
        public SpecialityService(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Speciality>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM Speciality";
            return await connection.QueryAsync<Speciality>(sql);
        }

        public async Task<Speciality?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM Speciality WHERE Speciality_Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Speciality>(sql, new { Id = id });
        }

        public async Task<int> CreateAsync(Speciality speciality)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO Speciality (Speciality_Name) 
                        VALUES (@Speciality_Name);
                        SELECT CAST(SCOPE_IDENTITY() as int);";
            return await connection.QuerySingleAsync<int>(sql, speciality);
        }

        public async Task<bool> UpdateAsync(Speciality speciality)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"UPDATE Speciality 
                        SET Speciality_Name = @Speciality_Name 
                        WHERE Speciality_Id = @Speciality_Id";
            var rowsAffected = await connection.ExecuteAsync(sql, speciality);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "DELETE FROM Speciality WHERE Speciality_Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }
    }
}