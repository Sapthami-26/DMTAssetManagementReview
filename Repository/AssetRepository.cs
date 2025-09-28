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

        // --- Retrieval Methods ---

        public async Task<Asset> GetAssetDetailsByInstanceIDAsync(string instanceID)
        {
            const string storedProcedure = "Asset Management_GetAssetDataByMasterID";

            using IDbConnection connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@InstanceID", instanceID, DbType.String, ParameterDirection.Input);

            var asset = await connection.QueryFirstOrDefaultAsync<Asset>(
                storedProcedure,
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (asset == null)
            {
                throw new InvalidOperationException($"Asset not found for InstanceID: {instanceID}");
            }
            return asset;
        }
        
        public async Task<DataTable> GetAttachmentPathAsync(string masterID, string instanceID)
        {
            const string storedProcedure = "AssetManagement_GetAttachmentPathByMasterID";

            using IDbConnection connection = new SqlConnection(_connectionString);
            
            var parameters = new DynamicParameters();
            parameters.Add("@MasterID", masterID, DbType.String, ParameterDirection.Input);
            parameters.Add("@InstanceID", instanceID, DbType.String, ParameterDirection.Input);
            
            await connection.QueryAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            
            return new DataTable(); 
        }

        public async Task<DataTable> GetAssetMasterIDAsync(string masterID, string instanceID)
        {
            const string storedProcedure = "Asset Management_GetAssetMasterID";
            using IDbConnection connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@MasterID", masterID, DbType.String, ParameterDirection.Input);
            parameters.Add("@InstanceID", instanceID, DbType.String, ParameterDirection.Input);
            await connection.QueryAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            return new DataTable(); 
        }


        // --- Update Methods ---

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