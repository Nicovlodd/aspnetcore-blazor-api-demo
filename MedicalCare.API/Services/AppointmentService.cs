using Dapper;
using MedicalCare.API.Data;
using MedicalCare.API.Models;

namespace MedicalCare.API.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly DbConnectionFactory _connectionFactory;

        public AppointmentService(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                SELECT 
                    a.Appointment_Id,
                    a.PatientId,
                    ISNULL(p.Patient_FirstName, '') + ' ' + ISNULL(p.Patient_LastName, '') AS Patient_Name,
                    a.DoctorId,
                    'Dr. ' + ISNULL(d.Doctor_FirstName, '') + ' ' + ISNULL(d.Doctor_LastName, '') AS Doctor_Name,
                    ISNULL(s.Speciality_Name, 'General') AS Speciality_Name,
                    a.Appointment_StartUtc,
                    a.Appointment_EndUtc,
                    a.Appointment_Diagnosis,
                    a.Appointment_Room,
                    a.Appointment_Status
                FROM Appointment a
                INNER JOIN Patient p ON a.PatientId = p.Patient_Id
                INNER JOIN Doctor d ON a.DoctorId = d.Doctor_Id
                LEFT JOIN Speciality s ON d.Speciality_Id = s.Speciality_Id
                ORDER BY a.Appointment_StartUtc DESC";

            return await connection.QueryAsync<Appointment>(sql);
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Appointment>(
                "SELECT * FROM Appointment WHERE Appointment_Id = @Id", new { Id = id });
        }

        public async Task<int> CreateAsync(Appointment appointment)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var parameters = new
            {
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                StartUtc = appointment.Appointment_StartUtc,
                EndUtc = appointment.Appointment_EndUtc,
                Diagnosis = appointment.Appointment_Diagnosis,
                Room = appointment.Appointment_Room,
                Status = appointment.Appointment_Status,
                CreatedBy = "AdminTest"
            };
            return await connection.QuerySingleAsync<int>(
                "sp_CreateAppointment", 
                parameters, 
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<bool> UpdateAsync(Appointment appointment)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
                UPDATE Appointment 
                SET PatientId = @PatientId, 
                    DoctorId = @DoctorId, 
                    Appointment_StartUtc = @Appointment_StartUtc,
                    Appointment_EndUtc = @Appointment_EndUtc,
                    Appointment_Diagnosis = @Appointment_Diagnosis,
                    Appointment_Room = @Appointment_Room, 
                    Appointment_Status = @Appointment_Status,
                    Appointment_ModifiedBy = 'AdminTest',
                    Appointment_ModifiedAt = SYSUTCDATETIME()
                WHERE Appointment_Id = @Appointment_Id";

            var rowsAffected = await connection.ExecuteAsync(sql, appointment);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(
                "DELETE FROM Appointment WHERE Appointment_Id = @Id", new { Id = id });
            return rowsAffected > 0;
        }


        public async Task<IEnumerable<Appointment>> SearchAsync(DateTime? date, string? doctorName, string? patientName, string? speciality)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT 
                    a.Appointment_Id,
                    a.PatientId,
                    ISNULL(p.Patient_FirstName, '') + ' ' + ISNULL(p.Patient_LastName, '') AS Patient_Name,
                    a.DoctorId,
                    'Dr. ' + ISNULL(d.Doctor_FirstName, '') + ' ' + ISNULL(d.Doctor_LastName, '') AS Doctor_Name,
                    ISNULL(s.Speciality_Name, 'General') AS Speciality_Name,
                    a.Appointment_StartUtc,
                    a.Appointment_EndUtc,
                    a.Appointment_Diagnosis,
                    a.Appointment_Room,
                    a.Appointment_Status
                FROM Appointment a
                INNER JOIN Patient p ON a.PatientId = p.Patient_Id
                INNER JOIN Doctor d ON a.DoctorId = d.Doctor_Id
                LEFT JOIN Speciality s ON d.Speciality_Id = s.Speciality_Id
                WHERE 1=1";

            var parameters = new DynamicParameters();

            // Filtro por Fecha Exacta 
            if (date.HasValue)
            {
                //se me olvido cambiarlo a append 
                sql += " AND CAST(a.Appointment_StartUtc AS DATE) = CAST(@Date AS DATE)";
                parameters.Add("Date", date.Value.Date);
            }

            if (!string.IsNullOrEmpty(doctorName))
            {
                sql += " AND (d.Doctor_FirstName LIKE @Doc OR d.Doctor_LastName LIKE @Doc)";
                parameters.Add("Doc", $"%{doctorName}%");
            }

            if (!string.IsNullOrEmpty(patientName))
            {
                sql += " AND (p.Patient_FirstName LIKE @Pat OR p.Patient_LastName LIKE @Pat)";
                parameters.Add("Pat", $"%{patientName}%");
            }

            if (!string.IsNullOrEmpty(speciality))
            {
                sql += " AND s.Speciality_Name LIKE @Spec";
                parameters.Add("Spec", $"%{speciality}%");
            }

            sql += " ORDER BY a.Appointment_StartUtc DESC";

            return await connection.QueryAsync<Appointment>(sql, parameters);
        }


        public async Task<IEnumerable<SpecialityDurationReport>> GetAverageDurationBySpecialityAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT 
                    ISNULL(s.Speciality_Name, 'Sin Especialidad Asignada') AS SpecialityName,
                    AVG(DATEDIFF(MINUTE, a.Appointment_StartUtc, a.Appointment_EndUtc)) AS AverageDurationMinutes
                FROM Appointment a
                INNER JOIN Doctor d ON a.DoctorId = d.Doctor_Id
                LEFT JOIN Speciality s ON d.Speciality_Id = s.Speciality_Id
                GROUP BY s.Speciality_Name";

            return await connection.QueryAsync<SpecialityDurationReport>(sql);
        }
    }
}