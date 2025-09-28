using Microsoft.AspNetCore.Http; 

namespace DMTAssetManagement.Models
{
    public class Asset
    {
        // Employee Details
        public string? Name { get; set; }
        public string? Date { get; set; }

        // Asset Details
        public string? TypeOfAsset { get; set; }
        public string? CarryOutRequestNo { get; set; }
        public string? ProjectName { get; set; }
        public string? Institute { get; set; }
        public string? ModelName { get; set; }
        public string? ModelCode { get; set; }
        public string? AssetNumber { get; set; }
        public string? IMEI { get; set; }
        public string? SerialNo { get; set; }
        public string? InitiatorStatus { get; set; }

        // Attachment / State Details
        public string? PhysicalVerification { get; set; }
        public int OI_Status { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public int RID { get; set; }
    }

    public class AssetUpdate
    {
        public string? MasterID { get; set; }
        public string? InstanceID { get; set; }
        public bool PhysicalVerification { get; set; } 
        public bool PhysicalVerificationApp { get; set; }
        public int Status { get; set; } 
        public string? Comments { get; set; }
    }
    
    // DTO required for file upload endpoint
    public class FileUploadModel 
    {
        public IFormFile? File { get; set; }
        public string? MasterId { get; set; }
        public string? InstanceId { get; set; }
    }
}