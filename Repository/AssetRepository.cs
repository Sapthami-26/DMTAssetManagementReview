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
        
        // --- UTILITY METHOD FOR SERIALIZATION FIX ---
        private List<Dictionary<string, object>> DataTableToDictionaryList(DataTable table)
        {
            var list = new List<Dictionary<string, object>>();
            foreach (DataRow row in table.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    // Handle DBNull values correctly
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

            var asset = await connection.QueryFirstOrDefaultAsync<Asset>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return asset; 
        }
        
        // FIX: Now returns a serializable List and uses SqlDataAdapter.
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
            
            // CONVERSION: Convert the DataTable to a serializable List before returning
            return DataTableToDictionaryList(dataTable);
        }

        // FIX: Now returns a serializable List and uses SqlDataAdapter.
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
            
            // CONVERSION: Convert the DataTable to a serializable List before returning
            return DataTableToDictionaryList(dataTable);
        }

        // --- Update Methods ---
        // (These remain correct)
        public async Task<int> UpdateAssetDataAsync(AssetUpdate updateData)
        {
            const string storedProcedure = "AssetManagement_UpdateAssetData";

            using IDbConnection connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            
            parameters.Add("@PhysicalVerification", updateData.PhysicalVerification, DbType.Boolean);
            parameters.Add("@PhysicalVerificationApp", updateData.PhysicalVerificationApp, DbType.Boolean);
            parameters.Add("@Status", updateData.Status, DbType.Int32);
            parameters.Add("@Comments", updateData.Comments, DbType.String);
            parameters.Add("@MasterID", updateData.MasterID, DbType.String); 
            parameters.Add("@InstanceID", updateData.InstanceID, DbType.String); 

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
            parameters.Add("@MasterID", updateData.MasterID, DbType.String); 
            parameters.Add("@InstanceID", updateData.InstanceID, DbType.String); 

            return await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> UpdateAttachmentDetailsAsync(string masterID, string instanceID, string fileName, string filePath)
        {
            const string storedProcedure = "AssetManagement_updateAttachmentDetailsByWF";

            using IDbConnection connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();

            parameters.Add("@MasterID", masterID, DbType.String);
            parameters.Add("@InstanceID", instanceID, DbType.String);
            parameters.Add("@FileName", fileName, DbType.String);
            parameters.Add("@FilePath", filePath, DbType.String);

            return await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
        }
    }
}