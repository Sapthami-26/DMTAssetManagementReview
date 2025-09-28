using DMTAssetManagement.Models;
using DMTAssetManagement.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DMTAssetManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetController : ControllerBase
    {
        private readonly IAssetRepository _assetRepository;

        public AssetController(IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
        }

        // --- RETRIEVAL APIs ---

        // 1. Get Asset details 
        // SP: Asset Management_GetAssetDataByMasterID. Parameter: @InstanceID [cite: 30]
        [HttpGet("{instanceId}")]
        public async Task<ActionResult<Asset>> GetAssetDetails(string instanceId)
        {
            var asset = await _assetRepository.GetAssetDetailsByInstanceIDAsync(instanceId);
            if (asset == null) return NotFound($"Asset details not found for Instance ID: {instanceId}");
            return Ok(asset);
        }
        
        // 2. Get Attachment Path 
        // SP: AssetManagement_GetAttachmentPathByMasterID. Parameters: @MasterID, @InstanceID [cite: 124, 125, 126]
        [HttpGet("attachmentpath")]
        public async Task<IActionResult> GetAttachmentPath(string masterId, string instanceId)
        {
            var dataTable = await _assetRepository.GetAttachmentPathAsync(masterId, instanceId);

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return NotFound($"Attachment path not found.");
            }
            
            return Ok(dataTable); 
        }

        // 3. Get Asset Master ID 
        // SP: Asset Management_GetAssetMasterID. Parameters: @MasterID, @InstanceID [cite: 143, 144, 145]
        [HttpGet("masterid/{masterId}/{instanceId}")]
        public async Task<IActionResult> GetAssetMasterID(string masterId, string instanceId)
        {
            var dataTable = await _assetRepository.GetAssetMasterIDAsync(masterId, instanceId);

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return NotFound($"Master ID data not found.");
            }
            
            return Ok(dataTable); 
        }

        // --- UPDATE APIs ---

        // 4. Update Asset Data (General Approval/Reject) 
        // SP: AssetManagement_UpdateAssetData. Parameters from AssetUpdate model[cite: 14].
        [HttpPut("approval/general")]
        public async Task<IActionResult> UpdateAssetGeneral([FromBody] AssetUpdate model)
        {
            if (model == null || string.IsNullOrEmpty(model.MasterID)) return BadRequest("Invalid approval data.");
            
            int rowsAffected = await _assetRepository.UpdateAssetDataAsync(model); 

            if (rowsAffected > 0)
            {
                return Ok(new { Message = "Asset data updated successfully (General Approval)." });
            }

            return BadRequest("Failed to update asset data (General Approval).");
        }

        // 5. Update Status on Inward (Inward Approval/Reject) 
        // SP: AssetManagement_UpdateStatusOnInward. Parameters from AssetUpdate model[cite: 16].
        [HttpPut("approval/inward")]
        public async Task<IActionResult> UpdateAssetInward([FromBody] AssetUpdate model)
        {
            if (model == null || string.IsNullOrEmpty(model.MasterID)) return BadRequest("Invalid approval data.");
            
            int rowsAffected = await _assetRepository.UpdateStatusOnInwardAsync(model); 

            if (rowsAffected > 0)
            {
                return Ok(new { Message = "Asset status updated successfully (Inward Approval)." });
            }

            return BadRequest("Failed to update asset status (Inward Approval).");
        }

        // 6. Update Attachment Details (Metadata) 
        // SP: AssetManagement_updateAttachmentDetailsByWF. Parameters: @MasterID, @InstanceID, @FileName, @FilePath [cite: 12]
        [HttpPost("attachment/metadata")]
        public async Task<IActionResult> UpdateAttachmentMetadata(string masterId, string instanceId, string fileName, string filePath)
        {
            if (string.IsNullOrEmpty(masterId) || string.IsNullOrEmpty(instanceId) || string.IsNullOrEmpty(fileName))
            {
                return BadRequest("Missing required parameters for attachment metadata update.");
            }

            int rowsAffected = await _assetRepository.UpdateAttachmentDetailsAsync(masterId, instanceId, fileName, filePath);

            if (rowsAffected > 0)
            {
                return Ok(new { Message = "Attachment metadata updated successfully." });
            }

            return BadRequest("Failed to update attachment metadata.");
        }
        
       
        [HttpPost("attachment/uploadfile")]
        public async Task<IActionResult> UploadAttachmentFile([FromForm] FileUploadModel model) 
        {
            if (model == null || model.File == null || model.File.Length == 0)
            {
                return BadRequest("No file uploaded or missing form data.");
            }
            
            if (string.IsNullOrEmpty(model.MasterId) || string.IsNullOrEmpty(model.InstanceId))
            {
                return BadRequest("Missing MasterId or InstanceId in form data.");
            }

            return Ok(new { Message = $"File {model.File.FileName} received. Metadata must be updated separately using the /attachment/metadata endpoint." });
        }
    }
}