using Dapper;
using MedicalCare.API.Data;
using MedicalCare.API.Models;

namespace MedicalCare.API.Services
{
    public class PatientService : IPatientService
    {
        private readonly DbConnectionFactory _connectionFactory;

        public PatientService(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM Patient";
            return await connection.QueryAsync<Patient>(sql);
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM Patient WHERE Patient_Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Patient>(sql, new { Id = id });
        }

        public async Task<int> CreateAsync(Patient patient)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"INSERT INTO Patient (Patient_RUT, Patient_FirstName, Patient_LastName, Patient_DateOfBirth) 
                        VALUES (@Patient_RUT, @Patient_FirstName, @Patient_LastName, @Patient_DateOfBirth);
                        SELECT CAST(SCOPE_IDENTITY() as int);";
            return await connection.QuerySingleAsync<int>(sql, patient);
        }

        public async Task<bool> UpdateAsync(Patient patient)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"UPDATE Patient 
                        SET Patient_RUT = @Patient_RUT, 
                            Patient_FirstName = @Patient_FirstName, 
                            Patient_LastName = @Patient_LastName, 
                            Patient_DateOfBirth = @Patient_DateOfBirth 
                        WHERE Patient_Id = @Patient_Id";
            var rowsAffected = await connection.ExecuteAsync(sql, patient);
            return rowsAffected > 0;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "DELETE FROM Patient WHERE Patient_Id = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Patient>> SearchAsync(string? rut, string? name, int? minAge, int? maxAge)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            // Iniciamos la consulta base
            var sql = new System.Text.StringBuilder("SELECT * FROM Patient WHERE 1=1");
            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(rut))
            {
                sql.Append(" AND Patient_RUT = @Rut");
                parameters.Add("Rut", rut);
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                sql.Append(" AND (Patient_FirstName LIKE @Name OR Patient_LastName LIKE @Name)");
                parameters.Add("Name", $"%{name}%");
            }

            if (minAge.HasValue)
            {
                sql.Append(" AND DATEDIFF(YEAR, Patient_DateOfBirth, GETDATE()) >= @MinAge");
                parameters.Add("MinAge", minAge.Value);
            }

            if (maxAge.HasValue)
            {
                sql.Append(" AND DATEDIFF(YEAR, Patient_DateOfBirth, GETDATE()) <= @MaxAge");
                parameters.Add("MaxAge", maxAge.Value);
            }

            return await connection.QueryAsync<Patient>(sql.ToString(), parameters);
        }
    }
}