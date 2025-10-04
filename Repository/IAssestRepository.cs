using DMTAssetManagement.Models;
using System.Data;

namespace DMTAssetManagement.Repositories
{
    public interface IAssetRepository
    {
        // Retrieval SPs
        Task<Asset> GetAssetDetailsByInstanceIDAsync(string instanceID);
        
        // --- UPDATED RETURN TYPE ---
        Task<List<Dictionary<string, object>>> GetAttachmentPathAsync(string masterID, string instanceID);
        Task<List<Dictionary<string, object>>> GetAssetMasterIDAsync(string masterID, string instanceID);
        // ---------------------------

        // Update SPs
        Task<int> UpdateAssetDataAsync(AssetUpdate updateData);
        Task<int> UpdateStatusOnInwardAsync(AssetUpdate updateData);
        Task<int> UpdateAttachmentDetailsAsync(string masterID, string instanceID, string fileName, string filePath);
    }
}