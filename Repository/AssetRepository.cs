using DMTAssetManagement.Models;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace DMTAssetManagement.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private readonly string _connectionString;

        public AssetRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("WFAppConnection") 
                                ?? throw new InvalidOperationException("WFAppConnection string not found.");
        }
        
        // --- UTILITY METHOD for JSON serialization fix ---
        private List<Dictionary<string, object>> DataTableToDictionaryList(DataTable table)
        {
            var list = new List<Dictionary<string, object>>();
            foreach (DataRow row in table.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    dict.Add(col.ColumnName, row[col] == DBNull.Value ? null : row[col]);
                }
                list.Add(dict);
            }
            return list;
        }

        // --- Retrieval Methods ---

        public async Task<Asset> GetAssetDetailsByInstanceIDAsync(string instanceID)
        {
            const string storedProcedure = "AssetManagement_GetAssetDataByMasterID";
            using IDbConnection connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@InstanceID", instanceID, DbType.String, ParameterDirection.Input);
            return await connection.QueryFirstOrDefaultAsync<Asset>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }
        
        // FIX: SqlDataAdapter + serialization (fixes 404/500)
        public async Task<List<Dictionary<string, object>>> GetAttachmentPathAsync(string masterID, string instanceID)
        {
            const string storedProcedure = "AssetManagement_GetAttachmentPathByMasterID";
            var dataTable = new DataTable();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(storedProcedure, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@MasterID", masterID);
            command.Parameters.AddWithValue("@InstanceID", instanceID);
            await connection.OpenAsync();
            using var adapter = new SqlDataAdapter(command);
            adapter.Fill(dataTable); 
            return DataTableToDictionaryList(dataTable);
        }

        // FIX: SqlDataAdapter + serialization (fixes 404/500)
        public async Task<List<Dictionary<string, object>>> GetAssetMasterIDAsync(string masterID, string instanceID)
        {
            const string storedProcedure = "AssetManagement_GetAssetMasterID";
            var dataTable = new DataTable();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(storedProcedure, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@MasterID", masterID);
            command.Parameters.AddWithValue("@InstanceID", instanceID);
            await connection.OpenAsync();
            using var adapter = new SqlDataAdapter(command);
            adapter.Fill(dataTable); 
            return DataTableToDictionaryList(dataTable);
        }


        // --- Update Methods (FIXED) ---

        public async Task<int> UpdateAssetDataAsync(AssetUpdate updateData)
        {
            const string storedProcedure = "AssetManagement_UpdateAssetData";

            using IDbConnection connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            
            parameters.Add("@PhysicalVerification", updateData.PhysicalVerification, DbType.Boolean);
            parameters.Add("@PhysicalVerificationApp", updateData.PhysicalVerificationApp, DbType.Boolean);
            parameters.Add("@Status", updateData.Status, DbType.Int32);
            parameters.Add("@Comments", updateData.Comments, DbType.String);
            
            // FIX: Changed to DbType.Int32
            parameters.Add("@MasterID", updateData.MasterID, DbType.Int32); 
            parameters.Add("@InstanceID", updateData.InstanceID, DbType.Int32); 
            
            // ADDED: Missing parameters for the SP
            parameters.Add("@RID", updateData.RID, DbType.Int32); 
            parameters.Add("@AMTID", updateData.AMTID, DbType.Int32); 

            return await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> UpdateStatusOnInwardAsync(AssetUpdate updateData)
        {
            const string storedProcedure = "AssetManagement_UpdateStatusOnInward";
            
            using IDbConnection connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@PhysicalVerification", updateData.PhysicalVerification, DbType.Boolean);
            parameters.Add("@PhysicalVerificationApp", updateData.PhysicalVerificationApp, DbType.Boolean);
            parameters.Add("@Status", updateData.Status, DbType.Int32);
            parameters.Add("@Comments", updateData.Comments, DbType.String);
            
            // FIX: Changed to DbType.Int32
            parameters.Add("@MasterID", updateData.MasterID, DbType.Int32); 
            parameters.Add("@InstanceID", updateData.InstanceID, DbType.Int32); 

            // ADDED: Assuming this SP also needs RID/AMTID, based on the pattern
            parameters.Add("@RID", updateData.RID, DbType.Int32); 
            parameters.Add("@AMTID", updateData.AMTID, DbType.Int32); 
            
            return await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> UpdateAttachmentDetailsAsync(string masterID, string instanceID, string fileName, string filePath)
        {
            const string storedProcedure = "AssetManagement_updateAttachmentDetailsByWF";

            using IDbConnection connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();

            // NOTE: Keeping these as string as the SP parameters were not shown/confirmed as int
            parameters.Add("@MasterID", masterID, DbType.String);
            parameters.Add("@InstanceID", instanceID, DbType.String);
            parameters.Add("@FileName", fileName, DbType.String);
            parameters.Add("@FilePath", filePath, DbType.String);

            return await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }
    }
}