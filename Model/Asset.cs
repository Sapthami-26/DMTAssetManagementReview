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
        // CHANGED to int and added required SP parameters
        public int MasterID { get; set; }
        public int InstanceID { get; set; } 
        public bool PhysicalVerification { get; set; } 
        public bool PhysicalVerificationApp { get; set; }
        public int Status { get; set; } 
        public string? Comments { get; set; }
        public int RID { get; set; }
        public int AMTID { get; set; }
    }
    
    public class FileUploadModel 
    {
        public IFormFile? File { get; set; }
        // Changed to int for consistency with AssetUpdate
        public int MasterId { get; set; }
        public int InstanceId { get; set; }
    }
}